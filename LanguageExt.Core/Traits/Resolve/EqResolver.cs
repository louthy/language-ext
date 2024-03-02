using System;
using System.Collections.Generic;
using System.Reflection;

namespace LanguageExt.Traits.Resolve;

public static class EqResolve<A>
{
    public static string? ResolutionError;

    public static Func<A, int> GetHashCodeFunc = null!;
    public static MethodInfo GetHashCodeMethod = null!;
    public static nint GetHashCodeMethodPtr;
    
    public static Func<A, A, bool> EqualsFunc = null!;
    public static MethodInfo EqualsMethod = null!;
    public static nint EqualsMethodPtr;
    
    public static int GetHashCode(A value) =>
        GetHashCodeFunc(value);

    public static bool Equals(A lhs, A rhs) =>
        EqualsFunc(lhs, rhs);

    public static bool Exists => 
        ResolutionError is null;

    static EqResolve()
    {
        var source = typeof(A);
        
        var impl = Resolver.Find(source, "Eq");
        if (impl is null)
        {
            ResolutionError = $"Trait implementation not found for: {typeof(A).Name}";
            MakeDefault();
            return;
        }
        
        // Equals
        
        var m = Resolver.Method(impl, "Equals", source, source);
        if (m is null)
        {
            ResolutionError = $"`Equals` method not found for: {typeof(A).Name}";
            MakeDefault();
            return;
        }

        EqualsMethod    = m;
        EqualsMethodPtr = m.MethodHandle.GetFunctionPointer();
        EqualsFunc      = (x, y) => (bool?)EqualsMethod.Invoke(null, [x, y]) ?? throw new InvalidOperationException();
        
        // GetHashCode
        
        m = Resolver.Method(impl, "GetHashCode", source);
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
        EqualsFunc      = EqualityComparer<A>.Default.Equals;
        EqualsMethod    = EqualsFunc.Method;
        EqualsMethodPtr = EqualsFunc.Method.MethodHandle.GetFunctionPointer();
        
        GetHashCodeFunc      = DefaultGetHashCode;
        GetHashCodeMethod    = GetHashCodeFunc.Method;
        GetHashCodeMethodPtr = GetHashCodeFunc.Method.MethodHandle.GetFunctionPointer();
    }

    static int DefaultGetHashCode(A value) =>
        value is null ? 0 : value.GetHashCode();
}
