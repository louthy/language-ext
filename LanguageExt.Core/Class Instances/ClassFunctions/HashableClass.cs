using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
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
                var primFun = MakePrimitiveGetHashCode(fullName);
                if (primFun == null)
                {
                    GetHashCode = ClassFunctions.MakeFunc1<A, int>(name, "GetHashCode", gens, ("Hashable", ""), ("Eq", ""), ("Ord", "")) ??
                                  new Func<A, int>(x => x?.GetHashCode() ?? 0);
                }
                else
                {
                    GetHashCode = primFun;
                }

                if (GetHashCode == null)
                {
                    GetHashCode = (A x) => throw new NotSupportedException($"Neither Hashable{name}, Eq{name}, nor Ord{name} instance found for {fullName} (GetHashCode)");
                }
            }
            catch (Exception e)
            {
                Error = Some(Common.Error.New(e));
                GetHashCode = (A x) => throw e;
            }
        }
        
        public static Task<int> GetHashCodeAsync(A x) =>
            GetHashCode(x).AsTask();             

        static Func<A, int> MakePrimitiveGetHashCode(string fullName) =>
            fullName switch
            {
                "LangaugeExt.bigint" => (Func<A, int>)(object)(Func<bigint, int>)default(HashableBigInt).GetHashCode,
                "System.Boolean" => (Func<A, int>)(object)(Func<bool, int>)default(HashableBool).GetHashCode,
                "System.DateTime" => (Func<A, int>)(object)(Func<DateTime, int>)default(HashableDateTime).GetHashCode,
                "System.Decimal" => (Func<A, int>)(object)(Func<decimal, int>)default(HashableDecimal).GetHashCode,
                "System.Single" => (Func<A, int>)(object)(Func<float, int>)default(HashableFloat).GetHashCode,
                "System.Double" => (Func<A, int>)(object)(Func<double, int>)default(HashableDouble).GetHashCode,
                "System.Guid" => (Func<A, int>)(object)(Func<Guid, int>)default(HashableGuid).GetHashCode,
                "System.String" => (Func<A, int>)(object)(Func<string, int>)default(HashableString).GetHashCode,
                "System.Char" => (Func<A, int>)(object)(Func<char, int>)default(HashableChar).GetHashCode,
                "System.Int16" => (Func<A, int>)(object)(Func<short, int>)default(HashableShort).GetHashCode,
                "System.Int32" => (Func<A, int>)(object)(Func<int, int>)default(HashableInt).GetHashCode,
                "System.Int64" => (Func<A, int>)(object)(Func<long, int>)default(HashableLong).GetHashCode,
                "System.TypeInfo" => (Func<A, int>)(object)(Func<TypeInfo, int>)default(HashableTypeInfo).GetHashCode,
                "System.Exception" => (Func<A, int>)(object)(Func<Exception, int>)default(HashableException).GetHashCode,
                _ => null
            };
    }
}
