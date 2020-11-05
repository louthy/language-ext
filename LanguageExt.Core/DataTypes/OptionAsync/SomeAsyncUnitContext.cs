using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public class SomeAsyncUnitContext<OPT, OA, A> where OPT : struct, OptionalAsync<OA, A>
    {
        private readonly OA option;
        private readonly Action<A> someHandler;
        private Action noneHandler;

        internal SomeAsyncUnitContext(OA option, Action<A> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        /// <summary>The None branch of the matching operation</summary>
        /// <param name="noneHandler">None branch operation</param>
        public Task<Unit> None(Action f)
        {
            noneHandler = f;
            return default (OPT).Match(option, HandleSome, HandleNone);
        }

        private Unit HandleSome(A value)
        {
            someHandler(value);
            return Prelude.unit;
        }

        private Unit HandleNone()
        {
            noneHandler();
            return Prelude.unit;
        }
    }
}
