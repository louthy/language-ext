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
        Action noneHandler;

        internal SomeUnitContext(OA option, Action<A> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        /// <summary>
        /// The None branch of the matching operation
        /// </summary>
        /// <param name="noneHandler">None branch operation</param>
        public Unit None(Action f)
        {
            noneHandler = f;
            return default(OPT).Match(option, HandleSome, HandleNone);
        }

        Unit HandleSome(A value)
        {
            someHandler(value);
            return unit;
        }

        Unit HandleNone()
        {
            noneHandler();
            return unit;
        }
    }

    /// <summary>
    /// Provides a fluent context when calling the Some(Action) method from
    /// OptionalUnsafe<A> type-class.  Must call None(Action) or None(Value) on this 
    /// context to complete the matching operation.
    /// </summary>
    /// <typeparam name="A">Bound optional value type</typeparam>
    public class SomeUnsafeUnitContext<OPT, OA, A>
        where OPT : struct, OptionalUnsafe<OA, A>
    {
        readonly OA option;
        readonly Action<A> someHandler;
        Action noneHandler;

        internal SomeUnsafeUnitContext(OA option, Action<A> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        /// <summary>
        /// The None branch of the matching operation
        /// </summary>
        /// <param name="noneHandler">None branch operation</param>
        public Unit None(Action f)
        {
            noneHandler = f;
            return default(OPT).MatchUnsafe(option, HandleSome, HandleNone);
        }

        Unit HandleSome(A value)
        {
            someHandler(value);
            return unit;
        }

        Unit HandleNone()
        {
            noneHandler();
            return unit;
        }
    }

}
