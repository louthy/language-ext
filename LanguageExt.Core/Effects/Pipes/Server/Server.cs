using System;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt.Pipes;

/// <summary>
/// `Server` receives requests of type `REQ` and sends responses of type `RES`.
///
/// `Servers` only `respond` and never `request`.
/// </summary>
/// <remarks> 
///       Upstream | Downstream
///           +---------+
///           |         |
///     Void <==       <== RES
///           |         |
///     Unit ==>       ==> REQ
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public class Server
{
    /// <summary>
    /// Monad return / pure
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<RT, REQ, RES, R> Pure<RT, REQ, RES, R>(R value) 
        where RT : HasIO<RT, Error> =>
        new Pure<RT, Void, Unit, REQ, RES, R>(value).ToServer();
 
    /// <summary>
    /// Send a value of type `RES` downstream and block waiting for a reply of type `REQ`
    /// </summary>
    /// <remarks>
    /// `respond` is the identity of the respond category.
    /// </remarks>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<RT, REQ, RES, REQ> respond<RT, REQ, RES>(RES value) 
        where RT : HasIO<RT, Error> =>
        new Respond<RT, Void, Unit, REQ, RES, REQ>(value, r => new Pure<RT, Void, Unit, REQ, RES, REQ>(r)).ToServer();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Eff<R> ma) 
        where RT : HasIO<RT, Error> =>
        new ProxyM<RT, Void, Unit, REQ, RES, R>(ma.WithRuntime<RT>().Morphism.MapRight(Proxy.Pure<RT, Void, Unit, REQ, RES, R>)).ToServer();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Eff<RT, R> ma) 
        where RT : HasIO<RT, Error> =>
        new ProxyM<RT, Void, Unit, REQ, RES, R>(ma.Map(Proxy.Pure<RT, Void, Unit, REQ, RES, R>).Morphism).ToServer();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Transducer<RT, R> ma) 
        where RT : HasIO<RT, Error> =>
        new ProxyM<RT, Void, Unit, REQ, RES, R>(ma.Map(Proxy.Pure<RT, Void, Unit, REQ, RES, R>)).ToServer();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Transducer<Unit, R> ma) 
        where RT : HasIO<RT, Error> =>
        new ProxyM<RT, Void, Unit, REQ, RES, R>(ma.Map(Proxy.Pure<RT, Void, Unit, REQ, RES, R>)).ToServer();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Transducer<RT, Sum<Error, R>> ma) 
        where RT : HasIO<RT, Error> =>
        new ProxyM<RT, Void, Unit, REQ, RES, R>(ma.MapRight(Proxy.Pure<RT, Void, Unit, REQ, RES, R>)).ToServer();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Transducer<Unit, Sum<Error, R>> ma) 
        where RT : HasIO<RT, Error> =>
        new ProxyM<RT, Void, Unit, REQ, RES, R>(ma.MapRight(Proxy.Pure<RT, Void, Unit, REQ, RES, R>)).ToServer();
}
