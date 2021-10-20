using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
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

                if (Equals == null)
                {
                    Equals = (A x, A y) => throw new NotSupportedException(
                        $"Neither Eq{name}, nor Ord{name} instance found for {fullName} (Equals)"
                    );
                }
            }
            catch (Exception e)
            {
                Error = Some(Common.Error.New(e));
                Equals = (A x, A y) => throw e;
            }
        }
        
        public static Task<bool> EqualsAsync(A x, A y) =>
            Equals(x, y).AsTask();

        public static int GetHashCode(A x) =>
            HashableClass<A>.GetHashCode(x);

        public static Task<int> GetHashCodeAsync(A x) =>
            HashableClass<A>.GetHashCodeAsync(x);        

        static Func<A, A, bool> MakePrimitiveEquals(string fullName) =>
            fullName switch
            {
                "LangaugeExt.bigint" => (Func<A, A, bool>)(object)(Func<bigint, bigint, bool>)default(EqBigInt).Equals,
                "System.Boolean" => (Func<A, A, bool>)(object)(Func<bool, bool, bool>)default(EqBool).Equals,
                "System.DateTime" => (Func<A, A, bool>)(object)(Func<DateTime, DateTime, bool>)default(EqDateTime).Equals,
                "System.Decimal" => (Func<A, A, bool>)(object)(Func<decimal, decimal, bool>)default(EqDecimal).Equals,
                "System.Single" => (Func<A, A, bool>)(object)(Func<float, float, bool>)default(EqFloat).Equals,
                "System.Double" => (Func<A, A, bool>)(object)(Func<double, double, bool>)default(EqDouble).Equals,
                "System.Guid" => (Func<A, A, bool>)(object)(Func<Guid, Guid, bool>)default(EqGuid).Equals,
                "System.String" => (Func<A, A, bool>)(object)(Func<string, string, bool>)default(EqString).Equals,
                "System.Char" => (Func<A, A, bool>)(object)(Func<char, char, bool>)default(EqChar).Equals,
                "System.Int16" => (Func<A, A, bool>)(object)(Func<short, short, bool>)default(EqShort).Equals,
                "System.Int32" => (Func<A, A, bool>)(object)(Func<int, int, bool>)default(EqInt).Equals,
                "System.Int64" => (Func<A, A, bool>)(object)(Func<long, long, bool>)default(EqLong).Equals,
                "System.TypeInfo" => (Func<A, A, bool>)(object)(Func<TypeInfo, TypeInfo, bool>)default(EqTypeInfo).Equals,
                "System.Exception" => (Func<A, A, bool>)(object)(Func<Exception, Exception, bool>)default(EqException).Equals,
                _ => null
            };        
    }
}
