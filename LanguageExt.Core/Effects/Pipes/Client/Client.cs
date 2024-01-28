using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

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
    public static Client<RT, REQ, RES, R> Pure<RT, REQ, RES, R>(R value) where RT : HasIO<RT, Error> =>
        new Pure<RT, REQ, RES, Unit, Void, R>(value).ToClient();

    /// <summary>
    /// Send a value of type `RES` downstream and block waiting for a reply of type `REQ`
    /// </summary>
    /// <remarks>
    /// `respond` is the identity of the respond category.
    /// </remarks>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<RT, REQ, RES, RES> request<RT, REQ, RES>(REQ value) where RT : HasIO<RT, Error> =>
        new Request<RT, REQ, RES, Unit, Void, RES>(value, r => new Pure<RT, REQ, RES, Unit, Void, RES>(r)).ToClient();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Eff<R> ma) where RT : HasIO<RT, Error> =>
        new M<RT, REQ, RES, Unit, Void, R>(ma.WithRuntime<RT>().Morphism.Map(mx => mx.Map(Proxy.Pure<RT, REQ, RES, Unit, Void, R>))).ToClient();

    /// <summary>
    /// Lift am IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Eff<RT, R> ma) where RT : HasIO<RT, Error> =>
        new M<RT, REQ, RES, Unit, Void, R>(ma.Morphism.Map(mx => mx.Map(Proxy.Pure<RT, REQ, RES, Unit, Void, R>))).ToClient();
}
