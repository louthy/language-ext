using System;
using System.Linq;
using System.Collections.Generic;
using GSet = System.Collections.Generic.HashSet<System.Type>;
using System.Reflection;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Finds the default class instance for a type.  So for `Eq<string>` it finds `EqString`.  The
/// result is cached so there's only a one time hit per type to resolve.  
/// </summary>
/// <typeparam name="A"></typeparam>
public static class Class<A> 
{
    public static readonly string? Name;
    public static readonly IReadOnlyCollection<Type>? All;
    public static readonly Option<string> Error;

    /// <summary>
    /// Default class instance.  If this is null then one couldn't be found, or 
    /// more that one was found.
    /// </summary>
    public static readonly A? Default;

    static Class()
    {
        var genParams = typeof(A).GetTypeInfo().GenericTypeArguments.Map(x => x.Name);

        if (ClassInstancesAssembly.Default.ClassInstances == null) return;
        
        All = ClassInstancesAssembly.Default.ClassInstances.ContainsKey(typeof(A))
                  ? ClassInstancesAssembly.Default.ClassInstances[typeof(A)]
                  : new GSet();

        if (All.Count == 1)
        {
            Default = (A)Activator.CreateInstance(All.Head())!;
            return;
        }

        //if(All.Count == 0)
        //{
        //    Default = TryHigherKind();
        //    if (Default != null) return;
        //}

        Name =  typeof(A).GetTypeInfo().Name.Split('`').Head(); // Build a friendly name (i.e. Eq<string> becomes EqString)
        Name += String.Join("", genParams);

        var defaultTypes = All.Where(t => t.Name == Name).ToList();
        if(defaultTypes.Count == 0)
        {
            Error = $"Can't find any types in the assemblies loaded called: {Name} that derive from {typeof(A)}.";
            return;
        }
        if (defaultTypes.Count > 1)
        {
            Error = $"There are {defaultTypes.Count} types with the same name that derive from {typeof(A)}";
            return;
        }

        // Create the class instance
        Default = (A)Activator.CreateInstance(defaultTypes.Head())!;
    }
}
