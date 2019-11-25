using System;
using System.Linq;
using System.Collections.Generic;
using GSet = System.Collections.Generic.HashSet<System.Type>;
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
        public readonly static IReadOnlyCollection<Type> All;
        public readonly static Option<string> Error;

        /// <summary>
        /// Default class instance.  If this is null then one couldn't be found, or 
        /// more that one was found.
        /// </summary>
        public readonly static A Default;

        static Class()
        {
            var genParams = typeof(A).GetTypeInfo().GenericTypeArguments.Map(x => x.Name);

            All = ClassInstancesAssembly.ClassInstances.ContainsKey(typeof(A))
                      ? ClassInstancesAssembly.ClassInstances[typeof(A)]
                      : new GSet();

            if (All.Count == 1)
            {
                Default = (A)Activator.CreateInstance(All.Head());
                return;
            }

            //if(All.Count == 0)
            //{
            //    Default = TryHigherKind();
            //    if (Default != null) return;
            //}

            Name = typeof(A).GetTypeInfo().Name.Split('`').Head();    // Build a friendly name (i.e. Eq<string> becomes EqString)
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
            Default = (A)Activator.CreateInstance(defaultTypes.Head());
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

                var all = ClassInstancesAssembly.ClassInstances.ContainsKey(hkType)
                            ? ClassInstancesAssembly.ClassInstances[hkType]
                            : new GSet();


                if (all.Count == 1 && all.Head().GetTypeInfo().GenericTypeParameters.Length == 1)
                {
                    return (A)Activator.CreateInstance(all.Head().MakeGenericType(last));
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
