using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

public static class OrdClass
{
    public static int Compare<A>(A x, A y) => OrdClass<A>.Compare(x, y);
    public static bool Equals<A>(A x, A y) => OrdClass<A>.Equals(x, y);
    public static int GetHashCode<A>(A x, A y) => OrdClass<A>.GetHashCode(x);
}

public static class OrdClass<A>
{
    public static readonly Option<Error> Error;
    public static readonly Func<A, A, int> Compare;
        
    static OrdClass()
    {
        try
        {
            var (fullName, name, gens) = ClassFunctions.GetTypeInfo<A>();
            Compare                    = (_, _) => throw new NotSupportedException($"Ord{name} instance not found for {fullName} (Compare)");
            
            var primFun = MakePrimitiveCompare(fullName);
            if (primFun == null)
            {
                if (Reflect.IsAnonymous(typeof(A)))
                {
                    Compare = IL.Compare<A>(false);
                }
                else
                {
                    Compare = ClassFunctions.MakeFunc2<A, A, int>(name, "Compare", gens, ("Ord", "")) ??
                              Comparer<A>.Default.Compare;
                }
            }
            else
            {
                Compare = primFun;
            }
        }
        catch (Exception e)
        {
            Error   = Some(Common.Error.New(e));
            Compare = (_, _) => throw e;
        }
    }

    public static Task<int> CompareAsync(A x, A y) =>
        Compare(x, y).AsTask();

    public static bool Equals(A x, A y) =>
        EqClass<A>.Equals(x, y);

    public static Task<bool> EqualsAsync(A x, A y) =>
        EqClass<A>.EqualsAsync(x, y);

    public static int GetHashCode(A x) =>
        HashableClass<A>.GetHashCode(x);

    public static Task<int> GetHashCodeAsync(A x) =>
        HashableClass<A>.GetHashCodeAsync(x);
        
    static Func<A, A, int>? MakePrimitiveCompare(string fullName) =>
        fullName switch
        {
            "LangaugeExt.bigint" => (Func<A, A, int>)(object)(Func<bigint, bigint, int>)OrdBigInt.Compare,
            "System.Boolean"     => (Func<A, A, int>)(object)(Func<bool, bool, int>)OrdBool.Compare,
            "System.DateTime"    => (Func<A, A, int>)(object)(Func<DateTime, DateTime, int>)OrdDateTime.Compare,
            "System.Decimal"     => (Func<A, A, int>)(object)(Func<decimal, decimal, int>)OrdDecimal.Compare,
            "System.Single"      => (Func<A, A, int>)(object)(Func<float, float, int>)OrdFloat.Compare,
            "System.Double"      => (Func<A, A, int>)(object)(Func<double, double, int>)OrdDouble.Compare,
            "System.Guid"        => (Func<A, A, int>)(object)(Func<Guid, Guid, int>)OrdGuid.Compare,
            "System.String"      => (Func<A, A, int>)(object)(Func<string, string, int>)OrdString.Compare,
            "System.Char"        => (Func<A, A, int>)(object)(Func<char, char, int>)OrdChar.Compare,
            "System.Int16"       => (Func<A, A, int>)(object)(Func<short, short, int>)OrdShort.Compare,
            "System.Int32"       => (Func<A, A, int>)(object)(Func<int, int, int>)OrdInt.Compare,
            "System.Int64"       => (Func<A, A, int>)(object)(Func<long, long, int>)OrdLong.Compare,
            "System.TypeInfo"    => (Func<A, A, int>)(object)(Func<TypeInfo, TypeInfo, int>)OrdTypeInfo.Compare,
            "System.Exception"   => (Func<A, A, int>)(object)(Func<Exception, Exception, int>)OrdException.Compare,
            _                    => null
        };        
}
