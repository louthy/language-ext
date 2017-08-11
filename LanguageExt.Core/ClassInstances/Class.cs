using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

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
    }
}
