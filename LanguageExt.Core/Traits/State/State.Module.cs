using System;

namespace LanguageExt.Traits;

public static class State
{
    public static K<M, Unit> put<M, S>(S value)
        where M : StateM<M, S> =>
        M.Put(value);

    public static  K<M, Unit> modify<M, S>(Func<S, S> modify)
        where M : StateM<M, S> =>
        M.Modify(modify);

    public static  K<M, Unit> modifyM<M, S>(Func<S, K<M, S>> modify)
        where M : StateM<M, S>, Monad<M> =>
        from s in M.Get
        from t in modify(s)
        from _ in M.Put(t)
        select default(Unit);    

    public static K<M, S> get<M, S>()
        where M : StateM<M, S> =>
        M.Get;

    public static K<M, A> gets<M, S, A>(Func<S, A> f)
        where M : StateM<M, S> =>
        M.Gets(f);

    public static K<M, A> getsM<M, S, A>(Func<S, K<M, A>> f)
        where M : StateM<M, S>, Monad<M> =>
        M.Flatten(M.Gets(f));

    public static K<M, A> state<M, S, A>(Func<S, (A Value, S State)> f)
        where M : StateM<M, S>, Monad<M> =>
        from s in M.Get
        let r = f(s)
        from _ in M.Put(r.State)
        select r.Value;

    /// <summary>
    /// Runs the `stateSetter` to update the state-monad's inner state.  Then runs the
    /// `operation`.  And finally, resets the state to how it was before running `stateSetter`.
    /// </summary>
    /// <returns>
    /// The result of `operation`
    /// </returns>
    public static K<M, A> bracket<M, S, A>(K<M, Unit> stateSetter, K<M, A> operation)
        where M : StateM<M, S>, Monad<M> =>
        from s in M.Get
        from _ in stateSetter
        from r in operation
        from u in M.Put(s)
        select r;
}
