using System;
using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// `MonadReaderT` trait implementation for `ReaderT` 
/// </summary>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class ResourceT<M>
{
    public static ResourceT<M, B> bind<A, B>(ResourceT<M, A> ma, Func<A, ResourceT<M, B>> f) =>
        ma.As().Bind(f);

    public static ResourceT<M, B> map<A, B>(Func<A, B> f, ResourceT<M, A> ma) =>
        ma.As().Map(f);

    public static ResourceT<M, A> Pure<A>(A value) =>
        ResourceT<M, A>.Pure(value);

    public static ResourceT<M, B> apply<A, B>(ResourceT<M, Func<A, B>> mf, ResourceT<M, A> ma) =>
        mf.As().Bind(ma.As().Map);

    public static ResourceT<M, B> action<A, B>(ResourceT<M, A> ma, ResourceT<M, B> mb) =>
        ma.As().Bind(_ => mb);

    public static ResourceT<M, A> lift<A>(K<M, A> ma) =>
        ResourceT<M, A>.Lift(ma);

    public static ResourceT<M, A> liftIO<A>(IO<A> ma) =>
        ResourceT<M, A>.Lift(M.LiftIO(ma));

    public static ResourceT<M, A> acquire<A>(IO<A> ma, Action<A> release) where A : class =>
        liftIO(ma).Bind(
            a => ResourceT<M, A>.Asks(
                rs =>
                {
                    rs.Acquire(a, release);
                    return a;
                }));

    public static ResourceT<M, A> acquire<A>(Func<EnvIO, A> f) where A : IDisposable =>
        acquire(IO<A>.Lift(f));

    public static ResourceT<M, A> acquire<A>(Func<A> f) where A : IDisposable =>
        acquire(IO<A>.Lift(f));

    public static ResourceT<M, A> acquire<A>(IO<A> ma) where A : IDisposable =>
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
