using System;

namespace LanguageExt.HKT;

/// <summary>
/// Applicative trait
/// </summary>
public interface Apply<F> : Functor<F> 
    where F : Apply<F>
{
    /// <summary>
    /// Applicative apply
    /// </summary>
    KArr<F, Unit, B> Ap<A, B>(KArr<F, Unit, Func<A, B>> f, KArr<F, Unit, A> x);
}

/// <summary>
/// Applicative trait with fixed input type
/// </summary>
public interface Apply<F, A> : Functor<F, A> 
    where F : Apply<F, A>
{
    /// <summary>
    /// Applicative apply
    /// </summary>
    KArr<F, A, C> Ap<B, C>(KArr<F, A, Func<B, C>> f, KArr<F, A, B> x);
}
