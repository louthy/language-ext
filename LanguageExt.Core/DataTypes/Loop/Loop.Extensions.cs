using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class LoopExtensions
{
    extension<A>(Loop<A> lhs)
    {
        /// <summary>
        /// Value extraction
        /// </summary>
        public static A operator >>(Loop<A> px, Lower _) =>
            px.Value;
    }

    extension<A, B>(Loop<A> lhs)
    {
        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="f">Function</param>
        /// <returns>Result of invoking the function</returns>
        public static Loop<B> operator >>(Loop<A> x, Func<A, B> f) =>
            new (f(x.Value));
    }   
}
