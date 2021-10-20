using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public static class OrdAsyncClass
    {
        public static Task<int> CompareAsync<A>(A x, A y) => OrdAsyncClass<A>.CompareAsync(x, y);
        public static Task<bool> EqualsAsync<A>(A x, A y) => OrdAsyncClass<A>.EqualsAsync(x, y);
        public static Task<int> GetHashCodeAsync<A>(A x, A y) => OrdAsyncClass<A>.GetHashCodeAsync(x);
    }

    public static class OrdAsyncClass<A>
    {
        public static readonly Option<Error> Error;
        public static readonly Func<A, A, Task<int>> CompareAsync;
        
        static OrdAsyncClass()
        {
            try
            {
                var (fullName, name, gens) = ClassFunctions.GetTypeInfo<A>();
                name = ClassFunctions.RemoveAsync(name);
                CompareAsync = ClassFunctions.MakeFunc2<A, A, Task<int>>(name, "CompareAsync", gens,
                    ("Ord", "Async"),
                    ("Ord", ""));
                
                if (CompareAsync == null && OrdClass<A>.Error.IsNone)
                {
                    CompareAsync = OrdClass<A>.CompareAsync;
                }

                if (CompareAsync == null)
                {
                    CompareAsync = new Func<A, A, Task<int>>((A x, A y) =>
                        throw new NotSupportedException(
                            $"Neither Ord{name}Async, nor Ord{name} instance found for {fullName} (CompareAsync)"));
                }
            }
            catch (Exception e)
            {
                Error = Some(Common.Error.New(e));
                CompareAsync = (A x, A y) => throw e;
            }
        }

        public static Task<bool> EqualsAsync(A x, A y) =>
            EqAsyncClass<A>.EqualsAsync(x, y);

        public static Task<int> GetHashCodeAsync(A x) =>
            HashableAsyncClass<A>.GetHashCodeAsync(x);
    }
}
