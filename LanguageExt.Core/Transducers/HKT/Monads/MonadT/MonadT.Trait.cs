namespace LanguageExt.HKT;

/// <summary>
/// MonadT trait
/// </summary>
/// <typeparam name="M">Self referring trait</typeparam>
/// <typeparam name="N">Inner monad trait</typeparam>
public interface MonadT<M, N> : Monad<M> 
    where M : MonadT<M, N>
    where N : Monad<N>
{
    public static abstract MonadT<M, N, A> Lift<A>(Monad<N, A> ma);
}
