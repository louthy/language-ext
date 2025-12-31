using System;

namespace LanguageExt;

/// <summary>
/// A continuation of a fold operation 
/// </summary>
/// <remarks>
/// Returned from `Foldable.FoldStep` and `Foldable.FoldStepBack`, it enables consumption of a foldable structure
/// one element at a time. Which in turn means we can avoid recursion and stack overflows.
/// </remarks>
/// <param name="State">Current state</param>
/// <typeparam name="A">Value type</typeparam>
/// <typeparam name="S">State type</typeparam>
public abstract record Fold<A, S>(S State)
{
    /// <summary>
    /// Indicates the fold operation has completed
    /// </summary>
    /// <param name="State">State</param>
    public sealed record Done(S State) : Fold<A, S>(State);
    
    /// <summary>
    /// Indicates the fold operation should continue
    /// </summary>
    /// <param name="State">Current state</param>
    /// <param name="Value">Current value</param>
    /// <param name="Next">Continuation function, pass your updated state to this</param>
    public sealed record Loop(S State, A Value, Func<S, Fold<A, S>> Next) : Fold<A, S>(State);
}
