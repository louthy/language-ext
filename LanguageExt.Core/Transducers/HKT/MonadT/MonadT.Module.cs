using System;

namespace LanguageExt.HKT;

public static class MonadT
{
    public static MonadT<MTran, M, A> pure<MTran, M, A>(A value)
        where M : Monad<M>
        where MTran : MonadT<MTran, M> =>
        MTran.Pure(value);

    public static MonadT<MTran, M, A> lift<MTran, M, A>(Monad<M, A> ma)
        where M : Monad<M>
        where MTran : MonadT<MTran, M> =>
        MTran.Lift(ma);

    public static MonadT<MTran, M, A> flatten<MTran, M, A>(MonadT<MTran, M, MonadT<MTran, M, A>> mma)
        where M : Monad<M>
        where MTran : MonadT<MTran, M> =>
        MTran.Flatten(mma);

    public static MonadT<MTran, M, B> bind<MTran, M, A, B>(
        MonadT<MTran, M, A> ma,
        Transducer<A, MonadT<MTran, M, B>> f)
        where M : Monad<M>
        where MTran : MonadT<MTran, M> =>
        MTran.Bind(ma, f);

    public static MonadT<MTran, M, B> bind<MTran, M, A, B>(
        MonadT<MTran, M, A> ma,
        Func<A, MonadT<MTran, M, B>> f)
        where M : Monad<M>
        where MTran : MonadT<MTran, M> =>
        MTran.Bind(ma, f);

    public static MB bind<MTran, MB, M, A, B>(
        MonadT<MTran, M, A> ma,
        Transducer<A, MB> f)
        where MB : MonadT<MTran, M, B>
        where M : Monad<M>
        where MTran : MonadT<MTran, M> =>
        (MB)MTran.Bind(ma, f.Map(x => (MonadT<MTran, M, B>)x));

    public static MB bind<MTran, MB, M, A, B>(
        MonadT<MTran, M, A> ma,
        Func<A, MB> f)
        where MB : MonadT<MTran, M, B>
        where M : Monad<M>
        where MTran : MonadT<MTran, M> =>
        (MB)MTran.Bind(ma, x => f(x));
}
