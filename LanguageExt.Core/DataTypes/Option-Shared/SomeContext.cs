using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Provides a fluent context when calling the Some(Func) method from
    /// a member of the Optional<A> type-class.  Must call None(Func) or 
    /// None(Value) on this context to complete the matching operation.
    /// </summary>
    /// <typeparam name="A">Bound optional value type</typeparam>
    /// <typeparam name="B">The operation return value type</typeparam>
    public class SomeContext<OPT, OA, A, B> where OPT : struct, Optional<OA, A>
    {
        readonly OA option;
        readonly Func<A, B> someHandler;
        readonly bool unsafeOpt;

        internal SomeContext(OA option, Func<A, B> someHandler, bool unsafeOpt)
        {
            this.option = option;
            this.someHandler = someHandler;
            this.unsafeOpt = unsafeOpt;
        }

        /// <summary>
        /// The None branch of the matching operation
        /// </summary>
        /// <param name="noneHandler">None branch operation</param>
        public B None(Func<B> noneHandler) =>
            unsafeOpt
                ? default(OPT).MatchUnsafe(option, someHandler, noneHandler)
                : default(OPT).Match(option, someHandler, noneHandler);

        /// <summary>
        /// The None branch of the matching operation
        /// </summary>
        /// <param name="noneHandler">None branch operation</param>
        public B None(B noneValue) =>
            None(() => noneValue);

    }
}
