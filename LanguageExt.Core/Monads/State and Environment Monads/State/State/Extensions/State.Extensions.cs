using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// State monad extensions
/// </summary>
public static partial class StateExtensions
{
    public static State<S, A> As<S, A>(this K<State<S>, A> ma) =>
        (State<S, A>)ma;

    /// <summary>
    /// Run the state monad 
    /// </summary>
    /// <param name="state">Initial state</param>
    public static (A Value, S State) Run<S, A>(this K<State<S>, A> ma, S state) =>
        ((State<S, A>)ma).runState(state);
    
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
