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
    public static MonadT<ReaderT<Env, M>, M, A> Pure<A>(A value) =>
        ReaderT<Env, M, A>.Pure(value);

    public static MonadT<ReaderT<Env, M>, M, A> Lift<A>(Monad<M, A> ma) => 
        ReaderT<Env, M, A>.Lift(ma);

    public static MonadT<ReaderT<Env, M>, M, A> Asks<A>(Transducer<Env, A> ma) => 
        ReaderT<Env, M, A>.Lift(ma);

    public static MonadT<ReaderT<Env, M>, M, B> Bind<A, B>(
        MonadT<ReaderT<Env, M>, M, A> mma,
        Transducer<A, MonadT<ReaderT<Env, M>, M, B>> f) =>
        new ReaderT<Env, M, B>(
            lift<Env, Transducer<Env, Monad<M, B>>>(
                env =>
                    mma.As().runReader
                       .Map(ma =>
                                M.Map(ma,
                                      f.Map(mb => mb.As()
                                                    .runReader
                                                    .Invoke(env)).Flatten()).AsMonad()).Map(M.Flatten)).Flatten());
    
    public static MonadT<ReaderT<Env, M>, M, A> Local<A>(
        Transducer<Env, Env> f,
        MonadT<ReaderT<Env, M>, M, A> ma) =>
        ma.As().Local(f);
}
