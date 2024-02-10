namespace LanguageExt.HKT;

public static class MonadT
{
    public static MonadT<MTran, M, A> lift<MTran, M, A>(Monad<M, A> ma)
        where M : Monad<M>
        where MTran : MonadT<MTran, M> =>
        MTran.Lift(ma);
}
