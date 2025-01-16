using System;
using System.Diagnostics.Contracts;
using System.Linq;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;
using LanguageExt.Async.Linq;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// Effects represent a 'fused' set of producer, pipes, and consumer into one type.
/// 
/// It neither can neither `yield` nor be `awaiting`, it represents an entirely closed effect system.
/// </summary>
/// <remarks>
///       Upstream | Downstream
///           +---------+
///           |         |
///     Void〈==      〈== Unit
///           |         |
///     Unit ==〉      ==〉Void
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public static class Effect
{
    [Pure]
    public static K<M, R> RunEffect<M, R>(this K<Proxy<Void, Unit, Unit, Void, M>, R> ma)
        where M : Monad<M>
    {
        return Go(ma.As());

        K<M, R> Go(Proxy<Void, Unit, Unit, Void, M, R> p) =>
            p.ToProxy() switch
            {
                ProxyM<Void, Unit, Unit, Void, M, R> (var mx)    => M.Bind(mx, Go),
                Pure<Void, Unit, Unit, Void, M, R> (var r)       => M.Pure(r),
                Iterator<Void, Unit, Unit, Void, M, R> iter      => runIterator(iter, Go),
                IteratorAsync<Void, Unit, Unit, Void, M, R> iter => runAsyncIterator(iter, Go),
                Request<Void, Unit, Unit, Void, M, R> (var v, _) => closed<K<M, R>>(v),
                Respond<Void, Unit, Unit, Void, M, R> (var v, _) => closed<K<M, R>>(v),
                _                                                => throw new NotSupportedException()
            };
    }

    static K<M, R> runIterator<M, R>(
        Iterator<Void, Unit, Unit, Void, M, R> iter,
        Func<Proxy<Void, Unit, Unit, Void, M, R>, K<M, R>> go)
        where M : Monad<M> =>
        from _ in iter.Run().Select(e => e.RunEffect()).Actions()
        from r in go(iter.Next())
        select r;

    static K<M, R> runAsyncIterator<M, R>(
        IteratorAsync<Void, Unit, Unit, Void, M, R> iter,
        Func<Proxy<Void, Unit, Unit, Void, M, R>, K<M, R>> go)
        where M : Monad<M> =>
        from _ in iter.Run().Select(e => e.RunEffect()).Actions()
        from r in go(iter.Next())
        select r;
    
    [Pure]
    static Proxy<UOut, UIn, DIn, DOut, N, R> HoistX<UOut, UIn, DIn, DOut, M, N, R>(
        this Proxy<UOut, UIn, DIn, DOut, M, R> ma,
        Func<K<M, Proxy<UOut, UIn, DIn, DOut, N, R>>, K<N, Proxy<UOut, UIn, DIn, DOut, N, R>>> nat) 
        where M : Monad<M> 
        where N : Monad<N> 
    {
        return Go(ma);

        Proxy<UOut, UIn, DIn, DOut, N, R> Go(Proxy<UOut, UIn, DIn, DOut, M, R> p) =>
            p.ToProxy() switch
            {
                ProxyM<UOut, UIn, DIn, DOut, M, R> (var mx)          => new ProxyM<UOut, UIn, DIn, DOut, N, R>(nat(mx.Map(Go))),
                Pure<UOut, UIn, DIn, DOut, M, R> (var r)             => new Pure<UOut, UIn, DIn, DOut, N, R> (r),
                Request<UOut, UIn, DIn, DOut, M, R> (var a1, var fa) => new Request<UOut, UIn, DIn, DOut, N, R>(a1, a =>Go(fa(a))),
                Respond<UOut, UIn, DIn, DOut, M, R> (var b, var fb1) => new Respond<UOut, UIn, DIn, DOut, N, R> (b, b1 => Go(fb1(b1))),
                _                                                    => throw new NotSupportedException()
            };
    }
        
    [Pure, MethodImpl(mops)]
    public static Effect<M, R> lift<M, R>(K<M, R> ma) 
        where M : Monad<M> =>
        lift<Void, Unit, Unit, Void, M, R>(ma).ToEffect();

    [Pure, MethodImpl(mops)]
    public static Effect<M, R> liftIO<M, R>(IO<R> ma) 
        where M : Monad<M> =>
        liftIO<Void, Unit, Unit, Void, M, R>(ma).ToEffect();

    [Pure, MethodImpl(mops)]
    public static Effect<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, Effect<M, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M> =>
        lift(ma).SelectMany(bind, project);
}
