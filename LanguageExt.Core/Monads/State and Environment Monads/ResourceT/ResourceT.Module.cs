using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `MonadReaderT` trait implementation for `ReaderT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class ResourceT<M>
    where M : Monad<M>
{
    public static ResourceT<M, A> Pure<A>(A value) => 
        ResourceT<M, A>.Pure(value);

    public static ResourceT<M, A> liftIO<A>(IO<A> ma) => 
        ResourceT<M, A>.Lift(M.LiftIO(ma));

    public static ResourceT<M, A> use<A>(IO<A> ma, Action<A> release) 
        where A : class =>
        liftIO(ma).Bind(
            a => ResourceT<M, A>.Asks(
                rs =>
                {
                    rs.Acquire(a, release);
                    return a;
                }));

    public static ResourceT<M, A> use<A>(Func<EnvIO, A> f) 
        where A : IDisposable =>
        use(IO<A>.Lift(f));

    public static ResourceT<M, A> use<A>(Func<A> f) 
        where A : IDisposable =>
        use(IO<A>.Lift(f));

    public static ResourceT<M, A> use<A>(IO<A> ma) 
        where A : IDisposable =>
        liftIO(ma).Bind(
            a => ResourceT<M, A>.Asks(
                rs =>
                {
                    rs.Acquire(a);
                    return a;
                }));

    public static ResourceT<M, Unit> release<A>(A value) =>
        ResourceT<M, Unit>.Asks(
            rs =>
            {
                rs.Release(value);
                return Prelude.unit;
            });
}

/// <summary>
/// `MonadReaderT` trait implementation for `ReaderT` 
/// </summary>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class ResourceT
{
    public static ResourceT<M, B> bind<M, A, B>(ResourceT<M, A> ma, Func<A, ResourceT<M, B>> f) 
        where M : Monad<M> =>
        ma.As().Bind(f);

    public static ResourceT<M, B> map<M, A, B>(Func<A, B> f, ResourceT<M, A> ma) 
        where M : Monad<M> =>
        ma.As().Map(f);

    public static ResourceT<M, A> Pure<M, A>(A value) 
        where M : Monad<M> =>
        ResourceT<M, A>.Pure(value);

    public static ResourceT<M, B> apply<M, A, B>(ResourceT<M, Func<A, B>> mf, ResourceT<M, A> ma)
        where M : Monad<M> =>
        mf.As().Bind(ma.As().Map);

    public static ResourceT<M, B> action<M, A, B>(ResourceT<M, A> ma, ResourceT<M, B> mb)
        where M : Monad<M> =>
        ma.As().Bind(_ => mb);

    public static ResourceT<M, A> lift<M, A>(K<M, A> ma)
        where M : Monad<M> =>
        ResourceT<M, A>.Lift(ma);

    public static ResourceT<M, A> liftIO<M, A>(IO<A> ma) 
        where M : Monad<M> =>
        ResourceT<M, A>.Lift(M.LiftIO(ma));

    public static ResourceT<M, A> acquire<M, A>(IO<A> ma, Action<A> release) 
        where A : class 
        where M : Monad<M> =>
        liftIO<M, A>(ma).Bind(
            a => ResourceT<M, A>.Asks(
                rs =>
                {
                    rs.Acquire(a, release);
                    return a;
                }));

    public static ResourceT<M, A> acquire<M, A>(Func<EnvIO, A> f) 
        where A : IDisposable
        where M : Monad<M> =>
        acquire<M, A>(IO<A>.Lift(f));

    public static ResourceT<M, A> acquire<M, A>(Func<A> f) 
        where A : IDisposable
        where M : Monad<M> =>
        acquire<M, A>(IO<A>.Lift(f));

    public static ResourceT<M, A> acquire<M, A>(IO<A> ma) 
        where A : IDisposable 
        where M : Monad<M> =>
        liftIO<M, A>(ma).Bind(
            a => ResourceT<M, A>.Asks(
                rs =>
                {
                    rs.Acquire(a);
                    return a;
                }));

    public static ResourceT<M, Unit> release<M, A>(A value)
        where M : Monad<M> =>
        ResourceT<M, Unit>.Asks(
            rs =>
            {
                rs.Release(value);
                return Prelude.unit;
            });
}
