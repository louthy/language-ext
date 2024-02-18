using System.Diagnostics.Contracts;

/// <summary>
/// Extension methods for State
/// </summary>
public static class StateExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static State<S, A> Flatten<S, A>(this State<S, State<S, A>> ma) =>
        new(ma.Bind(x => x));

}
