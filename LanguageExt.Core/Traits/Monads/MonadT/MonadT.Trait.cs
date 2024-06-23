using System;

namespace LanguageExt.Traits;

/// <summary>
/// MonadT trait
/// </summary>
/// <typeparam name="M">Self referring trait</typeparam>
/// <typeparam name="N">Inner monad trait</typeparam>
public interface MonadT<M, N> : Monad<M> 
    where M : MonadT<M, N>
    where N : Monad<N>
{
    public static abstract K<M, A> Lift<A>(K<N, A> ma);
    //public static abstract K<M, B> MapM<A, B>(Func<K<N, A>, K<N, B>> f, K<M, A> ma);
}
