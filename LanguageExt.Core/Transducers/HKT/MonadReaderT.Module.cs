using System;

namespace LanguageExt.HKT;

public static class MonadReaderT
{
    public static KArrow<MRdr, Env, M, A> pure<MRdr, Env, M, A>(A value)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Pure(value);

    public static KArrow<MRdr, Env, M, A> lift<MRdr, Env, M, A>(KStar<M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Lift(ma);

    public static KArrow<MRdr, Env, M, Env> ask<MRdr, Env, M>()
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Ask;

    public static KArrow<MRdr, Env, M, A> local<MRdr, Env, M, A>(Transducer<Env, Env> f, KArrow<MRdr, Env, M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Local(f, ma);

    public static KArrow<MRdr, Env, M, A> local<MRdr, Env, M, A>(Func<Env, Env> f, KArrow<MRdr, Env, M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Local(f, ma);

    public static KArrow<MRdr, Env, M, A> flatten<MRdr, Env, M, A>(KArrow<MRdr, Env, M, KArrow<MRdr, Env, M, A>> mma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Flatten(mma);

    public static KArrow<MRdr, Env, M, B> map<MRdr, Env, M, A, B>(KArrow<MRdr, Env, M, A> ma, Transducer<A, B> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        Functor.map(ma, f);

    public static KArrow<MRdr, Env, M, B> map<MRdr, Env, M, A, B>(KArrow<MRdr, Env, M, A> ma, Func<A, B> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        Functor.map(ma, f);

    public static KArrow<MRdr, Env, M, B> bind<MRdr, Env, M, A, B>(
        KArrow<MRdr, Env, M, A> ma,
        Transducer<A, KArrow<MRdr, Env, M, B>> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Bind(ma, f);

    public static KArrow<MRdr, Env, M, B> bind<MRdr, Env, M, A, B>(
        KArrow<MRdr, Env, M, A> ma,
        Func<A, KArrow<MRdr, Env, M, B>> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Bind(ma, f);

    public static MB bind<MRdr, MB, Env, M, A, B>(
        KArrow<MRdr, Env, M, A> ma,
        Transducer<A, MB> f)
        where MB : KArrow<MRdr, Env, M, B>
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        (MB)MRdr.Bind(ma, f.Map(x => x.AsKind()));

    public static MB bind<MRdr, MB, Env, M, A, B>(
        KArrow<MRdr, Env, M, A> ma,
        Func<A, MB> f)
        where MB : KArrow<MRdr, Env, M, B>
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        (MB)MRdr.Bind(ma, x => f(x));
}
