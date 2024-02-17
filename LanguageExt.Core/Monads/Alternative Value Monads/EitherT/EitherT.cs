using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `EitherT` monad transformer, which allows for an optional result. 
/// </summary>
/// <param name="runEither">Transducer that represents the transformer operation</param>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="L">Left value type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record EitherT<L, M, A>(K<M, Either<L, A>> runEither) : K<EitherT<L, M>, A>
    where M : Monad<M>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> Right(A value) =>
        Lift(M.Pure(value));
    
    /// <summary>
    /// Lift a fail value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> Left(L value) =>
        Lift(Either<L, A>.Left(value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="pure">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> Lift(Pure<A> pure) =>
        Right(pure.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="either">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> Lift(Either<L, A> either) =>
        new(M.Pure(either));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="fail">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> Lift(Fail<L> fail) =>
        Lift(Either<L, A>.Left(fail.Value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> Lift(K<M, A> monad) =>
        new(M.Map(Either<L, A>.Right, monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> LiftIO(IO<A> monad) =>
        Lift(M.LiftIO(monad));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Match
    //

    public K<M, B> Match<B>(Func<L, B> Left, Func<A, B> Right) =>
        M.Map(mx => mx.Match(Left: Left, Right: Right), runEither);
 
    public K<M, Either<L, A>> Run() =>
        runEither;
 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="M1">Target monad type</typeparam>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Mapped monad</returns>
    public EitherT<L, M1, B> MapT<M1, B>(Func<K<M, Either<L, A>>, K<M1, Either<L, B>>> f)
        where M1 : Monad<M1> =>
        new (f(runEither));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Map<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runEither));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Select<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runEither));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Bind<B>(Func<A, K<EitherT<L, M>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Bind<B>(Func<A, EitherT<L, M, B>> f) =>
        new(M.Bind(runEither, 
                   ex => ex.Match(
                       Right: x => f(x).runEither,
                       Left: e => M.Pure(Either<L, B>.Left(e)))));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(a => EitherT<L, M, B>.LiftIO(f(a)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(a => f(a).Value);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<A, K<EitherT<L, M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<A, EitherT<L, M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => EitherT<L, M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<A, Either<L, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => EitherT<L, M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => M.LiftIO(bind(x)), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //

    public static implicit operator EitherT<L, M, A>(Pure<A> ma) =>
        Right(ma.Value);
    
    public static implicit operator EitherT<L, M, A>(Fail<L> ma) =>
        Lift(Either<L, A>.Left(ma.Value));
    
    public static implicit operator EitherT<L, M, A>(IO<A> ma) =>
        LiftIO(ma);
}
