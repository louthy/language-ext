namespace LanguageExt.HKT;

/// <summary>
/// MonadT interface
/// </summary>
/// <typeparam name="M">Outer monad trait</typeparam>
/// <typeparam name="Env">Environment</typeparam>
/// <typeparam name="N">Inner monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface MonadReaderT<M, Env, N, A> : MonadT<M, N, A>
    where M : MonadT<M, N>, Monad<M>
    where N : Monad<N>;
