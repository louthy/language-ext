using System;
using LanguageExt;
using LanguageExt.Traits;

/// <summary>
/// State monad
/// </summary>
/// <param name="state"></param>
/// <typeparam name="S"></typeparam>
/// <typeparam name="A"></typeparam>
public record State<S, A>(StateT<S, Identity, A> state)
    : StateT<S, Identity, A>(state.runState)
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`StateT`</returns>
    public new static State<S, A> Pure(A value) =>
        new(StateT<S, Identity, A>.Pure(value));

    /// <summary>
    /// Extracts the state value, maps it, and then puts it back into
    /// the monadic state.
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`StateT`</returns>
    public new static State<S, Unit> Modify(Func<S, S> f) =>
        new(StateT<S, Identity, A>.Modify(f));

    /// <summary>
    /// Writes the value into the monadic state
    /// </summary>
    /// <returns>`StateT`</returns>
    public new static State<S, Unit> Put(S value) =>
        new(StateT<S, Identity, A>.Put(value));

    /// <summary>
    /// Extracts the state value and returns it as the bound value
    /// </summary>
    /// <returns>`StateT`</returns>
    public new static State<S, S> Get =>
        new(StateT<S, Identity, Unit>.Get);

    /// <summary>
    /// Extracts the state value and maps it to the bound value
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`StateT`</returns>
    public new static State<S, A> Gets(Func<S, A> f) =>
        new(StateT<S, Identity, A>.Gets(f));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`StateT`</returns>
    public new static State<S, A> Lift(Pure<A> pure) =>
        new(StateT<S, Identity, A>.Lift(pure));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`StateT`</returns>
    public new static State<S, A> Lift(K<Identity, A> ident) =>
        new(StateT<S, Identity, A>.Lift(ident));

    /// <summary>
    /// Lifts a function into the transformer 
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`StateT`</returns>
    public new static State<S, A> Lift(Func<A> f) =>
        new(StateT<S, Identity, A>.Lift(f));
    
    /// <summary>
    /// Lifts a an IO monad into the monad 
    /// </summary>
    /// <remarks>NOTE: If the IO monad isn't the innermost monad of the transformer
    /// stack then this will throw an exception.</remarks>
    /// <param name="ma">IO monad to lift</param>
    /// <returns>`StateT`</returns>
    public new static State<S, A> LiftIO(IO<A> ma) =>
        new(StateT<S, Identity, A>.LiftIO(ma));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, B> Map<B>(Func<A, B> f) =>
        new(state.Map(f));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, B> Select<B>(Func<A, B> f) =>
        Map(f);

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
    public new State<S, B> Bind<B>(Func<A, K<StateT<S, Identity>, B>> f) =>
        new(state.Bind(f));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, B> Bind<B>(Func<A, StateT<S, Identity, B>> f) =>
        new(state.Bind(f));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public State<S, B> Bind<B>(Func<A, State<S, B>> f) =>
        new(state.Bind(f));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, B> Bind<B>(Func<A, Gets<S, B>> f) =>
        new(state.Bind(f));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, Unit> Bind(Func<A, Put<S>> f) =>
        new(state.Bind(f));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, Unit> Bind(Func<A, Modify<S>> f) =>
        new(state.Bind(f));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, B> Bind<B>(Func<A, IO<B>> f) =>
        new(state.Bind(f));

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
    public new State<S, C> SelectMany<B, C>(Func<A, K<StateT<S, Identity>, B>> bind, Func<A, B, C> project) =>
        new(state.SelectMany(bind, project));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, C> SelectMany<B, C>(Func<A, StateT<S, Identity, B>> bind, Func<A, B, C> project) =>
        new(state.SelectMany(bind, project));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public State<S, C> SelectMany<B, C>(Func<A, State<S, B>> bind, Func<A, B, C> project) =>
        new(state.SelectMany(bind, project));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, C> SelectMany<B, C>(Func<A, K<Identity, B>> bind, Func<A, B, C> project) =>
        new(state.SelectMany(bind, project));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        new(state.SelectMany(bind, project));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, C> SelectMany<C>(Func<A, Put<S>> bind, Func<A, Unit, C> project) =>
        new(state.SelectMany(bind, project));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, C> SelectMany<B, C>(Func<A, Gets<S, B>> bind, Func<A, B, C> project) =>
        new(state.SelectMany(bind, project));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, C> SelectMany<C>(Func<A, Modify<S>> bind, Func<A, Unit, C> project) =>
        new(state.SelectMany(bind, project));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public new State<S, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        new(state.SelectMany(bind, project));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //

    public static implicit operator State<S, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    public static implicit operator State<S, A>(IO<A> ma) =>
        LiftIO(ma);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the monad
    //

    /// <summary>
    /// Run the state monad 
    /// </summary>
    /// <param name="state">Initial state</param>
    /// <returns>Bound monad</returns>
    public new (A Value, S State) Run(S state) =>
        runState(state).As().Value;

}
