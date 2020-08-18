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
        
        static EqAsyncClass()
        {
            try
            {
                var (fullName, name, gens) = ClassFunctions.GetTypeInfo<A>();
                name = ClassFunctions.RemoveAsync(name);
                
                EqualsAsync = ClassFunctions.MakeFunc2<A, A, Task<bool>>(name, "EqualsAsync", gens, 
                    ("Eq", "Async"), ("Ord", "Async"),
                    ("Eq", ""), ("Ord", "")
                    );

                if (EqualsAsync == null && EqClass<A>.Error.IsNone)
                {
                    EqualsAsync = EqClass<A>.EqualsAsync;
                }

                if (EqualsAsync == null)
                {
                    EqualsAsync = (A x, A y) => throw new NotSupportedException(
                        $"Neither Eq{name}Async, Ord{name}Async, Eq{name}, nor Ord{name} instance found for {fullName} (EqualsAsync)"
                    );
                }
            }
            catch (Exception e)
            {
                Error = Some(Common.Error.New(e));
                EqualsAsync = (A x, A y) => throw e;
            }
        }

        public static Task<int> GetHashCodeAsync(A x) =>
            HashableAsyncClass<A>.GetHashCodeAsync(x);    
    }
}
