using System;

namespace LanguageExt.HKT;

public static class MonadReaderT
{
    public static MonadReaderT<MRdr, Env, M, A> ask<MRdr, Env, M, A>(Transducer<Env, A> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M>, Monad<MRdr> =>
        MRdr.Asks(f);

    public static MonadReaderT<MRdr, Env, M, Env> ask<MRdr, Env, M>()
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M>, Monad<MRdr> =>
        MRdr.Ask;

    public static MonadReaderT<MRdr, Env, M, A> local<MRdr, Env, M, A>(Transducer<Env, Env> f, MonadT<MRdr, M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M>, Monad<MRdr> =>
        MRdr.Local(f, ma);

    public static MonadReaderT<MRdr, Env, M, A> local<MRdr, Env, M, A>(Func<Env, Env> f, MonadT<MRdr, M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M>, Monad<MRdr> =>
        MRdr.Local(f, ma);
}
