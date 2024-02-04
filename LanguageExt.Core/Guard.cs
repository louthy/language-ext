using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;
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
        
    /// <summary>
    /// Natural transformation to `Validation`
    /// </summary>
    public Validation<E, Unit> ToValidation() =>
        Flag
            ? Success<E, Unit>(unit)
            : Fail<E, Unit>(OnFalse());
        
    /// <summary>
    /// Natural transformation to `Validation`
    /// </summary>
    public Validation<MonoidE, E, Unit> ToValidation<MonoidE>() 
        where MonoidE : Monoid<E>, Eq<E> =>
        Flag
            ? Success<MonoidE, E, Unit>(unit)
            : Fail<MonoidE, E, Unit>(OnFalse());
        
    /// <summary>
    /// Natural transformation to `IO`
    /// </summary>
    public IO<RT, E, Unit> ToIO<RT>() 
        where RT : HasIO<RT, E> =>
        Flag
            ? Pure(unit)
            : Fail(OnFalse());
        
    /// <summary>
    /// Natural transformation to `IO`
    /// </summary>
    public IO<E, Unit> ToIO() =>
        Flag
            ? Pure(unit)
            : Fail(OnFalse());
        
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

    /// <summary>
    /// Monadic binding support for `Validation`
    /// </summary>
    public Validation<E, B> Bind<B>(
        Func<Unit, Validation<E, B>> f) =>
        Flag
            ? f(default)
            : Fail<E, B>(OnFalse());
        
    /// <summary>
    /// Monadic binding support for `Validation`
    /// </summary>
    public Validation<MonoidE, E, B> Bind<MonoidE, B>(Func<Unit, Validation<MonoidE, E, B>> f)
        where MonoidE : Monoid<E>, Eq<E> =>
        Flag
            ? f(default)
            : Fail<MonoidE, E, B>(OnFalse());

    /// <summary>
    /// Monadic binding support for `IO`
    /// </summary>
    public IO<RT, E, B> Bind<RT, B>(
        Func<Unit, IO<RT, E, B>> bind,
        Func<Unit, B, B> project)
        where RT : HasIO<RT, E> =>
        Flag
            ? bind(default).Map(b => project(default, b))
            : Fail(OnFalse());
        
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
        
    /// <summary>
    /// Monadic binding support for `Validation`
    /// </summary>
    public Validation<E, C> SelectMany<B, C>(Func<Unit, Validation<E, B>> bind, Func<Unit, B, C> project) =>
        Flag
            ? bind(default).Map(b => project(default, b))
            : Fail<E, C>(OnFalse());
        
    /// <summary>
    /// Monadic binding support for `Validation`
    /// </summary>
    public Validation<MonoidE, E, C> SelectMany<MonoidE, B, C>(
        Func<Unit, Validation<MonoidE, E, B>> bind, 
        Func<Unit, B, C> project)
        where MonoidE : Monoid<E>, Eq<E> =>
        Flag
            ? bind(default).Map(b => project(default, b))
            : Fail<MonoidE, E, C>(OnFalse());
        
        
    /// <summary>
    /// Monadic binding `SelectMany` extension for `IO`
    /// </summary>
    public IO<RT, E, C> SelectMany<RT, B, C>(
        Func<Unit, IO<RT, E, B>> bind,
        Func<Unit, B, C> project)
        where RT : HasIO<RT, E> =>
        Flag
            ? bind(default).Map(b => project(default, b))
            : Fail(OnFalse());
}
    
/*
static class Test
{
    public static IO<RT, Error, Unit> Testing<RT>(bool f1, bool f2, bool f3)
        where RT : HasIO<RT, Error> =>
        from _1 in guard(f1, Error.New("failed on 1"))
        from _2 in guard(f2, Error.New("failed on 2"))
        from _3 in guard(f3, Error.New("failed on 2"))
        from _4 in guard(f1, Error.New("failed on 1"))
        from _5 in guard(f2, Error.New("failed on 2"))
        from _6 in guard(f3, Error.New("failed on 2"))
        from tx in lift(() => Console.ReadLine())
        select unit;
}
*/
