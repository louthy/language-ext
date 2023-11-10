#nullable enable
using System;

namespace LanguageExt.Transducers;

/// <summary>
/// Reducer is an encapsulation of a fold operation.  It also takes a `TState` which can be
/// used to track resources allocated.
/// </summary>
/// <typeparam name="S">State type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public abstract record Reducer<S, A> 
{
    /// <summary>
    /// Run the reduce operation with an initial state and value
    /// </summary>
    public abstract TResult<S> Run(TState state, S stateValue, A value);
}

public static class Reducer
{
    public static Reducer<S, A> from<S, A>(Func<TState, S, A, TResult<S>> f) =>
        new FReducer<S, A>(f);
}

record FReducer<S, A>(Func<TState, S, A, TResult<S>> F) : Reducer<S, A>
{
    public override TResult<S> Run(TState state, S stateValue, A value) =>
        F(state, stateValue, value);
}
