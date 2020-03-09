using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public static class EqAsyncClass
    {
        public static Task<bool> EqualsAsync<A>(A x, A y) => EqAsyncClass<A>.EqualsAsync(x, y);
        public static Task<int> GetHashCodeAsync<A>(A x, A y) => EqAsyncClass<A>.GetHashCodeAsync(x);
    }

    public static class EqAsyncClass<A>
    {
        public static readonly Option<Error> Error;
        public static readonly Func<A, A, Task<bool>> EqualsAsync;
        public static readonly Func<A, Task<int>> GetHashCodeAsync = HashableAsyncClass<A>.GetHashCodeAsync;
        
        static EqAsyncClass()
        {
            try
            {
                var (fullName, name, gens) = ClassFunctions.GetTypeInfo<A>();
                EqualsAsync = ClassFunctions.MakeFunc2<A, A, Task<bool>>(name, "EqualsAsync", gens, 
                    ("Eq", "Async"), ("Ord", "Async"),
                    ("Eq", ""), ("Ord", "")
                    );
            }
            catch (Exception e)
            {
                Error = Some(Common.Error.New(e));
            }
        }
    }
}
