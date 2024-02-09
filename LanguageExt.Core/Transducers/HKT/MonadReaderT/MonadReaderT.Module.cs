using System;

namespace LanguageExt.HKT;

public static class MonadReaderT
{
    public static MonadReaderT<MRdr, Env, M, A> ask<MRdr, Env, M, A>(Transducer<Env, A> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Asks(f);

    public static MonadReaderT<MRdr, Env, M, Env> ask<MRdr, Env, M>()
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Ask;

    public static MonadReaderT<MRdr, Env, M, A> local<MRdr, Env, M, A>(Transducer<Env, Env> f, MonadT<MRdr, M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Local(f, ma);

    public static MonadReaderT<MRdr, Env, M, A> local<MRdr, Env, M, A>(Func<Env, Env> f, MonadT<MRdr, M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Local(f, ma);

    public static MonadReaderT<MTran, Env, M, B> bind<MTran, Env, M, A, B>(
        MonadReaderT<MTran, Env, M, A> ma,
        Transducer<A, MonadReaderT<MTran, Env, M, B>> f)
        where M : Monad<M>
        where MTran : MonadReaderT<MTran, Env, M> =>
        (MonadReaderT<MTran, Env, M, B>)MTran.Bind(ma, f.Map(x => (MonadT<MTran, M, B>)x));

    public static MonadReaderT<MTran, Env, M, B> bind<MTran, Env, M, A, B>(
        MonadReaderT<MTran, Env, M, A> ma,
        Func<A, MonadReaderT<MTran, Env, M, B>> f)
        where M : Monad<M>
        where MTran : MonadReaderT<MTran, Env, M> =>
        (MonadReaderT<MTran, Env, M, B>)MTran.Bind(ma, f);
    
}
