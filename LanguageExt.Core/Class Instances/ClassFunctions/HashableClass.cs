using System;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public static class HashableClass
    {
        public static int GetHashCode<A>(A x, A y) => HashableClass<A>.GetHashCode(x);
        public static Task<int> GetHashCodeAsync<A>(A x, A y) => HashableClass<A>.GetHashCodeAsync(x);
    }

    public static class HashableClass<A>
    {
        public static readonly Option<Error> Error;
        public new static readonly Func<A, int> GetHashCode;
        
        static HashableClass()
        {
            try
            {
                var (fullName, name, gens) = ClassFunctions.GetTypeInfo<A>();
                GetHashCode = _ => throw new NotSupportedException($"Neither Hashable{name}, Eq{name}, nor Ord{name} instance found for {fullName} (GetHashCode)");
                var primFun = MakePrimitiveGetHashCode(fullName);
                if (primFun == null)
                {
                    GetHashCode = ClassFunctions.MakeFunc1<A, int>(name, "GetHashCode", gens, ("Hashable", ""), ("Eq", ""), ("Ord", "")) ??
                                  (x => x?.GetHashCode() ?? 0);
                }
                else
                {
                    GetHashCode = primFun;
                }
            }
            catch (Exception e)
            {
                Error = Some(Common.Error.New(e));
                GetHashCode = _ => throw e;
            }
        }
        
        public static Task<int> GetHashCodeAsync(A x) =>
            GetHashCode(x).AsTask();             

        static Func<A, int>? MakePrimitiveGetHashCode(string fullName) =>
            fullName switch
            {
                "LangaugeExt.bigint" => (Func<A, int>)(object)(Func<bigint, int>)HashableBigInt.GetHashCode,
                "System.Boolean" => (Func<A, int>)(object)(Func<bool, int>)HashableBool.GetHashCode,
                "System.DateTime" => (Func<A, int>)(object)(Func<DateTime, int>)HashableDateTime.GetHashCode,
                "System.Decimal" => (Func<A, int>)(object)(Func<decimal, int>)HashableDecimal.GetHashCode,
                "System.Single" => (Func<A, int>)(object)(Func<float, int>)HashableFloat.GetHashCode,
                "System.Double" => (Func<A, int>)(object)(Func<double, int>)HashableDouble.GetHashCode,
                "System.Guid" => (Func<A, int>)(object)(Func<Guid, int>)HashableGuid.GetHashCode,
                "System.String" => (Func<A, int>)(object)(Func<string, int>)HashableString.GetHashCode,
                "System.Char" => (Func<A, int>)(object)(Func<char, int>)HashableChar.GetHashCode,
                "System.Int16" => (Func<A, int>)(object)(Func<short, int>)HashableShort.GetHashCode,
                "System.Int32" => (Func<A, int>)(object)(Func<int, int>)HashableInt.GetHashCode,
                "System.Int64" => (Func<A, int>)(object)(Func<long, int>)HashableLong.GetHashCode,
                "System.TypeInfo" => (Func<A, int>)(object)(Func<TypeInfo, int>)HashableTypeInfo.GetHashCode,
                "System.Exception" => (Func<A, int>)(object)(Func<Exception, int>)HashableException.GetHashCode,
                _ => null
            };
    }
}
