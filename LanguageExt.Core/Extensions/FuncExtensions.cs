using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static class FuncExtensions
    {
        /// <summary>
        /// Flip the arguments in a two argument function
        /// </summary>
        [Pure]
        public static Func<B, A, C> Flip<A, B, C>(this Func<A, B, C> f) =>
            (b, a) => f(a, b);

        /// <summary>
        /// Flip the arguments in a two argument function
        /// </summary>
        [Pure]
        public static Func<B, Func<A, C>> Flip<A, B, C>(this Func<A, Func<B, C>> f) =>
            b => a => f(a)(b);
    }
}
