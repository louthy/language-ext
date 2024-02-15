using System;

namespace LanguageExt.Traits;

public static class MonadStateT
{
    public static K<MState, Unit> put<MState, S, M>(S value)
        where M : Monad<M>
        where MState : MonadStateT<MState, S, M> =>
        MState.Put(value);

    public static  K<MState, Unit> modify<MState, S, M>(Func<S, S> modify)
        where M : Monad<M>
        where MState : MonadStateT<MState, S, M> =>
        MState.Modify(modify);

    public static K<MState, S> get<MState, S, M>()
        where M : Monad<M>
        where MState : MonadStateT<MState, S, M> =>
        MState.Get;

    public static K<MState, A> gets<MState, S, M, A>(Func<S, A> f)
        where M : Monad<M>
        where MState : MonadStateT<MState, S, M> =>
        MState.Gets(f);
}
