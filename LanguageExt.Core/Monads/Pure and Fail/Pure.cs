#nullable enable
using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

/// <summary>
/// Represents a pure value.  Usually understood to be the 'success' value.
/// </summary>
/// <remarks>
/// On its own this doesn't do much, but it allows other monads to convert
/// from it and provide binding extensions that mean this will work in
/// binding scenarios.
///
/// It simplifies certain scenarios where additional generic arguments are
/// needed.  This only requires the generic argument of the value which
/// means the C# inference system can work.
/// </remarks>
/// <param name="Value">Bound value</param>
/// <typeparam name="A">Bound value type</typeparam>
public readonly record struct Pure<A>(A Value)
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Standard operators
    //

    /// <summary>
    /// Functor map
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Result bound value type</typeparam>
    /// <returns>Result of the applying the mapping function to the `Pure` value</returns>
    public Pure<B> Map<B>(Func<A, B> f) =>
        new (f(Value));

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <typeparam name="B">Result bound value type</typeparam>
    /// <returns>Result of the applying the bind function to the `Pure` value</returns>
    public Pure<B> Bind<B>(Func<A, Pure<B>> f) =>
        f(Value);

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    /// <typeparam name="B">Result of the bind operation bound value type</typeparam>
    /// <typeparam name="C">Result of the mapping operation bound value type</typeparam>
    /// <returns>Result of the applying the bind and mapping function to the `Pure` value</returns>
    public Pure<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        new(project(Value, bind(Value).Value));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //
    
    public Option<A> ToOption() =>
        Value is null 
            ? Option<A>.None 
            : Option<A>.Some(Value);
    
    public OptionUnsafe<A> ToOptionUnsafe() =>
        OptionUnsafe<A>.Some(Value);
    
    public Either<L, A> ToEither<L>() =>
        Either<L, A>.Right(Value);
    
    public EitherUnsafe<L, A> ToEitherUnsafe<L>() =>
        EitherUnsafe<L, A>.Right(Value);
    
    public Fin<A> ToFin() =>
        Fin<A>.Succ(Value);
    
    public Validation<FAIL, A> ToValidation<FAIL>() =>
        Validation<FAIL, A>.Success(Value);
    
    public Validation<MonadFail, FAIL, A> ToValidation<MonadFail, FAIL>() 
        where MonadFail : struct, Monoid<FAIL>, Eq<FAIL> =>
        Validation<MonadFail, FAIL, A>.Success(Value);
    
    public IO<RT, E, A> ToIO<RT, E>()
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Pure(Value);
    
    public Eff<RT, A> ToEff<RT>()
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Success(Value);
    
    public Eff<A> ToEff() =>
        Eff<A>.Success(Value);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding
    //

    public Option<B> Bind<B>(Func<A, Option<B>> bind) =>
        bind(Value);

    public OptionUnsafe<B> Bind<B>(Func<A, OptionUnsafe<B>> bind) =>
        bind(Value);

    public Either<L, B> Bind<L, B>(Func<A, Either<L, B>> bind) =>
        bind(Value);

    public EitherUnsafe<L, B> Bind<L, B>(Func<A, EitherUnsafe<L, B>> bind) =>
        bind(Value);

    public Fin<B> Bind<B>(Func<A, Fin<B>> bind) =>
        bind(Value);

    public Validation<FAIL, B> Bind<FAIL, B>(Func<A, Validation<FAIL, B>> bind) =>
        bind(Value);

    public Validation<MonoidFail, FAIL, B> Bind<MonoidFail, FAIL, B>(Func<A, Validation<MonoidFail, FAIL, B>> bind)
        where MonoidFail : struct, Eq<FAIL>, Monoid<FAIL> =>
        bind(Value);

    public IO<RT, E, B> Bind<RT, E, B>(Func<A, IO<RT, E, B>> bind)
        where RT : struct, HasIO<RT, E> =>
        bind(Value);

    public Eff<RT, B> Bind<RT, B>(Func<A, Eff<RT, B>> bind)
        where RT : struct, HasIO<RT, Error> =>
        bind(Value);

    public Eff<B> Bind<B>(Func<A, Eff<B>> bind) =>
        bind(Value);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding and projection
    //

    public Option<C> SelectMany<B, C>(Func<A, Option<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Either<L, C> SelectMany<L, B, C>(Func<A, Either<L, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Fin<C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Validation<FAIL, C> SelectMany<FAIL, B, C>(Func<A, Validation<FAIL, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Validation<MonoidFail, FAIL, C> SelectMany<MonoidFail, FAIL, B, C>(
        Func<A, Validation<MonoidFail, FAIL, B>> bind, 
        Func<A, B, C> project)
        where MonoidFail : struct, Eq<FAIL>, Monoid<FAIL> =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public IO<RT, E, C> SelectMany<RT, E, B, C>(Func<A, IO<RT, E, B>> bind, Func<A, B, C> project)
        where RT : struct, HasIO<RT, E> =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Eff<RT, C> SelectMany<RT, B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project)
        where RT : struct, HasIO<RT, Error> =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Eff<C> SelectMany<B, C>(Func<A, Eff<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
}

public static class PureExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    /// <param name="mma">Nested `Pure` monad</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Flattened monad</returns>
    public static Pure<A> Flatten<A>(this Pure<Pure<A>> mma) =>
        mma.Value;
}
