using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LanguageExt.Traits.Resolve;

public static class OrdResolve<A>
{
    public static string? ResolutionError;
    
    public static Func<A, int> GetHashCodeFunc = null!;
    public static MethodInfo GetHashCodeMethod = null!;
    public static nint GetHashCodeMethodPtr;
    
    public static Func<A, A, bool> EqualsFunc = null!;
    public static MethodInfo EqualsMethod = null!;
    public static nint EqualsMethodPtr;
    
    public static Func<A, A, int> CompareFunc = null!;
    public static MethodInfo CompareMethod = null!;
    public static nint CompareMethodPtr;
    
    public static int GetHashCode(A value) =>
        GetHashCodeFunc(value);

    public static bool Equals(A lhs, A rhs) =>
        EqualsFunc(lhs, rhs);

    public static int Compare(A lhs, A rhs) =>
        CompareFunc(lhs, rhs);

    public static bool Exists => 
        ResolutionError is null;

    static OrdResolve()
    {
        var source = typeof(A);

        if(source.FullName?.StartsWith("LanguageExt.Traits.K") ?? false)
        {
            MakeTraitDefault();
            return;
        }
        
        if (typeof(Delegate).IsAssignableFrom(source))
        {
            MakeDelegateDefault();
            return;
        }

        MakeComparer(source);
    }
    
    static void MakeComparer(Type source)
    {
        var impl = Resolver.Find(source, "Ord");
        if (impl is null)
        {
            ResolutionError = $"Trait implementation not found for: {source.Name}";
            MakeDefault();
            return;
        }
        
        // Compare
        
        var m = Resolver.Method(impl, "Compare", source, source);
        if (m is null)
        {
            ResolutionError = $"`Compare` method not found for: {source.Name}";
            MakeDefault();
            return;
        }

        CompareMethod    = m;
        CompareMethodPtr = m.MethodHandle.GetFunctionPointer();
        CompareFunc      = (x, y) => (int?)CompareMethod.Invoke(null, [x, y]) ?? throw new InvalidOperationException();

        // Equals
        
        m = Resolver.Method(impl, "Equals", source, source);
        if (m is null)
        {
            ResolutionError = $"`Equals` method not found for: {source.Name}";
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
            ResolutionError = $"`GetHashCode` method not found for: {source.Name}";
            MakeDefault();
            return;
        }

        GetHashCodeMethod    = m;
        GetHashCodeMethodPtr = m.MethodHandle.GetFunctionPointer();
        GetHashCodeFunc      = x => (int?)GetHashCodeMethod.Invoke(null, [x]) ?? throw new InvalidOperationException();
    }
    
    static void MakeDefault()
    {
        CompareFunc      = Comparer<A>.Default.Compare;
        CompareMethod    = CompareFunc.Method;
        CompareMethodPtr = CompareFunc.Method.MethodHandle.GetFunctionPointer();
        
        EqualsFunc      = EqualityComparer<A>.Default.Equals;
        EqualsMethod    = EqualsFunc.Method;
        EqualsMethodPtr = EqualsFunc.Method.MethodHandle.GetFunctionPointer();
        
        GetHashCodeFunc      = DefaultGetHashCode;
        GetHashCodeMethod    = GetHashCodeFunc.Method;
        GetHashCodeMethodPtr = GetHashCodeFunc.Method.MethodHandle.GetFunctionPointer();
    }

    static void MakeDelegateDefault()
    {
        CompareFunc = (x, y) => ((object?)x, (object?)y) switch
                                {
                                    (Delegate dx, Delegate dy) => dx.Method.MetadataToken.CompareTo(dy.Method.MetadataToken),
                                    _                          => -1
                                };
        CompareMethod    = CompareFunc.Method;
        CompareMethodPtr = CompareFunc.Method.MethodHandle.GetFunctionPointer();
        
        EqualsFunc = (x, y) => ((object?)x, (object?)y) switch
                               {
                                   (Delegate dx, Delegate dy) => dx.Method.MetadataToken == dy.Method.MetadataToken,
                                   _                          => false
                               };
        EqualsMethod    = EqualsFunc.Method;
        EqualsMethodPtr = EqualsFunc.Method.MethodHandle.GetFunctionPointer();
        
        GetHashCodeFunc      = DefaultGetHashCode;
        GetHashCodeMethod    = GetHashCodeFunc.Method;
        GetHashCodeMethodPtr = GetHashCodeFunc.Method.MethodHandle.GetFunctionPointer();
    }

    static void MakeTraitDefault()
    {
        var gens = typeof(A).GetGenericArguments();

        var fname = gens[0].FullName;
        var tick  = fname?.IndexOf('`') ?? -1; 
        var iname = tick >= 0 ? fname?.Substring(0, tick) ?? "" : fname;

        var tgens = gens[0].GetGenericArguments();
        var gtype = gens[0].Assembly.GetType($"{iname}`{tgens.Length + 1}");

        var ngens = tgens.Concat([gens[1]]).ToArray();
        var type  = gtype!.MakeGenericType(ngens);

        var resolver = typeof(OrdResolve<>).MakeGenericType(type);

        var getHashCodeObj = ((Delegate?)resolver.GetField("GetHashCodeFunc")?.GetValue(null) ?? throw new InvalidOperationException()).Target;
        var equalsObj      = ((Delegate?)resolver.GetField("EqualsFunc")?.GetValue(null)      ?? throw new InvalidOperationException()).Target;
        var compareObj     = ((Delegate?)resolver.GetField("CompareFunc")?.GetValue(null)     ?? throw new InvalidOperationException()).Target;

        var getHashCodeMethod = (MethodInfo?)resolver.GetField("GetHashCodeMethod")?.GetValue(null) ?? throw new InvalidOperationException();
        var equalsMethod      = (MethodInfo?)resolver.GetField("EqualsMethod")?.GetValue(null)      ?? throw new InvalidOperationException();
        var compareMethod     = (MethodInfo?)resolver.GetField("CompareMethod")?.GetValue(null)     ?? throw new InvalidOperationException();

        GetHashCodeFunc      = x => (int?)getHashCodeMethod.Invoke(getHashCodeObj, [x]) ?? throw new InvalidOperationException(); 
        GetHashCodeMethod    = GetHashCodeFunc.Method;
        GetHashCodeMethodPtr = GetHashCodeFunc.Method.MethodHandle.GetFunctionPointer();
        
        EqualsFunc      = (x, y) => (bool?)equalsMethod.Invoke(equalsObj, [x, y]) ?? throw new InvalidOperationException();
        EqualsMethod    = EqualsFunc.Method;
        EqualsMethodPtr = EqualsFunc.Method.MethodHandle.GetFunctionPointer();
        
        CompareFunc      = (x, y) => (int?)compareMethod.Invoke(compareObj, [x, y]) ?? throw new InvalidOperationException();
        CompareMethod    = CompareFunc.Method;
        CompareMethodPtr = CompareFunc.Method.MethodHandle.GetFunctionPointer();
    }
    
    static int DefaultGetHashCode(A value) =>
        value is null ? 0 : value.GetHashCode();
}
