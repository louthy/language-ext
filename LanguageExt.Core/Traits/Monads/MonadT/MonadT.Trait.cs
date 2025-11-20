namespace LanguageExt.Traits;

/// <summary>
/// MonadT trait
/// </summary>
/// <typeparam name="T">Self referring trait</typeparam>
/// <typeparam name="M">Inner monad trait</typeparam>
public interface MonadT<T, out M> : Monad<T> 
    where T : MonadT<T, M>
    where M : Monad<M>
{
    public static abstract K<T, A> Lift<A>(K<M, A> ma);
}
