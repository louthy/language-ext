using System;

namespace LanguageExt.HKT;

public static class MonadReaderT
{
    public static K<MRdr, Env> ask<MRdr, Env, M>()
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M>, Monad<MRdr> =>
        MRdr.Ask;

    public static K<MRdr, A> local<MRdr, Env, M, A>(Func<Env, Env> f, K<MRdr, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M>, Monad<MRdr> =>
        MRdr.Local(f, ma);
}
