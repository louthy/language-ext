using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `OptionT` monad transformer, which allows for an optional result. 
/// </summary>
/// <param name="runOption">Transducer that represents the transformer operation</param>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record OptionT<M, A>(K<M, Option<A>> runOption) : K<OptionT<M>, A>
    where M : Monad<M>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> Some(A value) =>
        Lift(M.Pure(value));
    
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`OptionT`</returns>
    public static readonly OptionT<M, A> None =
        Lift(Option<A>.None);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> Lift(Pure<A> monad) =>
        Some(monad.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> Lift(Option<A> monad) =>
        new(M.Pure(monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> Lift(Fail<Unit> monad) =>
        Lift(Option<A>.None);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> Lift(K<M, A> monad) =>
        new(M.Map(Option<A>.Some, monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> Lift(K<M, Option<A>> monad) =>
        new(monad);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> LiftIO(IO<A> monad) =>
        Lift(M.LiftIO(monad));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Match
    //

    /// <summary>
    /// Match the two states of the Option and return a B, which can be null.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. May return null.</param>
    /// <param name="None">None match operation. May return null.</param>
    /// <returns>B, or null</returns>
    public K<M, B> Match<B>(Func<A, B> Some, Func<B> None) =>
        M.Map(mx => mx.Match(Some, None), runOption);

    /// <summary>
    /// Match the two states of the Option
    /// </summary>
    /// <param name="Some">Some match operation</param>
    /// <param name="None">None match operation</param>
    public K<M, Unit> Match(Action<A> Some, Action None) =>
        M.Map(mx => mx.Match(Some, None), runOption);

    /// <summary>
    /// Invokes the action if Option is in the Some state, otherwise nothing happens.
    /// </summary>
    /// <param name="f">Action to invoke if Option is in the Some state</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public K<M, Unit> IfSome(Action<A> f) =>
        M.Map(mx => mx.IfSome(f), runOption);

    /// <summary>
    /// Invokes the f function if Option is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    /// <param name="f">Function to invoke if Option is in the Some state</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public K<M, Unit> IfSome(Func<A, Unit> f) =>
        M.Map(mx => mx.IfSome(f), runOption);

    /// <summary>
    /// Returns the result of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will not accept a null return value from the None operation</remarks>
    /// <param name="None">Operation to invoke if the structure is in a None state</param>
    /// <returns>Result of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public K<M, A> IfNone(Func<A> None) =>
        M.Map(mx => mx.IfNone(None), runOption);

    /// <summary>
    /// Invokes the action if Option is in the None state, otherwise nothing happens.
    /// </summary>
    /// <param name="f">Action to invoke if Option is in the None state</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public K<M, Unit> IfNone(Action None) =>
        M.Map(mx => mx.IfNone(None), runOption);
        
    /// <summary>
    /// Returns the noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will not accept a null noneValue</remarks>
    /// <param name="noneValue">Value to return if in a None state</param>
    /// <returns>noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public K<M, A> IfNone(A noneValue) =>
        M.Map(mx => mx.IfNone(noneValue), runOption);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run
    //

    /// <summary>
    /// Runs the OptionT exposing the outer monad with an inner wrapped `Option`
    /// </summary>
    public K<M, Option<A>> Run() =>
        runOption;
 
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
    public OptionT<M1, B> MapT<M1, B>(Func<K<M, Option<A>>, K<M1, Option<B>>> f)
        where M1 : Monad<M1> =>
        new (f(runOption));

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, B> Map<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runOption));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, B> Select<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runOption));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, B> Bind<B>(Func<A, K<OptionT<M>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, B> Bind<B>(Func<A, OptionT<M, B>> f) =>
        new(M.Bind(runOption, 
                   ox => ox.Match(
                       Some: x => f(x).runOption,
                       None: () => M.Pure(Option<B>.None))));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(a => OptionT<M, B>.LiftIO(f(a)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, B> Bind<B>(Func<A, Pure<B>> f) =>
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
    /// <returns>`OptionT`</returns>
    public OptionT<M, C> SelectMany<B, C>(Func<A, K<OptionT<M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, C> SelectMany<B, C>(Func<A, OptionT<M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => OptionT<M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, C> SelectMany<B, C>(Func<A, Option<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => OptionT<M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public OptionT<M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => M.LiftIO(bind(x)), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //
    
    public static implicit operator OptionT<M, A>(Option<A> ma) =>
        Lift(ma);
    
    public static implicit operator OptionT<M, A>(Pure<A> ma) =>
        Some(ma.Value);
    
    public static implicit operator OptionT<M, A>(Fail<Unit> ma) =>
        Lift(Option<A>.None);
    
    public static implicit operator OptionT<M, A>(IO<A> ma) =>
        LiftIO(ma);
    
    public static implicit operator OptionT<M, A>(Lift<A> ma) =>
        LiftIO(ma);
    
    public static implicit operator OptionT<M, A>(Lift<EnvIO, A> ma) =>
        LiftIO(ma);

    public EitherT<L, M, A> ToEither<L>(L left) =>
        new(runOption.Map(ma => ma.ToEither(left)));

    public EitherT<L, M, A> ToEither<L>(Func<L> left) =>
        new(runOption.Map(ma => ma.ToEither(left)));

    public EitherT<L, M, A> ToEither<L>() where L : Monoid<L> =>
        new(runOption.Map(ma => ma.ToEither<L>()));
}
