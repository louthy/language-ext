#nullable enable
using System;

namespace LanguageExt.Transducers;

/// <summary>
/// Reducer is an encapsulation of a fold operation.  It also takes a `TState` which can be
/// used to track resources allocated.
/// </summary>
/// <typeparam name="A">Value type</typeparam>
/// <typeparam name="S">State type</typeparam>
public abstract record Reducer<A, S> 
{
    /// <summary>
    /// Run the reduce operation with an initial state and value
    /// </summary>
    public abstract TResult<S> Run(TState state, S stateValue, A value);
}

public static class Reducer
{
    public static Reducer<A, S> from<A, S>(Func<TState, S, A, TResult<S>> f) =>
        new FReducer<A, S>(f);
}

public static class Reducer<A>
{
    public static readonly Reducer<A, A> identity = 
        Reducer.from<A, A>((_, _, v) => TResult.Continue(v));
    
    public static readonly Reducer<A, Option<A>> optionIdentity = 
        Reducer.from<A, Option<A>>((_, _, v) => TResult.Continue(Prelude.Some(v)));
}

record FReducer<A, S>(Func<TState, S, A, TResult<S>> F) : Reducer<A, S>
{
    public override TResult<S> Run(TState state, S stateValue, A value) =>
        F(state, stateValue, value);
}
