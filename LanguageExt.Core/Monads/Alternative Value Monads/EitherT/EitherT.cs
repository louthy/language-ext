using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `EitherT` monad transformer, which allows for either an `L` or `R` result value to be carried. 
/// </summary>
/// <param name="runEither">Transducer that represents the transformer operation</param>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="L">Left value type</typeparam>
/// <typeparam name="R">Bound value type</typeparam>
public record EitherT<L, M, R>(K<M, Either<L, R>> runEither) : 
    Fallible<EitherT<L, M, R>, EitherT<L, M>, L, R>
    where M : Monad<M>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, R> Right(R value) =>
        Lift(M.Pure(value));
    
    /// <summary>
    /// Lift a fail value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, R> Left(L value) =>
        Lift(Either<L, R>.Left(value));

    /// <summary>
    /// Is the `EitherT` in a Right state?
    /// </summary>
    public K<M, bool> IsRight =>
        Match(Left: _ => false, Right: _ => true);

    /// <summary>
    /// Is the `EitherT` in a Left state?
    /// </summary>
    public K<M, bool> IsLeft =>
        Match(Left: _ => true, Right: _ => false);
    
    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="pure">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, R> Lift(Pure<R> pure) =>
        Right(pure.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="either">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, R> Lift(Either<L, R> either) =>
        new(M.Pure(either));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="fail">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, R> Lift(Fail<L> fail) =>
        Lift(Either<L, R>.Left(fail.Value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, R> Lift(K<M, R> monad) =>
        new(M.Map(Either<L, R>.Right, monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, R> Lift(K<M, Either<L, R>> monad) =>
        new(monad);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, R> LiftIO(IO<R> monad) =>
        Lift(M.LiftIO(monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, R> LiftIO(IO<Either<L, R>> monad) =>
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
    public K<M, B> Match<B>(Func<L, B> Left, Func<R, B> Right) =>
        M.Map(mx => mx.Match(Left: Left, Right: Right), runEither);

    /// <summary>
    /// Invokes the Right or Left action depending on the state of the Either
    /// </summary>
    /// <param name="Right">Action to invoke if in a Right state</param>
    /// <param name="Left">Action to invoke if in a Left state</param>
    /// <returns>Unit</returns>
    [Pure]
    public K<M, Unit> Match(Action<L> Left, Action<R> Right) =>
        M.Map(mx => mx.Match(Left: Left, Right: Right), runEither);
 
    /// <summary>
    /// Executes the Left function if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="Left">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public K<M, R> IfLeft(Func<R> Left) =>
        IfLeft(_ => Left());

    /// <summary>
    /// Executes the leftMap function if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public K<M, R> IfLeft(Func<L, R> leftMap) =>
        Match(Left: leftMap, Right: identity);

    /// <summary>
    /// Returns the rightValue if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="rightValue">Value to return if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public K<M, R> IfLeft(R rightValue) =>
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
    public K<M, Unit> IfRight(Action<R> Right) =>
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
    public K<M, L> IfRight(Func<R, L> rightMap) =>
        Match(Left: identity, Right: rightMap);
 
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
    public EitherT<L, M1, B> MapT<M1, B>(Func<K<M, Either<L, R>>, K<M1, Either<L, B>>> f)
        where M1 : Monad<M1> =>
        new (f(runEither));

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    public EitherT<L, M, B> MapM<B>(Func<K<M, R>, K<M, B>> f) =>
        new(runEither
               .Bind(fv => fv switch
                           {
                               Either.Right<L, R> (var v) => f(M.Pure(v)).Map(Either<L, B>.Right),
                               Either.Left<L, R> (var e)  => M.Pure<Either<L, B>>(e),
                               _                          => throw new NotSupportedException()
                           }));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Map<B>(Func<R, B> f) =>
        new(M.Map(mx => mx.Map(f), runEither));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Select<B>(Func<R, B> f) =>
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
    public EitherT<L, M, B> Bind<B>(Func<R, K<EitherT<L, M>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Bind<B>(Func<R, Either<L, B>> f) =>
        Bind(x => EitherT<L, M, B>.Lift(f(x)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Bind<B>(Func<R, EitherT<L, M, B>> f) =>
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
    public EitherT<L, M, B> Bind<B>(Func<R, IO<B>> f) =>
        Bind(a => EitherT<L, M, B>.LiftIO(f(a)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Bind<B>(Func<R, Pure<B>> f) =>
        Map(a => f(a).Value);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> Bind<B>(Func<R, Fail<L>> f) =>
        Bind(a => EitherT<L, M, B>.Lift(f(a).Value));

    /// <summary>
    /// Monad bi-bind operation
    /// </summary>
    /// <param name="Left">Left state mapping function</param>
    /// <param name="Right">Left state mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, B> BiBind<B>(Func<L, EitherT<L, M, B>> Left, Func<R, EitherT<L, M, B>> Right) =>
        new(M.Bind(runEither, 
                   ex => ex.Match(
                       Right: x => Right(x).runEither,
                       Left: e => Left(e).runEither)));

    /// <summary>
    /// Monad bi-bind operation
    /// </summary>
    /// <param name="Left">Left state mapping function</param>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, R> BindLeft(Func<L, EitherT<L, M, R>> Left) =>
        BiBind(Left, Right);

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
    public EitherT<L, M, C> SelectMany<B, C>(Func<R, K<EitherT<L, M>, B>> bind, Func<R, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<R, EitherT<L, M, B>> bind, Func<R, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<R, K<M, B>> bind, Func<R, B, C> project) =>
        SelectMany(x => EitherT<L, M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<R, Either<L, B>> bind, Func<R, B, C> project) =>
        SelectMany(x => EitherT<L, M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<R, Pure<B>> bind, Func<R, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<R, IO<B>> bind, Func<R, B, C> project) =>
        SelectMany(x => M.LiftIO(bind(x)), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //

    public static EitherT<L, M, R> operator >> (EitherT<L, M, R> lhs, EitherT<L, M, R> rhs) =>
        lhs.Bind(_ => rhs);
    
    public static EitherT<L, M, R> operator >> (EitherT<L, M, R> lhs, K<EitherT<L, M>, R> rhs) =>
        lhs.Bind(_ => rhs);
    
    public static implicit operator EitherT<L, M, R>(Either<L, R> ma) =>
        Lift(ma);
    
    public static implicit operator EitherT<L, M, R>(Pure<R> ma) =>
        Right(ma.Value);
    
    public static implicit operator EitherT<L, M, R>(Fail<L> ma) =>
        Lift(Either<L, R>.Left(ma.Value));

    public static implicit operator EitherT<L, M, R>(L fail) => 
        Lift(Either<L, R>.Left(fail));

    public static implicit operator EitherT<L, M, R>(IO<R> ma) =>
        LiftIO(ma);
    
    public static implicit operator EitherT<L, M, R>(Lift<R> ma) =>
        LiftIO(ma);
    
    public static implicit operator EitherT<L, M, R>(Lift<EnvIO, R> ma) =>
        LiftIO(ma);
    
    public static implicit operator EitherT<L, M, R>(IO<Either<L, R>> ma) =>
        LiftIO(ma);

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> lhs, EitherT<L, M, R> rhs) =>
        lhs.Combine(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(K<EitherT<L, M>, R> lhs, EitherT<L, M, R> rhs) =>
        lhs.As().Combine(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> lhs, K<EitherT<L, M>, R> rhs) =>
        lhs.Combine(rhs.As()).As();

    public static EitherT<L, M, R> operator |(EitherT<L, M, R> ma, R b) => 
        ma.Combine(pure<EitherT<L, M>, R>(b)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> ma, Pure<R> mb) =>
        ma.Combine(pure<EitherT<L, M>, R>(mb.Value)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> ma, Fail<L> mb) =>
        ma.Combine(fail<L, EitherT<L, M>, R>(mb.Value)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> ma, L mb) =>
        ma.Combine(fail<L, EitherT<L, M>, R>(mb)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> ma, CatchM<L, EitherT<L, M>, R> mb) =>
        (ma.Kind() | mb).As();

    public OptionT<M, R> ToOption() =>
        new(runEither.Map(ma => ma.ToOption()));
}
