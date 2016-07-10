using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Provides a fluent context when calling the Some(Func) method from
    /// Option<A>.  Must call None(Func) or None(Value) on this context
    /// to complete the matching operation.
    /// </summary>
    /// <typeparam name="A">Bound optional value type</typeparam>
    /// <typeparam name="B">The operation return value type</typeparam>
    public struct SomeContext<A, B>
    {
        readonly Option<A> option;
        readonly Func<A, B> someHandler;

        internal SomeContext(Option<A> option, Func<A, B> someHandler)
        {
            this.option = option || Option<A>.None;
            this.someHandler = someHandler;
        }

        /// <summary>
        /// The None branch of the matching operation
        /// </summary>
        /// <param name="noneHandler">None branch operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public B None(Func<B> noneHandler) =>
            match(option, someHandler, noneHandler);

        /// <summary>
        /// The None branch of the matching operation
        /// </summary>
        /// <param name="noneValue">None branch operation return value</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public B None(B noneValue) =>
            match(option, someHandler, () => noneValue);
    }
}
