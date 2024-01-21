#nullable enable
using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

/// <summary>
/// Applicative pure trait
/// </summary>
public interface Applicative<F> : Functor<F> 
    where F : Applicative<F>
{
    /// <summary>
    /// Lift transducer into applicative
    /// </summary>
    KArr<F, Unit, A> Lift<A>(Transducer<Unit, A> f);
}

/// <summary>
/// Applicative pure trait with fixed input type
/// </summary>
public interface Applicative<F, A> : Functor<F, A> 
    where F : Applicative<F, A>
{
    /// <summary>
    /// Lift transducer into applicative
    /// </summary>
    KArr<F, A, B> Lift<B>(Transducer<A, B> f);
}

public static class ApplicativeExtensions
{
    /// <summary>
    /// Lift transducer into applicative
    /// </summary>
    public static KArr<F, Unit, A> Lift<F, A>(this Applicative<F> self, Func<Unit, A> f) 
        where F : struct, Applicative<F> =>
        default(F).Lift(lift(f));

    /// <summary>
    /// Pure constructor
    /// </summary>
    public static KArr<F, Unit, B> Pure<F, B>(this Applicative<F> self, B value) 
        where F : struct, Applicative<F> =>
        default(F).Lift(Transducer.constant<Unit, B>(value));
    
    /// <summary>
    /// Lift transducer into applicative
    /// </summary>
    public static KArr<F, A, B> Lift<F, A, B>(this Applicative<F, A> self, Func<A, B> f)
        where F : struct, Applicative<F, A> =>
        default(F).Lift(lift(f));

    /// <summary>
    /// Pure constructor
    /// </summary>
    public static KArr<F, A, B> Pure<F, A, B>(this Applicative<F, A> self, B value)
        where F : struct, Applicative<F, A> =>
        default(F).Lift(Transducer.constant<A, B>(value));

}
