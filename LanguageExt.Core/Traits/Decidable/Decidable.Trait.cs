using System;

namespace LanguageExt.Traits;

/// <summary>
/// A `Decidable` contravariant functor is the contravariant analogue of `Alternative`.
/// 
/// Noting the superclass constraint that `f` must also be `Divisible`, a `Decidable` functor has the ability to
/// "fan out" input, under the intuition that contravariant functors consume input.
/// </summary>
/// <typeparam name="F">Self-referring type</typeparam>
public interface Decidable<F> : Divisible<F>
{
    /// <summary>
    /// Acts as identity to 'Choose'.
    /// </summary>
    public static abstract K<F, A> Lose<A>(Func<A, Void> f);
    
    /// <summary>
    /// Fan out the input 
    /// </summary>
    public static abstract K<F, A> Route<A, B, C>(Func<A, Either<B, C>> f, K<F, B> fb, K<F, C> fc);
}
