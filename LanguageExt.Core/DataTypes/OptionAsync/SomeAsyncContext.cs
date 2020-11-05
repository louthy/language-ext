using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public class SomeAsyncContext<OPT, OA, A, B> where OPT : struct, OptionalAsync<OA, A>
    {
        private readonly OA option;
        private readonly Func<A, B> someHandler;

        internal SomeAsyncContext(OA option, Func<A, B> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        /// <summary>The None branch of the matching operation</summary>
        /// <param name="noneHandler">None branch operation</param>
        public Task<B> None(Func<B> noneHandler) => default (OPT).Match(option, someHandler, noneHandler);

        /// <summary>The None branch of the matching operation</summary>
        /// <param name="noneValue">None branch value</param>
        public Task<B> None(B noneValue) => default (OPT).Match(option, someHandler, () => noneValue);
    }
}
