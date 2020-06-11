using System.Reflection;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using GSet = System.Collections.Generic.HashSet<System.Type>;
using Dict = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.HashSet<System.Type>>;

namespace LanguageExt.ClassInstances
{
    public static class ClassInstancesAssembly
    {
        internal static List<Type> Types;
        internal static List<Type> Structs;
        internal static List<Type> AllClassInstances;
        internal static Dict ClassInstances;

        static Assembly SafeLoadAsm(AssemblyName name)
        {
            try
            {
                return Assembly.Load(name);
            }
            catch
            {
                return null;
            }
        }

        static IEnumerable<AssemblyName> GetAssemblies()
        {
            var asmNames = EnumerableOptimal.ConcatFast(
                               Assembly.GetEntryAssembly()?.GetReferencedAssemblies() ?? new AssemblyName[0],
                                   EnumerableOptimal.ConcatFast(
                                        Assembly.GetCallingAssembly()?.GetReferencedAssemblies() ?? new AssemblyName[0],
                                        Assembly.GetExecutingAssembly()?.GetReferencedAssemblies() ?? new AssemblyName[0]))
                                    .Distinct();

            var init = new[] {
                Assembly.GetEntryAssembly()?.GetName(),
                Assembly.GetCallingAssembly()?.GetName(),
                Assembly.GetExecutingAssembly()?.GetName() }
                .Filter(n => n != null);

            var set = Set<OrdString, string>();

            foreach (var asm in init.Append(asmNames))
            {
                if (!set.Contains(asm.FullName))
                {
                    set = set.Add(asm.FullName);
                    yield return asm;
                }
            }
        }

        static ClassInstancesAssembly()
        {
            try
            {
#if COREFX13
                // We can't go looking for types, so let's settle for what's in lang-ext
                Types = typeof(ClassInstancesAssembly).GetTypeInfo().Assembly.DefinedTypes.Freeze();
#else

                Types = (from nam in GetAssemblies().ToList()
                         where nam != null && nam.Name != "mscorlib" && !nam.Name.StartsWith("System.") && !nam.Name.StartsWith("Microsoft.")
                         let asm = SafeLoadAsm(nam)
                         where asm != null
                         from typ in asm.GetTypes()
                         where typ != null && !typ.FullName.StartsWith("<") && !typ.FullName.Contains("+<")
                         select typ)
                        .ToList();

#endif
                Structs = Types.Filter(t => t?.IsValueType ?? false).ToList();
                AllClassInstances = Structs.Filter(t => t?.GetTypeInfo().ImplementedInterfaces?.Exists(i => i == typeof(Typeclass)) ?? false).ToList();
                ClassInstances = new Dict();
                foreach (var ci in AllClassInstances)
                {
                    var typeClasses = ci?.GetTypeInfo().ImplementedInterfaces
                                        ?.Filter(i => typeof(Typeclass).GetTypeInfo().IsAssignableFrom(i.GetTypeInfo()))
                                        ?.ToList() ?? new List<Type>();

                    foreach(var typeClass in typeClasses)
                    {
                        if(ClassInstances.ContainsKey(typeClass))
                        {
                            ClassInstances[typeClass].Add(ci);
                        }
                        else
                        {
                            var nset = new GSet();
                            nset.Add(ci);
                            ClassInstances.Add(typeClass, nset);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Error = e;
            }
        }

        /// <summary>
        /// If the caching throws an error, this will be set.
        /// </summary>
        public static readonly Option<Exception> Error;

        /// <summary>
        /// Force the caching of class instances.  If you run this at start-up then
        /// there's a much better chance the system will find all assemblies that
        /// have class instances in them.  Not a requirement though.
        /// </summary>
        public static Unit Initialise() => unit;

        public static Unit Register(Assembly asm)
        {
#if COREFX13
            Types = Types.AddRange(asm.DefinedTypes);
            var newStructs = asm.DefinedTypes.Filter(t => t.IsValueType).ToList();
            Structs = Structs.AddRange(newStructs);
            var newClassInstances = newStructs.Filter(t => t.ImplementedInterfaces.Exists(i => i == typeof(Typeclass)));
            AllClassInstances = AllClassInstances.AddRange(newClassInstances);

            foreach (var ci in newClassInstances)
            {
                var typeClasses = ci.ImplementedInterfaces
                                    .Filter(i => typeof(Typeclass).GetTypeInfo().IsAssignableFrom(i.GetTypeInfo()))
                                    .Map(t => t.GetTypeInfo())
                                    .Freeze();

                ClassInstances = typeClasses.Fold(ClassInstances, (s, x) => s.AddOrUpdate(x, Some: cis => cis.AddOrUpdate(ci), None: () => Set<OrdTypeInfo, TypeInfo>(ci)));
            }
#endif
            return unit;
        }

        //static readonly HashSet<A> primativeTypes = HashSet()
        
        public static Func<A, A, bool> MakeEquals<A>()
        {
            return null;
            
            /*var typeA = typeof(A);
            var name = typeA.Name;
            var prim = MakePrimativeEq()
            
            
            if (typeof(A).FullName.StartsWith($"LanguageExt.{name}`"))
            {
                var genA = typeof(A).GenericTypeArguments[0];
                var tryA = eqType.MakeGenericType(genA);
                var eq = tryA.GetMethod("Equals", new Type[] {typeof(A), typeof(A)});
            
                var lhs = Expression.Parameter(typeof(A), "lhs");
                var rhs = Expression.Parameter(typeof(A), "rhs");

                var lambda = Expression.Lambda<Func<A, A, bool>>(Expression.Call(Expression.Default(tryA), eq, lhs, rhs), lhs, rhs);
                return lambda.Compile();
            }
            else
            {
                return null;
            }*/
        }
    }
}
