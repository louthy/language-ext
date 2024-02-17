using System;
using System.Diagnostics.Contracts;
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
public record Client<REQ, RES, M, A> : Proxy<REQ, RES, Unit, Void, M, A>
    where M : Monad<M>
{
    public readonly Proxy<REQ, RES, Unit, Void, M, A> Value;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value">Correctly shaped `Proxy` that represents a `Client`</param>
    public Client(Proxy<REQ, RES, Unit, Void, M, A> value) =>
        Value = value;
                
    /// <summary>
    /// Calling this will effectively cast the sub-type to the base.
    /// </summary>
    /// <remarks>This type wraps up a `Proxy` for convenience, and so it's a `Proxy` proxy.  So calling this method
    /// isn't exactly the same as a cast operation, as it unwraps the `Proxy` from within.  It has the same effect
    /// however, and removes a level of indirection</remarks>
    /// <returns>A general `Proxy` type from a more specialised type</returns>
    [Pure]
    public override Proxy<REQ, RES, Unit, Void, M, A> ToProxy() =>
        Value.ToProxy();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public override Proxy<REQ, RES, Unit, Void, M, S> Bind<S>(Func<A, Proxy<REQ, RES, Unit, Void, M, S>> f) =>
        Value.Bind(f);
  
    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public override Proxy<REQ, RES, Unit, Void, M, S> Map<S>(Func<A, S> f) =>
        Value.Map(f);
        
    /// <summary>
    /// Monadic bind operation, for chaining `Client` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Client<REQ, RES, M, B> Bind<B>(Func<A, Client<REQ, RES, M, B>> f) => 
        Value.Bind(f).ToClient();
        
    /// <summary>
    /// Monadic bind operation, for chaining `Client` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Client<REQ, RES, M, B> SelectMany<B>(Func<A, Client<REQ, RES, M, B>> f) => 
        Value.Bind(f).ToClient();
        
    /// <summary>
    /// Monadic bind operation, for chaining `Client` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Client<REQ, RES, M, C> SelectMany<B, C>(Func<A, Client<REQ, RES, M, B>> f, Func<A, B, C> project) => 
        Value.Bind(a => f(a).Map(b => project(a, b))).ToClient();
        
    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public new Client<REQ, RES, M, B> Select<B>(Func<A, B> f) => 
        Value.Map(f).ToClient();        
        
    /// <summary>
    /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
    /// </summary>
    /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
    /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
    /// processing of the computation</param>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the function provided</returns>
    [Pure]
    public override Proxy<REQ, RES, C1, C, M, A> For<C1, C>(Func<Void, Proxy<REQ, RES, C1, C, M, Unit>> body) =>
        Value.For(body);

    /// <summary>
    /// Applicative action
    ///
    /// Invokes this `Proxy`, then the `Proxy r` 
    /// </summary>
    /// <param name="r">`Proxy` to run after this one</param>
    [Pure]
    public override Proxy<REQ, RES, Unit, Void, M, S> Action<S>(Proxy<REQ, RES, Unit, Void, M, S> r) =>
        Value.Action(r);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
    /// </remarks>
    [Pure]
    public override Proxy<UOutA, AUInA, Unit, Void, M, A> PairEachRequestWithRespond<UOutA, AUInA>(Func<REQ, Proxy<UOutA, AUInA, REQ, RES, M, A>> lhs) =>
        Value.PairEachRequestWithRespond(lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<UOutA, AUInA, Unit, Void, M, A> ReplaceRequest<UOutA, AUInA>(Func<REQ, Proxy<UOutA, AUInA, Unit, Void, M, RES>> lhs) =>
        Value.ReplaceRequest(lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<REQ, RES, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(Func<Void, Proxy<Unit, Void, DInC, DOutC, M, A>> rhs) =>
        Value.PairEachRespondWithRequest(rhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<REQ, RES, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(Func<Void, Proxy<REQ, RES, DInC, DOutC, M, Unit>> rhs) =>
        Value.ReplaceRespond(rhs);

    /// <summary>
    /// Reverse the arrows of the `Proxy` to find its dual.  
    /// </summary>
    /// <returns>The dual of `this`</returns>
    [Pure]
    public override Proxy<Void, Unit, RES, REQ, M, A> Reflect() =>
        Value.Reflect();

    /// <summary>
    /// 
    ///     Observe(lift (Pure(r))) = Observe(Pure(r))
    ///     Observe(lift (m.Bind(f))) = Observe(lift(m.Bind(x => lift(f(x)))))
    /// 
    /// This correctness comes at a small cost to performance, so use this function sparingly.
    /// This function is a convenience for low-level pipes implementers.  You do not need to
    /// use observe if you stick to the safe API.        
    /// </summary>
    [Pure]
    public override Proxy<REQ, RES, Unit, Void, M, A> Observe() =>
        Value.Observe();

    [Pure]
    public void Deconstruct(out Proxy<REQ, RES, Unit, Void, M, A> value) =>
        value = Value;

    /// <summary>
    /// Compose a `Server` and a `Client` together into an `Effect`.  Note the `Server` is provided as a function
    /// that takes a value of `REQ`.  This is how we model the request coming into the `Server`.  The resulting
    /// `Server` computation can then call `Server.respond(response)` to reply to the `Client`.
    ///
    /// The `Client` simply calls `Client.request(req)` to post a request to the `Server`, it is like an `awaiting`
    /// that also posts.  It will await the response from the `Server`. 
    /// </summary>
    /// <param name="x">`Server`</param>
    /// <param name="y">`Client`</param>
    /// <returns>`Effect`</returns>
    [Pure]
    public static Effect<M, A> operator |(Func<REQ, Server<REQ, RES, M, A>> x, Client<REQ, RES, M, A> y) =>
        y.PairEachRequestWithRespond(x).ToEffect();
        
    /// <summary>
    /// Chain one client after another
    /// </summary>
    [Pure]
    public static Client<REQ, RES, M, A> operator &(
        Client<REQ, RES, M, A> lhs,
        Client<REQ, RES, M, A> rhs) =>
        lhs.Bind(_ => rhs);
        
}
