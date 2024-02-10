using LanguageExt.HKT;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `MonadReaderT` trait implementation for `ReaderT` 
/// </summary>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public class ReaderT<Env, M> : MonadReaderT<ReaderT<Env, M>, Env, M>
    where M : Monad<M>
{
    public static Applicative<ReaderT<Env, M>, A> Pure<A>(A value) => 
        ReaderT<Env, M, A>.Pure(value);

    public static Applicative<ReaderT<Env, M>, B> Apply<A, B>(
        Applicative<ReaderT<Env, M>, Transducer<A, B>> mmf,
        Applicative<ReaderT<Env, M>, A> mma) =>
        new ReaderT<Env, M, B>(
            lift<Env, Transducer<Env, Monad<M, B>>>(
                env =>
                    mmf.As()
                       .runReader
                       .Map(mf =>
                                mma.As()
                                   .runReader
                                   .Map(ma => M.Apply(mf, ma)))
                      .Invoke(env)).Flatten());

    public static Applicative<ReaderT<Env, M>, B> Action<A, B>(
        Applicative<ReaderT<Env, M>, A> mma, 
        Applicative<ReaderT<Env, M>, B> mmb) => 
        new ReaderT<Env, M, B>(
            lift<Env, Transducer<Env, Monad<M, B>>>(
                env =>
                    mma.As()
                       .runReader
                       .Map(ma =>
                                mmb.As()
                                   .runReader
                                   .Map(mb => M.Action(ma, mb)))
                       .Invoke(env)).Flatten());

    public static Monad<ReaderT<Env, M>, B> Bind<A, B>(
        Monad<ReaderT<Env, M>, A> mma,
        Transducer<A, Monad<ReaderT<Env, M>, B>> f) =>
        new ReaderT<Env, M, B>(
            lift<Env, Transducer<Env, Monad<M, B>>>(
                env =>
                     mma.As()
                        .runReader
                        .Map(ma =>
                                 M.Map(ma,
                                       f.Map(mb => mb.As()
                                                     .runReader
                                                     .Invoke(env)).Flatten())).Map(M.Flatten)).Flatten());

    public static MonadT<ReaderT<Env, M>, M, A> Lift<A>(Monad<M, A> ma) => 
        ReaderT<Env, M, A>.Lift(ma);
    
    public static MonadReaderT<ReaderT<Env, M>, Env, M, A> Asks<A>(Transducer<Env, A> ma) => 
        ReaderT<Env, M, A>.Lift(ma);

    public static MonadReaderT<ReaderT<Env, M>, Env, M, A> Local<A>(
        Transducer<Env, Env> f,
        MonadT<ReaderT<Env, M>, M, A> ma) =>
        ma.As().Local(f);
}
