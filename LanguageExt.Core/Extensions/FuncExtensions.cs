using System;
using System.Diagnostics.Contracts;

namespace LanguageExt;

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

    extension<A, B, C>(Func<A, B> lhs)
    {
        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="f">First function</param>
        /// <param name="g">Second function</param>
        /// <returns>Composed function</returns>
        public static Func<A, C> operator >>(Func<A, B> f, Func<B, C> g) =>
            x => g(f(x));
    }
    
    extension<A, B, C>(Func<B, C> lhs)
    {
        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="f">First function</param>
        /// <param name="g">Second function</param>
        /// <returns>Composed function</returns>
        public static Func<A, C> operator <<(Func<B, C> g, Func<A, B> f) =>
            x => g(f(x));        
    }

    extension<A, B>(Pure<A> lhs)
    {
        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="f">Function</param>
        /// <returns>Result of invoking the function</returns>
        public static B operator >>(Pure<A> x, Func<A, B> f) =>
            f(x.Value);
    }

    extension<A, B>(Func<B, A> lhs)
    {
        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="f">Function</param>
        /// <param name="x">Input</param>
        /// <returns>Result of invoking the function</returns>
        public static A operator >>(Func<B, A> f, Pure<B>  x) =>
            f(x.Value);
    }
    
}
