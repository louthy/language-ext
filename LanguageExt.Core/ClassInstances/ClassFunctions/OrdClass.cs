using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public static class OrdClass
    {
        public static int Compare<A>(A x, A y) => OrdClass<A>.Compare(x, y);
        public static Task<int> CompareAsync<A>(A x, A y) => OrdClass<A>.CompareAsync(x, y);
        public static bool Equals<A>(A x, A y) => OrdClass<A>.Equals(x, y);
        public static Task<bool> EqualsAsync<A>(A x, A y) => OrdClass<A>.EqualsAsync(x, y);
        public static int GetHashCode<A>(A x, A y) => OrdClass<A>.GetHashCode(x);
        public static Task<int> GetHashCodeAsync<A>(A x, A y) => OrdClass<A>.GetHashCodeAsync(x);
    }

    public static class OrdClass<A>
    {
        public static readonly Option<Error> Error;
        public static readonly Func<A, A, int> Compare;
        public static readonly Func<A, A, Task<int>> CompareAsync = (x, y) => Compare(x, y).AsTask();
        public new static readonly Func<A, A, bool> Equals = EqClass<A>.Equals;
        public static readonly Func<A, A, Task<bool>> EqualsAsync = EqClass<A>.EqualsAsync;
        public new static readonly Func<A, int> GetHashCode = HashableClass<A>.GetHashCode;
        public static readonly Func<A, Task<int>> GetHashCodeAsync =HashableClass<A>.GetHashCodeAsync;
        
        static OrdClass()
        {
            try
            {
                var (fullName, name, gens) = ClassFunctions.GetTypeInfo<A>();
                
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

                if (Compare == null)
                {
                    Compare = (A x, A y) => throw new NotSupportedException($"Ord{name} instance not found for {fullName} (Compare)");
                }
            }
            catch (Exception e)
            {
                Error = Some(Common.Error.New(e));
                Compare = (A x, A y) => throw e;
            }
        }

        static Func<A, A, int> MakePrimitiveCompare(string fullName) =>
            fullName switch
            {
                "LangaugeExt.bigint" => (Func<A, A, int>)(object)(Func<bigint, bigint, int>)default(OrdBigInt).Compare,
                "System.intean" => (Func<A, A, int>)(object)(Func<bool, bool, int>)default(OrdBool).Compare,
                "System.DateTime" => (Func<A, A, int>)(object)(Func<DateTime, DateTime, int>)default(OrdDateTime).Compare,
                "System.Decimal" => (Func<A, A, int>)(object)(Func<decimal, decimal, int>)default(OrdDecimal).Compare,
                "System.Single" => (Func<A, A, int>)(object)(Func<float, float, int>)default(OrdFloat).Compare,
                "System.Double" => (Func<A, A, int>)(object)(Func<double, double, int>)default(OrdDouble).Compare,
                "System.Guid" => (Func<A, A, int>)(object)(Func<Guid, Guid, int>)default(OrdGuid).Compare,
                "System.String" => (Func<A, A, int>)(object)(Func<string, string, int>)default(OrdString).Compare,
                "System.Char" => (Func<A, A, int>)(object)(Func<char, char, int>)default(OrdChar).Compare,
                "System.Int16" => (Func<A, A, int>)(object)(Func<short, short, int>)default(OrdShort).Compare,
                "System.Int32" => (Func<A, A, int>)(object)(Func<int, int, int>)default(OrdInt).Compare,
                "System.Int64" => (Func<A, A, int>)(object)(Func<long, long, int>)default(OrdLong).Compare,
                "System.TypeInfo" => (Func<A, A, int>)(object)(Func<TypeInfo, TypeInfo, int>)default(OrdTypeInfo).Compare,
                "System.Exception" => (Func<A, A, int>)(object)(Func<Exception, Exception, int>)default(OrdException).Compare,
                _ => null
            };        
    }
}
