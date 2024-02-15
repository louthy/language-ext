using System;
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
    public static OptionT<M, A> LiftIO(IO<A> monad) =>
        Lift(M.LiftIO(monad));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Match
    //

    public K<M, B> Match<B>(Func<A, B> Some, Func<B> None) =>
        M.Map(mx => mx.Match(Some, None), runOption);
 
    public K<M, Option<A>> Run() =>
        runOption;
 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

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

    public static implicit operator OptionT<M, A>(Pure<A> ma) =>
        Some(ma.Value);
    
    public static implicit operator OptionT<M, A>(Fail<Unit> ma) =>
        Lift(Option<A>.None);
    
    public static implicit operator OptionT<M, A>(IO<A> ma) =>
        LiftIO(ma);
}
