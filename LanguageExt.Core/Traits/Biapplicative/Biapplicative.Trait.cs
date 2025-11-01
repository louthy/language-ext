using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Applicative functor 
/// </summary>
/// <typeparam name="FF">Applicative functor type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface Biapplicative<FF> : Bifunctor<FF>
    where FF : Biapplicative<FF>
{
    public static abstract K<FF, C, D> BiApply<A, B, C, D>(
        K<FF, Func<A, C>, Func<B, D>> ff,
        K<FF, A, B> fab);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //

    public static virtual K<FF, Func<C, E>, Func<D, F>> BiApply<A, B, C, D, E, F>(
        K<FF, Func<A, C, E>, Func<B, D, F>> ff,
        K<FF, A, B> fab) =>
        FF.BiApply(ff.BiMap(curry, curry), fab);

    public static virtual K<FF, E, F> BiApply<A, B, C, D, E, F>(
        K<FF, Func<A, C, E>, Func<B, D, F>> ff,
        K<FF, A, B> fab,
        K<FF, C, D> fcd) =>
        FF.BiApply(FF.BiApply(ff, fab), fcd);
}
