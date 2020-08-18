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
        public static List<Type> Types =>
            Internal.Types;

        public static List<Type> Structs =>
            Internal.Structs;

        public static List<Type> AllClassInstances =>
            Internal.AllClassInstances;

        public static Dict ClassInstances =>
            Internal.ClassInstances;

        /// <summary>
        /// If the caching throws an error, this will be set.
        /// </summary>
        public static Option<Exception> Error =>
            Internal.Error;

        /// <summary>
        /// Force the caching of class instances.  If you run this at start-up then
        /// there's a much better chance the system will find all assemblies that
        /// have class instances in them.  Not a requirement though.
        /// </summary>
        public static Unit Initialise() =>
            unit;
        
        static class Internal
        {
            public static List<Type> Types;
            public static List<Type> Structs;
            public static List<Type> AllClassInstances;
            public static Dict ClassInstances;

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

                var init = new[] {Assembly.GetEntryAssembly()?.GetName(), Assembly.GetCallingAssembly()?.GetName(), Assembly.GetExecutingAssembly()?.GetName()}
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

            static Internal()
            {
                try
                {

                    Types = (from nam in GetAssemblies().ToList()
                            where nam != null && nam.Name != "mscorlib" && !nam.Name.StartsWith("System.") && !nam.Name.StartsWith("Microsoft.")
                            let asm = SafeLoadAsm(nam)
                            where asm != null
                            from typ in asm.GetTypes()
                            where typ != null && !typ.FullName.StartsWith("<") && !typ.FullName.Contains("+<")
                            select typ)
                        .ToList();

                    Structs = Types.Filter(t => t?.IsValueType ?? false).ToList();
                    AllClassInstances = Structs.Filter(t => t?.GetTypeInfo().ImplementedInterfaces?.Exists(i => i == typeof(Typeclass)) ?? false).ToList();
                    ClassInstances = new Dict();
                    foreach (var ci in AllClassInstances)
                    {
                        var typeClasses = ci?.GetTypeInfo().ImplementedInterfaces
                            ?.Filter(i => typeof(Typeclass).GetTypeInfo().IsAssignableFrom(i.GetTypeInfo()))
                            ?.ToList() ?? new List<Type>();

                        foreach (var typeClass in typeClasses)
                        {
                            if (ClassInstances.ContainsKey(typeClass))
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
                catch (Exception e)
                {
                    Error = e;
                }
            }

            /// <summary>
            /// If the caching throws an error, this will be set.
            /// </summary>
            public static readonly Option<Exception> Error;
        }
    }
}
