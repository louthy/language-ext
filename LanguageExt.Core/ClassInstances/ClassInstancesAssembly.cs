using System.Reflection;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;

namespace LanguageExt.ClassInstances
{
    public static class ClassInstancesAssembly
    {
        internal static Lst<TypeInfo> Types;
        internal static Lst<TypeInfo> Structs;
        internal static Lst<TypeInfo> AllClassInstances;
        internal static Map<OrdTypeInfo, TypeInfo, Set<OrdTypeInfo, TypeInfo>> ClassInstances;

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
            var asmNames = Enumerable.Concat(
                               Assembly.GetEntryAssembly()?.GetReferencedAssemblies() ?? new AssemblyName[0],
                                   Enumerable.Concat(
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

                Types = (from nam in GetAssemblies().Freeze()
                         where nam != null && nam.Name != "mscorlib" && !nam.Name.StartsWith("System.")
                         let asm = SafeLoadAsm(nam)
                         where asm != null
                         from typ in asm.GetTypes()
                         where typ != null
                         select typ.GetTypeInfo())
                        .Freeze();

#endif
                Structs = Types.Filter(t => t?.IsValueType ?? false);
                AllClassInstances = Structs.Filter(t => t?.ImplementedInterfaces?.Exists(i => i == typeof(Typeclass)) ?? false);
                ClassInstances = new Map<OrdTypeInfo, TypeInfo, Set<OrdTypeInfo, TypeInfo>>();
                foreach (var ci in AllClassInstances)
                {
                    var typeClasses = ci?.ImplementedInterfaces
                                        ?.Filter(i => typeof(Typeclass).GetTypeInfo().IsAssignableFrom(i.GetTypeInfo()))
                                        ?.Map(t => t.GetTypeInfo())
                                        ?.Freeze() ?? Lst<TypeInfo>.Empty;

                    ClassInstances = typeClasses.Fold(ClassInstances, (s, x) => s.AddOrUpdate(x, Some: cis => cis.AddOrUpdate(ci), None: () => Set<OrdTypeInfo, TypeInfo>(ci)));
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
    }
}
