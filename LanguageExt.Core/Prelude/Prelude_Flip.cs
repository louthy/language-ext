using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Reverse the order of the arguments to a curried function
        /// </summary>
        [Pure]
        public static Func<B, Func<A, R>> flip<A, B, R>(Func<A, Func<B, R>> f) =>
            b => a => f(a)(b);

        /// <summary>
        /// Reverse the order of the arguments to a curried function
        /// </summary>
        [Pure]
        public static Func<C, Func<B, Func<A, R>>> flip<A, B, C, R>(Func<A, Func<B, Func<C, R>>> f) =>
            c => b => a => f(a)(b)(c);

        /// <summary>
        /// Reverse the order of the arguments to a function
        /// </summary>
        [Pure]
        public static Func<B, A, R> flip<A, B, R>(Func<A, B, R> f) =>
            (b, a) => f(a, b);

        /// <summary>
        /// Reverse the order of the arguments to a function
        /// </summary>
        [Pure]
        public static Func<C, B, A, R> flip<A, B, C, R>(Func<A, B, C, R> f) =>
            (c, b, a) => f(a, b, c);

        /// <summary>
        /// Reverse the order of the arguments to a curried function
        /// </summary>
        [Pure]
        public static Func<B, Func<A, R>> Flip<A, B, R>(this Func<A, Func<B, R>> f) =>
            b => a => f(a)(b);

        /// <summary>
        /// Reverse the order of the arguments to a curried function
        /// </summary>
        [Pure]
        public static Func<C, Func<B, Func<A, R>>> Flip<A, B, C, R>(this Func<A, Func<B, Func<C, R>>> f) =>
            c => b => a => f(a)(b)(c);

        /// <summary>
        /// Reverse the order of the arguments to a function
        /// </summary>
        [Pure]
        public static Func<B, A, R> Flip<A, B, R>(this Func<A, B, R> f) =>
            (b, a) => f(a, b);

        /// <summary>
        /// Reverse the order of the arguments to a function
        /// </summary>
        [Pure]
        public static Func<C, B, A, R> Flip<A, B, C, R>(this Func<A, B, C, R> f) =>
            (c, b, a) => f(a, b, c);
    }
}
