using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;
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
///     Void <==       <== Unit
///           |         |
///     Unit ==>       ==> Void
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public static class Effect
{
    [Pure]
    internal static K<M, R> RunEffect<M, R>(this Proxy<Void, Unit, Unit, Void, M, R> ma) 
        where M : Monad<M> 
    {
        return Go(ma);
            
        K<M, R> Go(Proxy<Void, Unit, Unit, Void, M, R> p) =>
            p.ToProxy() switch
            {
                ProxyM<Void, Unit, Unit, Void, M, R> (var mx)     => M.Bind(mx, Go),
                Pure<Void, Unit, Unit, Void, M, R> (var r)        => M.Pure(r),                                                                                
                Request<Void, Unit, Unit, Void, M, R > (var v, _) => closed<K<M, R>>(v),
                Respond<Void, Unit, Unit, Void, M, R> (var v, _)  => closed<K<M, R>>(v),
                _                                                 => throw new NotSupportedException()
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
}
