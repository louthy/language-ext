namespace LanguageExt.Traits;

public static partial class State
{
    /// <summary>
    /// Runs the `stateSetter` to update the state-monad's inner state.  Then runs the
    /// `operation`.  And finally, resets the state to how it was before running `stateSetter`.
    /// </summary>
    /// <returns>
    /// The result of `operation`
    /// </returns>
    public static K<M, A> Bracket<M, S, A>(this K<M, Unit> stateSetter, K<M, A> operation)
        where M : StateM<M, S>, Monad<M> =>
        from s in M.Get
        from _ in stateSetter
        from r in operation
        from u in M.Put(s)
        select r;
}
