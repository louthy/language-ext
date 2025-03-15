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
    Readable<ReaderT<Env, M>, Env>, 
    Choice<ReaderT<Env, M>>
    where M : Monad<M>, Choice<M>
{
    static K<ReaderT<Env, M>, B> Monad<ReaderT<Env, M>>.Bind<A, B>(
        K<ReaderT<Env, M>, A> ma,
        Func<A, K<ReaderT<Env, M>, B>> f) =>
        new ReaderT<Env, M, B>(env => ma.As().runReader(env).Bind(a => f(a).As().runReader(env)));

    static K<ReaderT<Env, M>, B> Functor<ReaderT<Env, M>>.Map<A, B>(Func<A, B> f, K<ReaderT<Env, M>, A> ma) => 
        new ReaderT<Env, M, B>(env => ma.As().runReader(env).Map(f));

    static K<ReaderT<Env, M>, A> Applicative<ReaderT<Env, M>>.Pure<A>(A value) => 
        new ReaderT<Env, M, A>(_ => M.Pure(value));

    static K<ReaderT<Env, M>, B> Applicative<ReaderT<Env, M>>.Apply<A, B>(K<ReaderT<Env, M>, Func<A, B>> mf, K<ReaderT<Env, M>, A> ma) => 
        new ReaderT<Env, M, B>(env => mf.As().runReader(env).Apply(ma.As().runReader(env)));

    static K<ReaderT<Env, M>, B> Applicative<ReaderT<Env, M>>.Action<A, B>(K<ReaderT<Env, M>, A> ma, K<ReaderT<Env, M>, B> mb) =>
        new ReaderT<Env, M, B>(env => ma.As().runReader(env).Action(mb.As().runReader(env)));

    static K<ReaderT<Env, M>, A> MonadT<ReaderT<Env, M>, M>.Lift<A>(K<M, A> ma) => 
        new ReaderT<Env, M, A>(_ => ma);

    static K<ReaderT<Env, M>, Env> Readable<ReaderT<Env, M>, Env>.Ask =>
        new ReaderT<Env, M, Env>(M.Pure);

    static K<ReaderT<Env, M>, A> Readable<ReaderT<Env, M>, Env>.Asks<A>(Func<Env, A> f) =>
        new ReaderT<Env, M, A>(env => M.Pure(f(env)));

    static K<ReaderT<Env, M>, A> Readable<ReaderT<Env, M>, Env>.Local<A>(Func<Env, Env> f, K<ReaderT<Env, M>, A> ma) =>
        new ReaderT<Env, M, A>(env => ma.As().runReader(f(env)));

    static K<ReaderT<Env, M>, A> MonadIO<ReaderT<Env, M>>.LiftIO<A>(IO<A> ma) =>
        new ReaderT<Env, M, A>(_ => M.LiftIO(ma));

    static K<ReaderT<Env, M>, IO<A>> MonadIO<ReaderT<Env, M>>.ToIO<A>(K<ReaderT<Env, M>, A> ma) =>
        new ReaderT<Env, M, IO<A>>(env => ma.As().runReader(env).ToIO());

    static K<ReaderT<Env, M>, A> SemigroupK<ReaderT<Env, M>>.Combine<A>(
        K<ReaderT<Env, M>, A> ma, K<ReaderT<Env, M>, A> mb) =>
        new ReaderT<Env, M, A>(env => M.Combine(ma.As().runReader(env), mb.As().runReader(env)));

    static K<ReaderT<Env, M>, A> Choice<ReaderT<Env, M>>.Choose<A>(
        K<ReaderT<Env, M>, A> ma, K<ReaderT<Env, M>, A> mb) =>
        new ReaderT<Env, M, A>(env => M.Choose(ma.As().runReader(env), mb.As().runReader(env)));

    public static K<ReaderT<Env, M>, A> Choose<A>(K<ReaderT<Env, M>, A> ma, Func<K<ReaderT<Env, M>, A>> mb) => 
        new ReaderT<Env, M, A>(env => M.Choose(ma.As().runReader(env), mb().As().runReader(env)));
}
