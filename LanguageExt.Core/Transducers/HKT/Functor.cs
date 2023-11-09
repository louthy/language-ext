using System;
using LanguageExt.Transducers;

namespace LanguageExt.HKT;

/// <summary>
/// Functor trait
/// </summary>
/// <typeparam name="F">Functor type</typeparam>
public interface Functor<F> where F : Functor<F>
{
    /// <summary>
    /// Map from `A -> B` to `A -> C` 
    /// </summary>
    KArr<F, Unit, B> Map<A, B>(KArr<F, Unit, A> fab, Transducer<A, B> f);
}

/// <summary>
/// Functor trait with constrained input-type
/// </summary>
/// <typeparam name="F">Functor type</typeparam>
/// <typeparam name="A">Lower kind input type</typeparam>
public interface Functor<F, A> where F : Functor<F, A>
{
    /// <summary>
    /// Map from `A->B` to `A->C` 
    /// </summary>
    KArr<F, A, C> Map<B, C>(KArr<F, A, B> fab, Transducer<B, C> f);
}

public static class FunctorExtensions
{
    /// <summary>
    /// Map from `A -> B` to `A -> C` 
    /// </summary>
    public static KArr<F, Unit, B> Map<F, A, B>(this KArr<F, Unit, A> fab, Func<A, B> f) 
        where F : struct, Functor<F> =>
        default(F).Map(fab, Transducer.lift(f));
    
    /// <summary>
    /// Map from `A->B` to `A->C` 
    /// </summary>
    public static KArr<F, A, C> Map<F, A, B, C>(this KArr<F, A, B> fab, Func<B, C> f)
        where F : struct, Functor<F, A> =>
        default(F).Map(fab, Transducer.lift(f));
}
