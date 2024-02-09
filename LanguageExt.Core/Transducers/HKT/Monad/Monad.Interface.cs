namespace LanguageExt.HKT;

/// <summary>
/// Monad interface
/// </summary>
/// <typeparam name="M">Monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface Monad<M, A> : Functor<M, A>
    where M : Monad<M>
{
    public Monad<M, A> AsMonad() => this;
}
