using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public static class HashableAsyncClass
    {
        public static Task<int> GetHashCodeAsync<A>(A x, A y) => HashableAsyncClass<A>.GetHashCodeAsync(x);
    }

    public static class HashableAsyncClass<A>
    {
        public static readonly Option<Error> Error;
        public static readonly Func<A, Task<int>> GetHashCodeAsync;
        
        static HashableAsyncClass()
        {
            try
            {
                var (fullName, name, gens) = ClassFunctions.GetTypeInfo<A>();
                GetHashCodeAsync = ClassFunctions.MakeFunc1<A, Task<int>>(name, "GetHashCodeAsync", gens, 
                    ("Hashable", "Async"), ("Eq", "Async"), ("Ord", "Async"),
                    ("Hashable", ""), ("Eq", ""), ("Ord", "")
                    );
            }
            catch (Exception e)
            {
                Error = Some(Common.Error.New(e));
            }
        }
    }
}
