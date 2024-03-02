using System;

namespace LanguageExt.Traits;

public static partial class MonadT
{
    public static K<M, B> MapM<M, N, A, B>(this Func<K<N, A>, K<N, B>> f, K<M, A> ma)
        where M : MonadT<M, N>
        where N : Monad<N> =>
        M.MapM(f, ma);

    public static K<M, B> MapM<M, N, A, B>(this K<M, A> ma, Func<K<N, A>, K<N, B>> f)
        where M : MonadT<M, N>
        where N : Monad<N> =>
        M.MapM(f, ma);
}
