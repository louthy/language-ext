using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// A `Divisible` contravariant functor is the contravariant analogue of `Applicative`.
///
/// Continuing the intuition that 'Contravariant' functors (`Cofunctor`) consume input, a 'Divisible'
/// contravariant functor also has the ability to be composed "beside" another contravariant
/// functor.
/// </summary>
/// <typeparam name="F">Self referring type</typeparam>
public static class Decidable
{
    /// <summary>
    /// Acts as identity to 'Choose'.
    /// </summary>
    public static K<F, A> lose<F, A>(Func<A, Void> f)
        where F : Decidable<F> =>
        F.Lose(f);

    /// <summary>
    /// Acts as identity to 'Choose'.
    /// </summary>
    /// <remarks>
    ///     lost = lose(identity)
    /// </remarks>
    public static K<F, Void> lost<F>()
        where F : Decidable<F> =>
        lose<F, Void>(identity);

    /// <summary>
    /// Fan out the input 
    /// </summary>
    public static K<F, A> route<F, A, B, C>(Func<A, Either<B, C>> f, K<F, B> fb, K<F, C> fc)
        where F : Decidable<F> =>
        F.Route(f, fb, fc);

    /// <summary>
    /// Fan out the input 
    /// </summary>
    /// <remarks>
    ///     route(fb, fc) = route(id, fb, fc)
    /// </remarks>
    public static K<F, Either<A, B>> route<F, A, B>(K<F, A> fa, K<F, B> fb)
        where F : Decidable<F> =>
        route<F, Either<A, B>, A, B>(identity, fa, fb);
}
