using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// Monad transformer trait implementation for `Proxy`
/// </summary>
public class Proxy<UOut, UIn, DIn, DOut, M> : 
    MonadT<Proxy<UOut, UIn, DIn, DOut, M>, M>
    where M : Monad<M>
{
    static K<Proxy<UOut, UIn, DIn, DOut, M>, B> Monad<Proxy<UOut, UIn, DIn, DOut, M>>.Bind<A, B>(
        K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma, 
        Func<A, K<Proxy<UOut, UIn, DIn, DOut, M>, B>> f) => 
        ma.As().Bind(x => f(x).As());

    static K<Proxy<UOut, UIn, DIn, DOut, M>, B> Functor<Proxy<UOut, UIn, DIn, DOut, M>>.Map<A, B>(
        Func<A, B> f, K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma) => 
        ma.As().Map(f);

    static K<Proxy<UOut, UIn, DIn, DOut, M>, A> Applicative<Proxy<UOut, UIn, DIn, DOut, M>>.Pure<A>(A value) => 
        Proxy.Pure<UOut, UIn, DIn, DOut, M, A>(value);

    static K<Proxy<UOut, UIn, DIn, DOut, M>, B> Applicative<Proxy<UOut, UIn, DIn, DOut, M>>.Apply<A, B>(
        K<Proxy<UOut, UIn, DIn, DOut, M>, Func<A, B>> mf, 
        K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma) => 
        mf.As().Apply(ma.As());

    static K<Proxy<UOut, UIn, DIn, DOut, M>, A> MonadT<Proxy<UOut, UIn, DIn, DOut, M>, M>.Lift<A>(K<M, A> ma) =>
        Proxy.lift<UOut, UIn, DIn, DOut, M, A>(ma);

    public static K<Proxy<UOut, UIn, DIn, DOut, M>, A> LiftIO<A>(IO<A> ma) => 
        Proxy.lift<UOut, UIn, DIn, DOut, M, A>(M.LiftIO(ma));

    public static K<Proxy<UOut, UIn, DIn, DOut, M>, B> WithRunInIO<A, B>(
        Func<Func<K<Proxy<UOut, UIn, DIn, DOut, M>, A>, IO<A>>, IO<B>> inner)
    {
        return Proxy.lift<UOut, UIn, DIn, DOut, M, B>(
            M.WithRunInIO<A, B>(
                run =>
                    inner(ma => run(Go(ma.As())))));

        K<M, R> Go<R>(Proxy<UOut, UIn, DIn, DOut, M, R> p) =>
            p.ToProxy() switch
            {
                ProxyM<UOut, UIn, DIn, DOut, M, R> (var mx) => mx.Bind(x => Go(x)),
                Pure<UOut, UIn, DIn, DOut, M, R> (var r)    => M.Pure(r),
                Iterator<UOut, UIn, DIn, DOut, M, R> iter   => runIterator(iter),
                Request<UOut, UIn, DIn, DOut, M, R>         => throw new InvalidOperationException("WithRunInIO only supported for closed Effects"),
                Respond<UOut, UIn, DIn, DOut, M, R>         => throw new InvalidOperationException("WithRunInIO only supported for closed Effects"),
                _                                           => throw new NotSupportedException()
            };
        
        K<M, R> runIterator<R>(Iterator<UOut, UIn, DIn, DOut, M, R> iter) =>
            from _ in iter.Run().Select(Go).Actions()
            from r in Go(iter.Next())
            select r;
    }
}
