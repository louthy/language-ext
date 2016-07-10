using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Provides a fluent context when calling the Some(Action) method from
    /// Option<A>.  Must call None(Action) or None(Value) on this context
    /// to complete the matching operation.
    /// </summary>
    /// <typeparam name="A">Bound optional value type</typeparam>
    public struct SomeUnitContext<A>
    {
        readonly Option<A> option;
        readonly Action<A> someHandler;

        internal SomeUnitContext(Option<A> option, Action<A> someHandler)
        {
            this.option = option || Option<A>.None;
            this.someHandler = someHandler;
        }

        /// <summary>
        /// The None branch of the matching operation
        /// </summary>
        /// <param name="noneHandler">None branch operation</param>
        public Unit None(Action noneHandler) =>
            match(option, someHandler, noneHandler);
    }
}
