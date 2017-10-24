using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Finds the default class instance for a type.  So for `Eq<string>` it finds `EqString`.  The
    /// result is cached so there's only a one time hit per type to resolve.  
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public static class Class<A>
    {
        public readonly static string Name;
        public readonly static Set<OrdTypeInfo, TypeInfo> All;
        public readonly static Option<string> Error;

        /// <summary>
        /// Default class instance.  If this is null then one couldn't be found, or 
        /// more that one was found.
        /// </summary>
        public readonly static A Default;

        static Class()
        {
            var genParams = typeof(A).GetTypeInfo().GenericTypeArguments.Map(x => x.Name);

            All = ClassInstancesAssembly.ClassInstances
                                        .Find(typeof(A).GetTypeInfo())
                                        .IfNone(Set.empty<OrdTypeInfo, TypeInfo>());

            if(All.Count == 1)
            {
                Default = (A)Activator.CreateInstance(All.Head().AsType());
                return;
            }

            All = toSet<OrdTypeInfo,TypeInfo>( GetOrderableContainerOrd().Select(v=>v.GetTypeInfo()));

            if(All.Count == 1)
            {
                Default = (A)Activator.CreateInstance(All.Head().AsType());
                return;
            }

            All = toSet<OrdTypeInfo,TypeInfo>( GetOrderableContainerOrd2().Select(v=>v.GetTypeInfo()));

            if(All.Count == 1)
            {
                Default = (A)Activator.CreateInstance(All.Head().AsType());
                return;
            }

            //if(All.Count == 0)
            //{
            //    Default = TryHigherKind();
            //    if (Default != null) return;
            //}

            Name = typeof(A).GetTypeInfo().Name.Split('`').Head();
            Name += String.Join("", genParams);

            var defaultTypes = All.Where(t => t.Name == Name).ToList();
            if(defaultTypes.Count == 0)
            {
#if COREFX
                Error = $"Can't find any types in the assemblies loaded called: {Name} that derive from {typeof(A)}.  Because you're using " +
                        $".NET Standard 1.3 you must make sure you have registered the Assembly that contains the types you want to resolve: " +
                         "ClassInstancesAssembly.Register(typeof(MyType).GetTypeInfo().Assembly).  You do not need to do this for the "+
                         "LanguageExt.Core assembly.";
#else
                Error = $"Can't find any types in the assemblies loaded called: {Name} that derive from {typeof(A)}.";
#endif
                return;
            }
            if (defaultTypes.Count > 1)
            {
                Error = $"There are {defaultTypes.Count} types with the same name that derive from {typeof(A)}";
                return;
            }

            // Create the class instance
            Default = (A)Activator.CreateInstance(defaultTypes.Head().AsType());
        }


        private static Option<TypeInfo> ArityOneGeneric(TypeInfo baseInfo)
        {
            return baseInfo.GenericTypeArguments != null && baseInfo.GenericTypeArguments.Length == 1
                ? Some(baseInfo.GenericTypeArguments[0].GetTypeInfo())
                : None;
        }
        private static Option<(TypeInfo,TypeInfo)> ArityTwoGeneric(TypeInfo baseInfo)
        {
            var e = baseInfo.GenericTypeArguments;
            if (e == null || e.Length!=2)
                return None;
            return (e[0].GetTypeInfo(), e[1].GetTypeInfo());
        }
        private static string RemoveGenerics(string name) => FirstCharToUpper(OrdNameMaps(name.Split('`').Head()));

        /// <summary>
        /// The pattern we are trying to match here is
        /// <![CDATA[M<N<T>>]]>
        /// which for examples models <![CDATA[Ord<Set<T>>]]>
        /// The generated type needs to match <![CDATA[MN<MT,T>]]> 
        /// or for our concrete example <![CDATA[OrdSet<OrdInt,Int32>]]>
        /// </summary>
        /// <returns></returns>
        public static Option<Type> GetOrderableContainerOrd()
        {
            var mType = typeof(A).GetTypeInfo();

            var mTypeName = RemoveGenerics( mType.Name );
            return
                from nType in ArityOneGeneric( mType )
                let nTypeName = RemoveGenerics( nType.Name )
                from tType in ArityOneGeneric( nType )
                let tTypeName = RemoveGenerics( tType.Name )
                let mnTypeName = mTypeName + nTypeName + "`2"
                let mtTypeName = mTypeName + tTypeName
                from mnType in ClassInstancesAssembly.Structs.Where( t => t.Name == mnTypeName ).HeadOrNone()
                from ntType in ClassInstancesAssembly.Structs.Where( t => t.Name == mtTypeName ).HeadOrNone()
                select mnType.MakeGenericType( ntType, tType );

        }

        /// <summary>
        /// The pattern we are trying to match here is
        /// <![CDATA[M<N<K,T>>]]>
        /// which for examples models <![CDATA[Ord<Set<OrdT,T>>]]>
        /// The generated type needs to match <![CDATA[MN<OrdT,T>]]> 
        /// or for our concrete example <![CDATA[OrdSet<OrdInt,Int32>]]>
        /// </summary>
        /// <returns></returns>
        public static Option<Type> GetOrderableContainerOrd2()
        {

            var mType = typeof(A).GetTypeInfo();

            var mTypeName = RemoveGenerics( mType.Name );

            return from nType in ArityOneGeneric( mType )
                   let nTypeName = RemoveGenerics( nType.Name )
                   from  ktTypes in ArityTwoGeneric( nType )
                   let kTypeName = RemoveGenerics( ktTypes.Item1.Name )
                   let tTypeName = RemoveGenerics( ktTypes.Item2.Name )
                   let mnTypeName = mTypeName + nTypeName + "`2"
                   from mnType in ClassInstancesAssembly.Structs.Where( t => t.Name == mnTypeName ).HeadOrNone()
                   select mnType.MakeGenericType( ktTypes.Item1, ktTypes.Item2 );

        }


        static string OrdNameMaps(string typename)
        {
            switch (typename)
            {
                case "Int32": return "Int";
                case "Int64": return "Long";
                default: return typename;
            }
        }


        public static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        static A TryHigherKind()
        {
            // Very hacky attempt to get a 'higher kinded' type.  Just playing with some ideas
            // a real version would need to resolve the kind: `A`, the higher-kind (i.e. `Option<>`)
            // and the class instance `MOption<A>` from its inherited type: `Monad<Option<A>, A>>`

            try
            {
                var type = typeof(A).GetTypeInfo();
                var genType = type.GetGenericTypeDefinition();
                if (type.GenericTypeArguments == null || type.GenericTypeArguments.Length < 2) return default(A);

                var last = type.GenericTypeArguments.Last();
                var lastA = genType.GetTypeInfo().GenericTypeParameters.Last();


                var hkType = genType.MakeGenericType(
                    type.GenericTypeArguments
                        .Map(x =>
                            x.GenericTypeArguments.Contains(last)
                                ? x.GetGenericTypeDefinition()
                                : x)
                        .Take(type.GenericTypeArguments.Length - 1)
                        .Append(new[] { lastA })
                        .ToArray()
                        );

                var all = ClassInstancesAssembly.ClassInstances
                                                .Find(hkType.GetTypeInfo())
                                                .IfNone(Set.empty<OrdTypeInfo, TypeInfo>());


                if (all.Count == 1 && all.Head().GenericTypeParameters.Length == 1)
                {
                    return (A)Activator.CreateInstance(all.Head().AsType().MakeGenericType(last));
                }
                else
                {
                    return default(A);
                }
            }
            catch (Exception)
            {
                return default(A);
            }
        }
    }
}
