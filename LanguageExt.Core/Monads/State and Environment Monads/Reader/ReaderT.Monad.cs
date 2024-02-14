using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `MonadReaderT` trait implementation for `ReaderT` 
/// </summary>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class ReaderT<Env, M> : MonadReaderT<ReaderT<Env, M>, Env, M>
    where M : Monad<M>
{
    static K<ReaderT<Env, M>, B> Monad<ReaderT<Env, M>>.Bind<A, B>(K<ReaderT<Env, M>, A> ma, Func<A, K<ReaderT<Env, M>, B>> f) => 
        ma.As().Bind(f);

    static K<ReaderT<Env, M>, B> Functor<ReaderT<Env, M>>.Map<A, B>(Func<A, B> f, K<ReaderT<Env, M>, A> ma) => 
        ma.As().Map(f);

    static K<ReaderT<Env, M>, A> Applicative<ReaderT<Env, M>>.Pure<A>(A value) => 
        ReaderT<Env, M, A>.Pure(value);

    static K<ReaderT<Env, M>, B> Applicative<ReaderT<Env, M>>.Apply<A, B>(K<ReaderT<Env, M>, Func<A, B>> mf, K<ReaderT<Env, M>, A> ma) => 
        mf.As().Bind(ma.As().Map);

    static K<ReaderT<Env, M>, B> Applicative<ReaderT<Env, M>>.Action<A, B>(K<ReaderT<Env, M>, A> ma, K<ReaderT<Env, M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<ReaderT<Env, M>, A> MonadT<ReaderT<Env, M>, M>.Lift<A>(K<M, A> ma) => 
        ReaderT<Env, M, A>.Lift(ma);

    static K<ReaderT<Env, M>, Env> MonadReaderT<ReaderT<Env, M>, Env, M>.Ask =>
        ReaderT<Env, M, Env>.Asks(Prelude.identity);

    static K<ReaderT<Env, M>, A> MonadReaderT<ReaderT<Env, M>, Env, M>.Asks<A>(Func<Env, A> f) => 
        ReaderT<Env, M, A>.Asks(f);

    static K<ReaderT<Env, M>, A> MonadReaderT<ReaderT<Env, M>, Env, M>.Local<A>(Func<Env, Env> f, K<ReaderT<Env, M>, A> ma) =>
        ma.As().Local(f);

    static K<ReaderT<Env, M>, A> Monad<ReaderT<Env, M>>.LiftIO<A>(IO<A> ma) =>
        ReaderT<Env, M, A>.Lift(M.LiftIO(ma));
}
