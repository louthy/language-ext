using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

public static class EqClass
{
    public static bool Equals<A>(A x, A y) => EqClass<A>.Equals(x, y);
    public static Task<bool> EqualsAsync<A>(A x, A y) => EqClass<A>.EqualsAsync(x, y);
    public static int GetHashCode<A>(A x, A y) => EqClass<A>.GetHashCode(x);
    public static Task<int> GetHashCodeAsync<A>(A x, A y) => EqClass<A>.GetHashCodeAsync(x);
}

public static class EqClass<A>
{
    public static readonly Option<Error> Error;
    public new static readonly Func<A, A, bool> Equals;
        
    static EqClass()
    {
        try
        {
            var (fullName, name, gens) = ClassFunctions.GetTypeInfo<A>();
            Equals = (_, _) => throw new NotSupportedException($"Neither Eq{name}, nor Ord{name} instance found for {fullName} (Equals)");
                
            var primFun = MakePrimitiveEquals(fullName);
            if (primFun == null)
            {
                if (Reflect.IsAnonymous(typeof(A)))
                {
                    Equals = IL.EqualsTyped<A>(false);
                }
                else
                {
                    Equals = ClassFunctions.MakeFunc2<A, A, bool>(name, "Equals", gens, ("Eq", ""), ("Ord", "")) ??
                             EqualityComparer<A>.Default.Equals;
                }
            }
            else
            {
                Equals = primFun;
            }
        }
        catch (Exception e)
        {
            Error  = Some(Common.Error.New(e));
            Equals = (_, _) => throw e;
        }
    }
        
    public static Task<bool> EqualsAsync(A x, A y) =>
        Equals(x, y).AsTask();

    public static int GetHashCode(A x) =>
        HashableClass<A>.GetHashCode(x);

    public static Task<int> GetHashCodeAsync(A x) =>
        HashableClass<A>.GetHashCodeAsync(x);        

    static Func<A, A, bool>? MakePrimitiveEquals(string fullName) =>
        fullName switch
        {
            "LangaugeExt.bigint" => (Func<A, A, bool>)(object)(Func<bigint, bigint, bool>)EqBigInt.Equals,
            "System.Boolean"     => (Func<A, A, bool>)(object)(Func<bool, bool, bool>)EqBool.Equals,
            "System.DateTime"    => (Func<A, A, bool>)(object)(Func<DateTime, DateTime, bool>)EqDateTime.Equals,
            "System.Decimal"     => (Func<A, A, bool>)(object)(Func<decimal, decimal, bool>)EqDecimal.Equals,
            "System.Single"      => (Func<A, A, bool>)(object)(Func<float, float, bool>)EqFloat.Equals,
            "System.Double"      => (Func<A, A, bool>)(object)(Func<double, double, bool>)EqDouble.Equals,
            "System.Guid"        => (Func<A, A, bool>)(object)(Func<Guid, Guid, bool>)EqGuid.Equals,
            "System.String"      => (Func<A, A, bool>)(object)(Func<string, string, bool>)EqString.Equals,
            "System.Char"        => (Func<A, A, bool>)(object)(Func<char, char, bool>)EqChar.Equals,
            "System.Int16"       => (Func<A, A, bool>)(object)(Func<short, short, bool>)EqShort.Equals,
            "System.Int32"       => (Func<A, A, bool>)(object)(Func<int, int, bool>)EqInt.Equals,
            "System.Int64"       => (Func<A, A, bool>)(object)(Func<long, long, bool>)EqLong.Equals,
            "System.TypeInfo"    => (Func<A, A, bool>)(object)(Func<TypeInfo, TypeInfo, bool>)EqTypeInfo.Equals,
            "System.Exception"   => (Func<A, A, bool>)(object)(Func<Exception, Exception, bool>)EqException.Equals,
            _                    => null
        };        
}
