using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `State` monad transformer, which adds a modifiable state to a given monad. 
/// </summary>
/// <param name="runState">Transducer that represents the transformer operation</param>
/// <typeparam name="S">State type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record State<S, A>(Func<S, (A Value, S State)> runState) : K<State<S>, A>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`State`</returns>
    public static State<S, A> Pure(A value) =>
        new (s => (value, s));

    /// <summary>
    /// Extracts the state value, maps it, and then puts it back into
    /// the monadic state.
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`State`</returns>
    public static State<S, Unit> Modify(Func<S, S> f) =>
        new (s => (unit, f(s)));

    /// <summary>
    /// Writes the value into the monadic state
    /// </summary>
    /// <returns>`State`</returns>
    public static State<S, Unit> Put(S value) =>
        new (_ => (unit, value));

    /// <summary>
    /// Extracts the state value and returns it as the bound value
    /// </summary>
    /// <returns>`State`</returns>
    public static State<S, S> Get { get; } =
        new (s => (s, s));

    /// <summary>
    /// Extracts the state value and maps it to the bound value
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`State`</returns>
    public static State<S, A> Gets(Func<S, A> f) =>
        new (s => (f(s), s));

    /// <summary>
    /// Extracts the state value and maps it to the bound value
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`State`</returns>
    public static State<S, A> GetsM(Func<S, State<S, A>> f) =>
        State<S, State<S, A>>.Gets(f).Flatten();

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`State`</returns>
    public static State<S, A> Lift(Pure<A> monad) =>
        Pure(monad.Value);

    /// <summary>
    /// Lifts a function into the transformer 
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`State`</returns>
    public static State<S, A> Lift(Func<A> f) =>
        new(s => (f(), s));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, B> Map<B>(Func<A, B> f) =>
        new(s => mapFirst(f, runState(s)));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, B> Select<B>(Func<A, B> f) =>
        new(s => mapFirst(f, runState(s)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, B> Bind<B>(Func<A, K<State<S>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, B> Bind<B>(Func<A, State<S, B>> f) =>
        new(s =>
            {
                var (a, s1) = runState(s);
                return f(a).runState(s1);
            });

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, B> Bind<B>(Func<A, Gets<S, B>> f) =>
        Bind(x => f(x).ToState());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, Unit> Bind(Func<A, Put<S>> f) =>
        Bind(x => f(x).ToState());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, Unit> Bind(Func<A, Modify<S>> f) =>
        Bind(x => f(x).ToState());

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
    /// <returns>`State`</returns>
    public State<S, C> SelectMany<B, C>(Func<A, K<State<S>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, C> SelectMany<B, C>(Func<A, State<S, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, C> SelectMany<C>(Func<A, Put<S>> bind, Func<A, Unit, C> project) =>
        SelectMany(x => bind(x).ToState(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, C> SelectMany<B, C>(Func<A, Gets<S, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).ToState(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`State`</returns>
    public State<S, C> SelectMany<C>(Func<A, Modify<S>> bind, Func<A, Unit, C> project) =>
        SelectMany(x => bind(x).ToState(), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //

    public static implicit operator State<S, A>(Pure<A> ma) =>
        Pure(ma.Value);

    public static State<S, A> operator >> (State<S, A> lhs, State<S, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    public static State<S, A> operator >> (State<S, A> lhs, K<State<S>, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the monad
    //

    /// <summary>
    /// Run the state monad 
    /// </summary>
    /// <param name="state">Initial state</param>
    /// <returns>Bound monad</returns>
    public (A Value, S State) Run(S state) =>
        runState(state);
}
