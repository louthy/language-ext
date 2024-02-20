using System;

namespace LanguageExt.Traits;

public static class State
{
    public static K<M, Unit> put<M, S>(S value)
        where M : State<M, S> =>
        M.Put(value);

    public static  K<M, Unit> modify<M, S>(Func<S, S> modify)
        where M : State<M, S> =>
        M.Modify(modify);

    public static K<M, S> get<M, S>()
        where M : State<M, S> =>
        M.Get;

    public static K<M, A> gets<M, S, A>(Func<S, A> f)
        where M : State<M, S> =>
        M.Gets(f);
}
