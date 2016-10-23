using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Provides a fluent context when calling the Some(Action) method from
    /// Optional<A> type-class.  Must call None(Action) or None(Value) on this 
    /// context to complete the matching operation.
    /// </summary>
    /// <typeparam name="A">Bound optional value type</typeparam>
    public class SomeUnitContext<OPT, OA, A> 
        where OPT  : struct, Optional<OA, A>
    {
        readonly OA option;
        readonly Action<A> someHandler;
        readonly bool unsafeOpt;

        internal SomeUnitContext(OA option, Action<A> someHandler, bool unsafeOpt)
        {
            this.option = option;
            this.someHandler = someHandler;
            this.unsafeOpt = unsafeOpt;
        }

        /// <summary>
        /// The None branch of the matching operation
        /// </summary>
        /// <param name="noneHandler">None branch operation</param>
        public Unit None(Action noneHandler) =>
            unsafeOpt
                ? default(OPT).MatchUnsafe(option, x => { someHandler(x); return unit; }, () => { noneHandler(); return unit; })
                : default(OPT).Match(option, x => { someHandler(x); return unit; }, () => { noneHandler(); return unit; });
    }
}
