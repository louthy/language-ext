using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `MonadReaderT` trait implementation for `ReaderT` 
/// </summary>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class ReaderT<Env, M> :
    MonadT<ReaderT<Env, M>, M>,
    ReaderM<ReaderT<Env, M>, Env>, 
    SemiAlternative<ReaderT<Env, M>>
    where M : Monad<M>, SemiAlternative<M>
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

    static K<ReaderT<Env, M>, B> MonadT<ReaderT<Env, M>, M>.MapM<A, B>(Func<K<M, A>, K<M, B>> f, K<ReaderT<Env, M>, A> ma) =>
        ma.As().MapM(f);

    static K<ReaderT<Env, M>, Env> ReaderM<ReaderT<Env, M>, Env>.Ask =>
        ReaderT<Env, M, Env>.Asks(Prelude.identity);

    static K<ReaderT<Env, M>, A> ReaderM<ReaderT<Env, M>, Env>.Asks<A>(Func<Env, A> f) => 
        ReaderT<Env, M, A>.Asks(f);

    static K<ReaderT<Env, M>, A> ReaderM<ReaderT<Env, M>, Env>.Local<A>(Func<Env, Env> f, K<ReaderT<Env, M>, A> ma) =>
        ma.As().Local(f);

    static K<ReaderT<Env, M>, A> Monad<ReaderT<Env, M>>.LiftIO<A>(IO<A> ma) =>
        ReaderT<Env, M, A>.Lift(M.LiftIO(ma));
    
    static K<ReaderT<Env, M>, B> Monad<ReaderT<Env, M>>.WithRunInIO<A, B>(
        Func<Func<K<ReaderT<Env, M>, A>, IO<A>>, IO<B>> inner) =>
        new ReaderT<Env, M, B>(
            env =>
                M.WithRunInIO<A, B>(
                    run =>
                        inner(ma => run(ma.As().runReader(env)))));

    static K<ReaderT<Env, M>, A> SemigroupK<ReaderT<Env, M>>.Combine<A>(
        K<ReaderT<Env, M>, A> ma, K<ReaderT<Env, M>, A> mb) =>
        new ReaderT<Env, M, A>(env => M.Combine(ma.As().runReader(env), mb.As().runReader(env)));

}
