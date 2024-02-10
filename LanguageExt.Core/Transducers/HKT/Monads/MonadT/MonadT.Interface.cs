namespace LanguageExt.HKT;

/// <summary>
/// MonadT interface
/// </summary>
/// <typeparam name="M">Outer monad trait</typeparam>
/// <typeparam name="N">Inner monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface MonadT<M, N, A> : Monad<M, A>
    where M : MonadT<M, N>
    where N : Monad<N>;
