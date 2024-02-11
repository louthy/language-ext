using System;

namespace LanguageExt.HKT;

public interface MonadReaderT<MRdr, Env, M> : MonadT<MRdr, M> 
    where M : Monad<M>
    where MRdr : MonadReaderT<MRdr, Env, M>
{
    public static abstract K<MRdr, A> Asks<A>(Func<Env, A> f);

    public static virtual K<MRdr, Env> Ask =>
        MRdr.Asks(Prelude.identity);

    public static abstract K<MRdr, A> Local<A>(
        Func<Env, Env> f,
        K<MRdr, A> ma);
}
