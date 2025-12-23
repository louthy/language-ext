using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class PureExtensions
{
    extension<A>(Pure<A> lhs)
    {
        /// <summary>
        /// Value extraction
        /// </summary>
        public static A operator >>(Pure<A> px, Lower _) =>
            px.Value;
    }

    extension<A, B>(Pure<A> lhs)
    {
        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="f">Function</param>
        /// <returns>Result of invoking the function</returns>
        public static Pure<B> operator >>(Pure<A> x, Func<A, B> f) =>
            new (f(x.Value));
    }   
    
    /// <summary>
    /// Monadic join
    /// </summary>
    /// <param name="mma">Nested `Pure` monad</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Flattened monad</returns>
    public static Pure<A> Flatten<A>(this Pure<Pure<A>> mma) =>
        mma.Value;

    public static Validation<F, B> Bind<F, A, B>(this Pure<A> ma, Func<A, Validation<F, B>> bind)
        where F : Monoid<F> =>
        bind(ma.Value);

    public static Validation<F, A> ToValidation<F, A>(this Pure<A> ma)
        where F : Monoid<F> =>
        Validation.Success<F, A>(ma.Value);

    public static Validation<F, C> SelectMany<F, A, B, C>(
        this Pure<A> ma,
        Func<A, Validation<F, B>> bind,
        Func<A, B, C> project)
        where F : Monoid<F> =>
        bind(ma.Value).Map(y => project(ma.Value, y));
}
