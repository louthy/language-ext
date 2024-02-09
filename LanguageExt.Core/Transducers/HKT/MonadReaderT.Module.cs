using System;

namespace LanguageExt.HKT;

public static class MonadReaderT
{
    public static MonadT<MRdr, M, A> pure<MRdr, Env, M, A>(A value)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Pure(value);

    public static MonadT<MRdr, M, A> lift<MRdr, Env, M, A>(Monad<M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Lift(ma);

    public static MonadT<MRdr, M, A> ask<MRdr, Env, M, A>(Transducer<Env, A> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Asks(f);

    public static MonadT<MRdr, M, Env> ask<MRdr, Env, M>()
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Ask;

    public static MonadT<MRdr, M, A> local<MRdr, Env, M, A>(Transducer<Env, Env> f, MonadT<MRdr, M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Local(f, ma);

    public static MonadT<MRdr, M, A> local<MRdr, Env, M, A>(Func<Env, Env> f, MonadT<MRdr, M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Local(f, ma);

    public static MonadT<MRdr, M, A> flatten<MRdr, Env, M, A>(MonadT<MRdr, M, MonadT<MRdr, M, A>> mma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Flatten(mma);

    public static MonadT<MRdr, M, B> bind<MRdr, Env, M, A, B>(
        MonadT<MRdr, M, A> ma,
        Transducer<A, MonadT<MRdr, M, B>> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Bind(ma, f);

    public static MonadT<MRdr, M, B> bind<MRdr, Env, M, A, B>(
        MonadT<MRdr, M, A> ma,
        Func<A, MonadT<MRdr, M, B>> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Bind(ma, f);

    public static MB bind<MRdr, MB, Env, M, A, B>(
        MonadT<MRdr, M, A> ma,
        Transducer<A, MB> f)
        where MB : MonadT<MRdr, M, B>
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        (MB)MRdr.Bind(ma, f.Map(x => (MonadT<MRdr, M, B>)x));

    public static MB bind<MRdr, MB, Env, M, A, B>(
        MonadT<MRdr, M, A> ma,
        Func<A, MB> f)
        where MB : MonadT<MRdr, M, B>
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        (MB)MRdr.Bind(ma, x => f(x));
}
