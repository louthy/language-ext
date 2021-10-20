using System.Reflection;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GSet = System.Collections.Generic.HashSet<System.Type>;
using Dict = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.HashSet<System.Type>>;

namespace LanguageExt.ClassInstances
{
    public class ClassInstancesAssembly
    {
        public static ClassInstancesAssembly singleton;
        
        public List<Type> Types;
        public List<Type> Structs;
        public List<Type> AllClassInstances;
        public Dict ClassInstances;
        
        /// <summary>
        /// If the caching throws an error, this will be set.
        /// </summary>
        public readonly Option<Exception> Error;

        /// <summary>
        /// Singleton access
        /// </summary>
        public static ClassInstancesAssembly Default =>
            singleton ?? (singleton = new ClassInstancesAssembly());
        
        /// <summary>
        /// Force the caching of class instances.  If you run this at start-up then
        /// there's a much better chance the system will find all assemblies that
        /// have class instances in them.  Not a requirement though.
        /// </summary>
        public static Unit Initialise() =>
            ignore(Default);        

        /// <summary>
        /// Ctor
        /// </summary>
        public ClassInstancesAssembly()
        {
            try
            {
                Debug.WriteLine("Internal ctor");

                var asms = GetAssemblies().ToList();

                Debug.WriteLine($"Assemblies collected");

                var asmNames = (from nam in asms
                                where nam != null && nam.Name != "mscorlib" && !nam.Name.StartsWith("System.") && !nam.Name.StartsWith("Microsoft.")
                                select nam)
                               .ToList();

                Debug.WriteLine($"Assemblies filtered");

                var loadedAsms = (from nam in asmNames
                                  let asm = SafeLoadAsm(nam)
                                  where asm != null
                                  select asm)
                                 .ToList();

                Debug.WriteLine($"Assemblies loaded");

                var allTypes = (from asm in loadedAsms
                                from typ in SafeGetTypes(asm)
                                select typ)
                               .ToList();
                
                Debug.WriteLine($"Types collected");
                
                Types = (from typ in allTypes
                         where typ != null && !typ.FullName.StartsWith("<") && !typ.FullName.Contains("+<")
                         select typ)
                        .ToList();

                Debug.WriteLine($"Types found: {Types.Count}");

                Structs = Types.Filter(t => t?.IsValueType ?? false).ToList();
                
                Debug.WriteLine($"Structs found: {Structs.Count}");
                
                AllClassInstances = Structs.Filter(t => t?.GetTypeInfo().ImplementedInterfaces?.Exists(i => i == typeof(Typeclass)) ?? false).ToList();

                Debug.WriteLine($"AllClassInstances found: {AllClassInstances.Count}");
                
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
                
                Debug.WriteLine($"ClassInstances found: {ClassInstances.Count}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Internal error: {e}");
                Error = e;
            }
        }

        Assembly SafeLoadAsm(AssemblyName name)
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
        
        
        Type[] SafeGetTypes(Assembly asm)
        {
            try
            {
                return asm.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null).ToArray();
            }
            catch
            {
                return new Type[0];
            }
        }

        IEnumerable<AssemblyName> GetAssemblies()
        {
            var asmNames = Enumerable.Concat(
                    Assembly.GetEntryAssembly()?.GetReferencedAssemblies() ?? new AssemblyName[0],
                    Enumerable.Concat(
                        Assembly.GetCallingAssembly()?.GetReferencedAssemblies() ?? new AssemblyName[0],
                        Assembly.GetExecutingAssembly()?.GetReferencedAssemblies() ?? new AssemblyName[0]))
                .Distinct();

            var init = new[] {Assembly.GetEntryAssembly()?.GetName(), Assembly.GetCallingAssembly()?.GetName(), Assembly.GetExecutingAssembly()?.GetName()};

            foreach (var asm in Enumerable.Concat(init, asmNames).Where(n => n != null).Distinct())
            {
                yield return asm;
            }
        }
    }
}
