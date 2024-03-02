using System;
using System.Collections.Generic;
using System.Reflection;

namespace LanguageExt.Traits.Resolve;

public static class HashableResolve<A>
{
    public static string? ResolutionError;

    public static Func<A, int> GetHashCodeFunc = null!;
    public static MethodInfo GetHashCodeMethod = null!;
    public static nint GetHashCodeMethodPtr;

    public static int GetHashCode(A value) =>
        GetHashCodeFunc(value);

    public static bool Exists => 
        ResolutionError is null;
    
    static HashableResolve()
    {
        var source = typeof(A);
        
        var impl = Resolver.Find(source, "Hashable");
        if (impl is null)
        {
            ResolutionError = $"Trait implementation not found for: {typeof(A).Name}";
            MakeDefault();
            return;
        }
        
        var m = Resolver.Method(impl, "GetHashCode", source);
        if (m is null)
        {
            ResolutionError = $"`GetHashCode` method not found for: {typeof(A).Name}";
            MakeDefault();
            return;
        }

        GetHashCodeMethod    = m;
        GetHashCodeMethodPtr = m.MethodHandle.GetFunctionPointer();
        GetHashCodeFunc      = x => (int?)GetHashCodeMethod.Invoke(null, [x]) ?? throw new InvalidOperationException();
    }
    
    static void MakeDefault()
    {
        GetHashCodeFunc      = DefaultGetHashCode;
        GetHashCodeMethod    = GetHashCodeFunc.Method;
        GetHashCodeMethodPtr = GetHashCodeFunc.Method.MethodHandle.GetFunctionPointer();
    }

    static int DefaultGetHashCode(A value) =>
        value is null ? 0 : value.GetHashCode();
}
