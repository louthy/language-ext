using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// State monad extensions
/// </summary>
public static class StateExtensions
{
    public static State<S, A> As<S, A>(this K<State<S>, A> ma) =>
        (State<S, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static State<S, A> Flatten<S, A>(this State<S, State<S, A>> mma) =>
        mma.Bind(x => x);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static State<S, A> Flatten<S, A>(this State<S, K<State<S>, A>> mma) =>
        mma.Bind(x => x);
}
