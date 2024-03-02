using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

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
    public static EitherT<L, M, A> Lift(K<M, Either<L, A>> monad) =>
        new(monad);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> LiftIO(IO<A> monad) =>
        Lift(M.LiftIO(monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> LiftIO(IO<Either<L, A>> monad) =>
        Lift(M.LiftIO(monad));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Match
    //

    /// <summary>
    /// Invokes the Right or Left function depending on the state of the Either
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Left">Function to invoke if in a Left state</param>
    /// <param name="Right">Function to invoke if in a Right state</param>
    /// <returns>The return value of the invoked function</returns>
    [Pure]
    public K<M, B> Match<B>(Func<L, B> Left, Func<A, B> Right) =>
        M.Map(mx => mx.Match(Left: Left, Right: Right), runEither);

    /// <summary>
    /// Invokes the Right or Left action depending on the state of the Either
    /// </summary>
    /// <param name="Right">Action to invoke if in a Right state</param>
    /// <param name="Left">Action to invoke if in a Left state</param>
    /// <returns>Unit</returns>
    [Pure]
    public K<M, Unit> Match(Action<L> Left, Action<A> Right) =>
        M.Map(mx => mx.Match(Left: Left, Right: Right), runEither);
 
    /// <summary>
    /// Executes the Left function if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="Left">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public K<M, A> IfLeft(Func<A> Left) =>
        IfLeft(_ => Left());

    /// <summary>
    /// Executes the leftMap function if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public K<M, A> IfLeft(Func<L, A> leftMap) =>
        Match(Left: leftMap, Right: identity);

    /// <summary>
    /// Returns the rightValue if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="rightValue">Value to return if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public K<M, A> IfLeft(A rightValue) =>
        IfLeft(_ => rightValue);

    /// <summary>
    /// Executes the Left action if the Either is in a Left state.
    /// </summary>
    /// <param name="Left">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    public K<M, Unit> IfLeft(Action<L> Left) =>
        Match(Left: Left, Right: _ => {});

    /// <summary>
    /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
    /// </summary>
    /// <param name="Right">Action to invoke</param>
    /// <returns>Unit</returns>
    public K<M, Unit> IfRight(Action<A> Right) =>
        Match(Left: _ => { }, Right: Right);

    /// <summary>
    /// Returns the leftValue if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="leftValue">Value to return if in the Left state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public K<M, L> IfRight(L leftValue) =>
        Match(Left: identity, Right: _ => leftValue);

    /// <summary>
    /// Returns the result of Right() if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="Right">Function to generate a Left value if in the Right state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public K<M, L> IfRight(Func<L> Right) =>
        Match(Left: identity, Right: _ => Right());

    /// <summary>
    /// Returns the result of rightMap if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="rightMap">Function to generate a Left value if in the Right state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public K<M, L> IfRight(Func<A, L> rightMap) =>
        Match(Left: identity, Right: rightMap);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run
    //

    /// <summary>
    /// Runs the EitherT exposing the outer monad with an inner wrapped `Either`
    /// </summary>
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
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    public EitherT<L, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new(runEither
               .Bind(fv => fv switch
                           {
                               Either.Right<L, A> (var v) => f(M.Pure(v)).Map(Either<L, B>.Right),
                               Either.Left<L, A> (var e)  => M.Pure<Either<L, B>>(e),
                               _                          => throw new NotSupportedException()
                           }));
    
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
    public EitherT<L, M, B> Bind<B>(Func<A, Either<L, B>> f) =>
        Bind(x => EitherT<L, M, B>.Lift(f(x)));

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

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Bind<B>(Func<A, Fail<L>> f) =>
        Bind(a => EitherT<L, M, B>.Lift(f(a).Value));

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

    public static implicit operator EitherT<L, M, A>(Either<L, A> ma) =>
        Lift(ma);
    
    public static implicit operator EitherT<L, M, A>(Pure<A> ma) =>
        Right(ma.Value);
    
    public static implicit operator EitherT<L, M, A>(Fail<L> ma) =>
        Lift(Either<L, A>.Left(ma.Value));
    
    public static implicit operator EitherT<L, M, A>(IO<A> ma) =>
        LiftIO(ma);
    
    public static implicit operator EitherT<L, M, A>(Lift<A> ma) =>
        LiftIO(ma);
    
    public static implicit operator EitherT<L, M, A>(Lift<EnvIO, A> ma) =>
        LiftIO(ma);
    
    public static implicit operator EitherT<L, M, A>(IO<Either<L, A>> ma) =>
        LiftIO(ma);

    public OptionT<M, A> ToOption() =>
        new(runEither.Map(ma => ma.ToOption()));
}
