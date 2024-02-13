using System;
using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// `MonadReaderT` trait implementation for `ReaderT` 
/// </summary>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class ReaderT<Env, M>
    where M : MonadIO<M>
{
    public static ReaderT<Env, M, B> bind<A, B>(ReaderT<Env, M, A> ma, Func<A, ReaderT<Env, M, B>> f) => 
        ma.As().Bind(f);

    public static ReaderT<Env, M, B> map<A, B>(Func<A, B> f, ReaderT<Env, M, A> ma) => 
        ma.As().Map(f);

    public static ReaderT<Env, M, A> Pure<A>(A value) => 
        ReaderT<Env, M, A>.Pure(value);

    public static ReaderT<Env, M, B> apply<A, B>(ReaderT<Env, M, Func<A, B>> mf, ReaderT<Env, M, A> ma) => 
        mf.As().Bind(ma.As().Map);

    public static ReaderT<Env, M, B> action<A, B>(ReaderT<Env, M, A> ma, ReaderT<Env, M, B> mb) =>
        ma.As().Bind(_ => mb);

    public static ReaderT<Env, M, A> lift<A>(K<M, A> ma) => 
        ReaderT<Env, M, A>.Lift(ma);

    public static ReaderT<Env, M, Env> ask =>
        ReaderT<Env, M, Env>.Asks(Prelude.identity);

    public static ReaderT<Env, M, A> asks<A>(Func<Env, A> f) => 
        ReaderT<Env, M, A>.Asks(f);

    public static ReaderT<Env, M, A> local<A>(Func<Env, Env> f, ReaderT<Env, M, A> ma) =>
        ma.As().Local(f);

    public static ReaderT<Env, M, A> liftIO<A>(IO<A> ma) => 
        ReaderT<Env, M, A>.Lift(M.LiftIO(ma));
}
