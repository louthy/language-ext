using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Used by various error producing monads to have a contextual `where`
/// </summary>
/// <remarks>
/// See `Prelude.guard(...)`
/// </remarks>
public readonly struct Guard<E, A>
{
    public readonly bool Flag;
    readonly Func<E> onFalse;

    internal Guard(bool flag, Func<E> onFalse) =>
        (Flag, this.onFalse) = (flag, onFalse ?? throw new ArgumentNullException(nameof(onFalse)));

    internal Guard(bool flag, E onFalse)
    {
        if (isnull(onFalse)) throw new ArgumentNullException(nameof(onFalse));
        (Flag, this.onFalse) = (flag, () => onFalse);
    }

    public Guard<E, B> Cast<B>() =>
        new (Flag, OnFalse);
        
    public Func<E> OnFalse =>
        onFalse ?? throw new InvalidOperationException(
            "Guard isn't initialised. It was probably created via new Guard() or default(Guard), and so it has no OnFalse handler");

    public Guard<E, C> SelectMany<C>(Func<E, Guard<E, Unit>> bind, Func<Unit, Unit, C> project) =>
        Flag ? bind(default!).Cast<C>() : Cast<C>();

    public Guard<E, B> Select<B>(Func<B, B> _) =>
        Cast<B>();
        
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Natural transformations for the types supporting guards
    // 

    public Transducer<Unit, Sum<E, Unit>> ToTransducer() =>
        Transducer.compose(
            Transducer.constant<Unit, Guard<E, A>>(this),
            Transducer.guard<E, A>());
        
    /// <summary>
    /// Natural transformation to `Either`
    /// </summary>
    public Either<E, Unit> ToEither() =>
        Flag
            ? Right(unit)
            : Left(OnFalse());
        
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind implementations for the types supporting guards
    // 

    /// <summary>
    /// Monadic binding support for `Either`
    /// </summary>
    public Either<E, B> Bind<B>(Func<Unit, Either<E, B>> f) =>
        Flag
            ? f(default)
            : Left(OnFalse());

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany implementations for the types supporting guards
    // 
        
    /// <summary>
    /// Monadic binding `SelectMany` extension for `Guard`
    /// </summary>
    public Either<E, C> SelectMany<B, C>(
        Func<Unit, Either<E, B>> bind, Func<Unit, B, C> project) =>
        Flag
            ? bind(default).Map(b => project(default, b))
            : Left(OnFalse());
}

public static class GuardExtensions
{
    /// <summary>
    /// Monadic binding support for `Eff`
    /// </summary>
    public static IO<B> Bind<B>(
        this Guard<Error, Unit> guard,
        Func<Unit, IO<B>> f) =>
        guard.Flag 
            ? f(unit) 
            : Fail(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Eff`
    /// </summary>
    public static IO<C> SelectMany<B, C>(
        this Guard<Error, Unit> guard,
        Func<Unit, IO<B>> bind, 
        Func<Unit, B, C> project) =>
        guard.Flag
            ? bind(default).Map(b => project(default, b))
            : Fail(guard.OnFalse());    
    
    /// <summary>
    /// Natural transformation to `IO`
    /// </summary>
    public static IO<Unit> ToIO(this Guard<Error, Unit> guard) =>
        IO.lift(() => guard.Flag
                          ? unit
                          : guard.OnFalse().Throw());
    
    /// <summary>
    /// Natural transformation to `Eff`
    /// </summary>
    public static Eff<Unit> ToEff(this Guard<Error, Unit> guard) =>
        guard.Flag
            ? Pure(unit)
            : Fail(guard.OnFalse());

    /// <summary>
    /// Monadic binding support for `Eff`
    /// </summary>
    public static Eff<B> Bind<B>(
        this Guard<Error, Unit> guard,
        Func<Unit, Eff<B>> f) =>
        guard.Flag
            ? f(default)
            : Fail(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Eff`
    /// </summary>
    public static Eff<C> SelectMany<B, C>(
        this Guard<Error, Unit> guard,
        Func<Unit, Eff<B>> bind, 
        Func<Unit, B, C> project) =>
        guard.Flag
            ? bind(default).Map(b => project(default, b))
            : Fail(guard.OnFalse());    
    
    /// <summary>
    /// Natural transformation to `Eff`
    /// </summary>
    public static Eff<RT, Unit> ToEff<RT>(this Guard<Error, Unit> guard) =>
        guard.Flag
            ? Pure(unit)
            : Fail(guard.OnFalse());

    /// <summary>
    /// Monadic binding support for `Eff`
    /// </summary>
    public static Eff<RT, B> Bind<RT, B>(
        this Guard<Error, Unit> guard,
        Func<Unit, Eff<RT, B>> f) =>
        guard.Flag
            ? f(default)
            : Fail(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Eff`
    /// </summary>
    public static Eff<RT, C> SelectMany<RT, B, C>(
        this Guard<Error, Unit> guard,
        Func<Unit, Eff<RT, B>> bind, 
        Func<Unit, B, C> project) =>
        guard.Flag
            ? bind(default).Map(b => project(default, b))
            : Fail(guard.OnFalse());    
    
    /// <summary>
    /// Natural transformation to `Validation`
    /// </summary>
    public static Validation<F, Unit> ToValidation<F>(this Guard<F, Unit> guard) 
        where F : Monoid<F> =>
        guard.Flag
            ? Validation<F, Unit>.Success(default)
            : Validation<F, Unit>.Fail(guard.OnFalse());
 
    /// <summary>
    /// Monadic binding support for `Validation`
    /// </summary>
    public static Validation<F, B> Bind<F, B>(
        this Guard<F, Unit> guard,
        Func<Unit, Validation<F, B>> f) 
        where F : Monoid<F> =>
        guard.Flag
            ? f(default)
            : Validation<F, B>.Fail(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `Validation`
    /// </summary>
    public static Validation<F, C> SelectMany<F, B, C>(
        this Guard<F, Unit> guard,
        Func<Unit, Validation<F, B>> bind, 
        Func<Unit, B, C> project) 
        where F : Monoid<F> =>
        guard.Flag
            ? bind(default).Map(b => project(default, b))
            : Validation<F, C>.Fail(guard.OnFalse());

}
