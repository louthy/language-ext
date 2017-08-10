using System.Reflection;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;

namespace LanguageExt.ClassInstances
{
#if COREFX
    public
#endif
    static class ClassInstancesAssembly
    {
        internal static Lst<TypeInfo> Types;
        internal static Lst<TypeInfo> Structs;
        internal static Lst<TypeInfo> AllClassInstances;
        internal static Map<OrdTypeInfo, TypeInfo, Set<OrdTypeInfo, TypeInfo>> ClassInstances;

        static ClassInstancesAssembly()
        {
#if COREFX
            // We can't go looking for types, so let's settle for what's in lang-ext
            Types = typeof(ClassInstancesAssembly).GetTypeInfo().Assembly.DefinedTypes.Freeze();
#else

            Types = (from nam in Assembly.GetEntryAssembly().GetReferencedAssemblies()
                     where nam != null
                     let asm = Assembly.Load(nam)
                     where asm != null
                     from typ in asm.GetTypes()
                     where typ != null
                     select typ.GetTypeInfo())
                    .Append(Assembly.GetEntryAssembly()?.GetTypes()?.Map(t => t.GetTypeInfo()) ?? new TypeInfo[0])
                    .Freeze();

#endif
            Structs = Types.Filter(t => t?.IsValueType ?? false);
            AllClassInstances = Structs.Filter(t => t?.ImplementedInterfaces?.Exists(i => i == typeof(Typeclass)) ?? false);
            ClassInstances = new Map<OrdTypeInfo, TypeInfo, Set<OrdTypeInfo, TypeInfo>>();
            foreach(var ci in AllClassInstances)
            {
                var typeClasses = ci?.ImplementedInterfaces
                                    ?.Filter(i => typeof(Typeclass).GetTypeInfo().IsAssignableFrom(i.GetTypeInfo()))
                                    ?.Map(t => t.GetTypeInfo())
                                    ?.Freeze() ?? Lst<TypeInfo>.Empty;

                ClassInstances = typeClasses.Fold(ClassInstances, (s, x) => s.AddOrUpdate(x, Some: cis => cis.AddOrUpdate(ci), None: () => Set<OrdTypeInfo, TypeInfo>(ci)));
            }
        }

#if COREFX
        public static Unit Register(Assembly asm)
        {
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
            return unit;
        }
#endif
    }
}
