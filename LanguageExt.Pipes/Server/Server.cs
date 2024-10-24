using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;

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
    public static Server<REQ, RES, M, R> Pure<REQ, RES, M, R>(R value) 
        where M : Monad<M> =>
        new Pure<Void, Unit, REQ, RES, M, R>(value).ToServer();
 
    /// <summary>
    /// Send a value of type `RES` downstream and block waiting for a reply of type `REQ`
    /// </summary>
    /// <remarks>
    /// `respond` is the identity of the respond category.
    /// </remarks>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<REQ, RES, M, REQ> respond<REQ, RES, M>(RES value) 
        where M : Monad<M> =>
        new Respond<Void, Unit, REQ, RES, M, REQ>(value, r => new Pure<Void, Unit, REQ, RES, M, REQ>(r)).ToServer();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<REQ, RES, M, R> lift<REQ, RES, M, R>(K<M, R> ma) 
        where M : Monad<M> =>
        new ProxyM<Void, Unit, REQ, RES, M, R>(M.Map(Proxy.Pure<Void, Unit, REQ, RES, M, R>, ma)).ToServer();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<REQ, RES, M, R> liftIO<REQ, RES, M, R>(IO<R> ma) 
        where M : Monad<M> =>
        lift<REQ, RES, M, R>(M.LiftIO(ma));
}
