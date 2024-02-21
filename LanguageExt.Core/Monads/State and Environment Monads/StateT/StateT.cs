using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `StateT` monad transformer, which adds a modifiable state to a given monad. 
/// </summary>
/// <param name="runState">Transducer that represents the transformer operation</param>
/// <typeparam name="S">State type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record StateT<S, M, A>(Func<S, K<M, (A Value, S State)>> runState) : K<StateT<S, M>, A>
    where M : Monad<M>, Alternative<M>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> Pure(A value) =>
        Lift(M.Pure(value));

    /// <summary>
    /// Alternative empty
    /// </summary>
    public static StateT<S, M, A> Empty =>
        Lift(M.Empty<A>());

    /// <summary>
    /// Extracts the state value, maps it, and then puts it back into
    /// the monadic state.
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, Unit> Modify(Func<S, S> f) =>
        new(state => M.Pure((unit, f(state))));

    /// <summary>
    /// Extracts the state value, maps it, and then puts it back into
    /// the monadic state.
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, Unit> ModifyM(Func<S, K<M, S>> f) =>
        new(state => f(state).Map(s => (unit, s)));

    /// <summary>
    /// Writes the value into the monadic state
    /// </summary>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, Unit> Put(S value) =>
        new(_ => M.Pure((unit, value)));

    /// <summary>
    /// Extracts the state value and returns it as the bound value
    /// </summary>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, S> Get =>
        new(state => M.Pure((state, state)));

    /// <summary>
    /// Extracts the state value and maps it to the bound value
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> Gets(Func<S, A> f) =>
        new(state => M.Pure((f(state), state)));

    /// <summary>
    /// Extracts the state value and maps it to the bound value
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> GetsM(Func<S, K<M, A>> f) =>
        new(state => M.Map(v => (v, state), f(state)));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> Lift(Pure<A> monad) =>
        Pure(monad.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> Lift(K<M, A> monad) =>
        new(state => M.Map(value => (value, state), monad));

    /// <summary>
    /// Lifts a function into the transformer 
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> Lift(Func<A> f) =>
        new(state => M.Pure((f(), state)));
    
    /// <summary>
    /// Lifts a an IO monad into the monad 
    /// </summary>
    /// <remarks>NOTE: If the IO monad isn't the innermost monad of the transformer
    /// stack then this will throw an exception.</remarks>
    /// <param name="ma">IO monad to lift</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> LiftIO(IO<A> ma) =>
        Lift(M.LiftIO(ma));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="M1">Trait of the monad to map to</typeparam>
    /// <returns>`ReaderT`</returns>
    public StateT<S, M1, B> MapT<M1, B>(Func<K<M, (A Value, S State)>, K<M1, (B Value, S State)>> f)
        where M1 : Monad<M1>, Alternative<M1> =>
        new (state => f(runState(state)));

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, B> Map<B>(Func<A, B> f) =>
        new(state => M.Map(x => (f(x.Value), x.State), runState(state)));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, B> Select<B>(Func<A, B> f) =>
        new(state => M.Map(x => (f(x.Value), x.State), runState(state)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, B> Bind<B>(Func<A, K<StateT<S, M>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, B> Bind<B>(Func<A, StateT<S, M, B>> f) =>
        new(state => M.Bind(runState(state), x => f(x.Value).runState(x.State)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, B> Bind<B>(Func<A, Gets<S, B>> f) =>
        Bind(x => f(x).ToStateT<M>());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, Unit> Bind(Func<A, Put<S>> f) =>
        Bind(x => f(x).ToStateT<M>());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, Unit> Bind(Func<A, Modify<S>> f) =>
        Bind(x => f(x).ToStateT<M>());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => StateT<S, M, B>.LiftIO(f(x)));

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
    /// <returns>`StateT`</returns>
    public StateT<S, M, C> SelectMany<B, C>(Func<A, K<StateT<S, M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, C> SelectMany<B, C>(Func<A, StateT<S, M, B>> bind, Func<A, B, C> project) =>
        new(state => M.Bind(runState(state), 
                            x => M.Map(y => (project(x.Value, y.Value), x.State), 
                                       bind(x.Value).runState(x.State))));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        new(state => M.Bind(runState(state), x => M.Map(y => (project(x.Value, y), x.State), bind(x.Value))));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, C> SelectMany<C>(Func<A, Put<S>> bind, Func<A, Unit, C> project) =>
        Bind(x => bind(x).ToStateT<M>().Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, C> SelectMany<B, C>(Func<A, Gets<S, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).ToStateT<M>().Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, C> SelectMany<C>(Func<A, Modify<S>> bind, Func<A, Unit, C> project) =>
        Bind(x => bind(x).ToStateT<M>().Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public StateT<S, M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //

    public static implicit operator StateT<S, M, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    public static implicit operator StateT<S, M, A>(IO<A> ma) =>
        LiftIO(ma);
    
    public static StateT<S, M, A> operator |(StateT<S, M, A> ma, StateT<S, M, A> mb) =>
        new (state => M.Or(ma.runState(state), mb.runState(state)));
    
    public static StateT<S, M, A> operator |(StateT<S, M, A> ma, Pure<A> mb) =>
        new (state => M.Or(ma.runState(state), Pure(mb.Value).runState(state)));
    
    public static StateT<S, M, A> operator |(Pure<A> ma,  StateT<S, M, A>mb) =>
        new (state => M.Or(Pure(ma.Value).runState(state), mb.runState(state)));
    
    public static StateT<S, M, A> operator |(IO<A> ma, StateT<S, M, A> mb) =>
        new (state => M.Or(LiftIO(ma).runState(state), mb.runState(state)));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the monad
    //

    /// <summary>
    /// Run the state monad 
    /// </summary>
    /// <param name="state">Initial state</param>
    /// <returns>Bound monad</returns>
    public K<M, (A Value, S State)> Run(S state) =>
        runState(state);
}
