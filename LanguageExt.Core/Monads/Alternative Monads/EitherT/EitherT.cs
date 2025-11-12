using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `EitherT` monad transformer, which allows for either an `L` or `R` result value to be carried. 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="L">Left value type</typeparam>
/// <typeparam name="R">Bound value type</typeparam>
public record EitherT<L, M, R>(K<M, Either<L, R>> runEither) : 
    Fallible<EitherT<L, M, R>, EitherT<L, M>, L, R>,
    K<EitherT<M>, L, R>
    where M : Monad<M>
{
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
                               Either<L, R>.Right (var v) => f(M.Pure(v)).Map(Either.Right<L, B>),
                               Either<L, R>.Left (var e)  => M.Pure<Either<L, B>>(e),
                               _                          => throw new NotSupportedException()
                           }));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Mapped structure</returns>
    public EitherT<L, M, B> Map<B>(Func<R, B> f) =>
        new(M.Map(mx => mx.Map(f), runEither));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Mapped structure</returns>
    public EitherT<L, M, B> Select<B>(Func<R, B> f) =>
        new(M.Map(mx => mx.Map(f), runEither));
    
    /// <summary>
    /// Maps the left value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Mapped structure</returns>
    public EitherT<B, M, R> MapLeft<B>(Func<L, B> f) =>
        new(M.Map(mx => mx.MapLeft(f), runEither));

    /// <summary>
    /// Bifunctor map operation
    /// </summary>
    /// <param name="Left">Left map function</param>
    /// <param name="Right">Right map function</param>
    /// <typeparam name="L1">Target left type</typeparam>
    /// <typeparam name="R1">Target right type</typeparam>
    /// <returns>Mapped structure</returns>
    public EitherT<L1, M, R1> BiMap<L1, R1>(
        Func<L, L1> Left, 
        Func<R, R1> Right) => 
        Bifunctor.bimap(Left, Right, this).As2();

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
        Bind(x => EitherT.lift<L, M, B>(f(x)));

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
                       Left: e => M.Pure(Either.Left<L, B>(e)))));

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
        Bind(a => EitherT.lift<L, M, B>(f(a).Value));

    /// <summary>
    /// Bimonad bind left
    /// </summary>
    /// <param name="f"></param>
    /// <typeparam name="L1"></typeparam>
    /// <returns></returns>
    public EitherT<L1, M, R> BindLeft<L1>(Func<L, K<EitherT<M>, L1, R>> f) =>
        Bimonad.bindFirst(this, f).As2();

    /// <summary>
    /// Bimonad bind right
    /// </summary>
    /// <param name="f"></param>
    /// <typeparam name="L1"></typeparam>
    /// <returns></returns>
    public EitherT<L, M, R1> BindRight<R1>(Func<R, K<EitherT<M>, L, R1>> f) =>
        Bimonad.bindSecond(this, f).As2();

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
        SelectMany(x => EitherT.lift<L, M, B>(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<B, C>(Func<R, Either<L, B>> bind, Func<R, B, C> project) =>
        SelectMany(x => EitherT.lift<L, M, B>(bind(x)), project);

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
        SelectMany(x => M.LiftIOMaybe(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public EitherT<L, M, C> SelectMany<C>(Func<R, Guard<L, Unit>> bind, Func<R, Unit, C> project) =>
        SelectMany(x => bind(x).ToEither(), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //

    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static EitherT<L, M, R> operator >> (EitherT<L, M, R> lhs, EitherT<L, M, R> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static EitherT<L, M, R> operator >> (EitherT<L, M, R> lhs, K<EitherT<L, M>, R> rhs) =>
        lhs.Bind(_ => rhs);

    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static EitherT<L, M, R> operator >> (EitherT<L, M, R> lhs, EitherT<L, M, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static EitherT<L, M, R> operator >> (EitherT<L, M, R> lhs, K<EitherT<L, M>, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    public static implicit operator EitherT<L, M, R>(Either<L, R> ma) =>
        EitherT.lift<L, M, R>(ma);
    
    public static implicit operator EitherT<L, M, R>(Pure<R> ma) =>
        EitherT.Right<L, M, R>(ma.Value);

    public static implicit operator EitherT<L, M, R>(Fail<L> ma) =>
        EitherT.lift<L, M, R>(ma);

    public static implicit operator EitherT<L, M, R>(L fail) =>
        EitherT.Left<L, M, R>(fail);

    public static implicit operator EitherT<L, M, R>(IO<R> ma) =>
        EitherT.liftIOMaybe<L, M, R>(ma);
    
    public static implicit operator EitherT<L, M, R>(Lift<R> ma) =>
        EitherT.liftIOMaybe<L, M, R>(ma.ToIO());
    
    public static implicit operator EitherT<L, M, R>(Lift<EnvIO, R> ma) =>
        EitherT.liftIOMaybe<L, M, R>(ma.ToIO());
    
    public static implicit operator EitherT<L, M, R>(IO<Either<L, R>> ma) =>
        EitherT.liftIOMaybe<L, M, R>(ma);

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> lhs, EitherT<L, M, R> rhs) =>
        lhs.Choose(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(K<EitherT<L, M>, R> lhs, EitherT<L, M, R> rhs) =>
        lhs.As().Choose(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> lhs, K<EitherT<L, M>, R> rhs) =>
        lhs.Choose(rhs.As()).As();

    public static EitherT<L, M, R> operator |(EitherT<L, M, R> ma, R b) => 
        ma.Choose(pure<EitherT<L, M>, R>(b)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> ma, Pure<R> mb) =>
        ma.Choose(pure<EitherT<L, M>, R>(mb.Value)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> ma, Fail<L> mb) =>
        ma.Choose(fail<L, EitherT<L, M>, R>(mb.Value)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> ma, L mb) =>
        ma.Choose(fail<L, EitherT<L, M>, R>(mb)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static EitherT<L, M, R> operator |(EitherT<L, M, R> ma, CatchM<L, EitherT<L, M>, R> mb) =>
        (ma.Kind() | mb).As();

    public OptionT<M, R> ToOption() =>
        new(runEither.Map(ma => ma.ToOption()));

    /*
    public StreamT<M, R> ToStream() =>
        from seq in StreamT<M, Seq<R>>.Lift(runEither.Map(ma => ma.IsRight ? Seq((R)ma) : Seq<R>.Empty))
        from res in StreamT<M, R>.Lift(seq)
        select res;

    public StreamT<M, L> LeftToStream() =>
        from seq in StreamT<M, Seq<L>>.Lift(runEither.Map(ma => ma.IsLeft ? Seq((L)ma) : Seq<L>.Empty))
        from res in StreamT<M, L>.Lift(seq)
        select res;
        */
    
}
