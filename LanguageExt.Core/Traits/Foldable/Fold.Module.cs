using System;

namespace LanguageExt;

/// <summary>
/// Fold continuation module
/// </summary>
public record Fold
{
    /// <summary>
    /// Indicates the fold operation has completed
    /// </summary>
    /// <param name="state">State</param>
    public static Fold<A, S> Done<A, S>(S state) => 
        new Fold<A, S>.Done(state);

    /// <summary>
    /// Indicates the fold operation should continue
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="value">Current value</param>
    /// <param name="next">Continuation function, pass your updated state to this</param>
    public static Fold<A, S> Loop<A, S>(S state, A value, Func<S, Fold<A, S>> next) =>
        new Fold<A, S>.Loop(state, value, next);
}
