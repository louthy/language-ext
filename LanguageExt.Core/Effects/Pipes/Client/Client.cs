using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// `Client` sends requests of type `REQ` and receives responses of type `RES`.
/// 
/// Clients only `request` and never `respond`.
/// </summary>
/// <remarks>
/// 
///       Upstream | Downstream
///           +---------+
///           |         |
///     REQ  <==       <== Unit
///           |         |
///     RES  ==>       ==> Void
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public class Client
{
    /// <summary>
    /// Monad return / pure
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<REQ, RES, M, R> Pure<REQ, RES, M, R>(R value)
        where M : Monad<M> =>
        new Pure<REQ, RES, Unit, Void, M, R>(value).ToClient();

    /// <summary>
    /// Send a value of type `RES` downstream and block waiting for a reply of type `REQ`
    /// </summary>
    /// <remarks>
    /// `respond` is the identity of the respond category.
    /// </remarks>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<REQ, RES, M, RES> request<REQ, M, RES>(REQ value)
        where M : Monad<M> =>
        new Request<REQ, RES, Unit, Void, M, RES>(value, r => new Pure<REQ, RES, Unit, Void, M, RES>(r)).ToClient();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<REQ, RES, M, R> lift<REQ, RES, M, R>(K<M, R> ma) 
        where M : Monad<M> =>
        new ProxyM<REQ, RES, Unit, Void, M, R>(M.Map(Proxy.Pure<REQ, RES, Unit, Void, M, R>, ma)).ToClient();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<REQ, RES, M, R> liftIO<REQ, RES, M, R>(IO<R> ma) 
        where M : Monad<M> =>
        new ProxyM<REQ, RES, Unit, Void, M, R>(M.Map(Proxy.Pure<REQ, RES, Unit, Void, M, R>, M.LiftIO(ma))).ToClient();
}
