using System;
using System.Diagnostics.Contracts;
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
public record Server<REQ, RES, M, A> : Proxy<Void, Unit, REQ, RES, M, A> 
    where M : Monad<M>
{
    public readonly Proxy<Void, Unit, REQ, RES, M, A> Value;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value">Correctly shaped `Proxy` that represents a `Server`</param>
    public Server(Proxy<Void, Unit, REQ, RES, M, A> value) =>
        Value = value;
        
    /// <summary>
    /// Calling this will effectively cast the sub-type to the base.
    /// </summary>
    /// <remarks>This type wraps up a `Proxy` for convenience, and so it's a `Proxy` proxy.  So calling this method
    /// isn't exactly the same as a cast operation, as it unwraps the `Proxy` from within.  It has the same effect
    /// however, and removes a level of indirection</remarks>
    /// <returns>A general `Proxy` type from a more specialised type</returns>
    [Pure]
    public override Proxy<Void, Unit, REQ, RES, M, A> ToProxy() =>
        Value.ToProxy();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public override Proxy<Void, Unit, REQ, RES, M, B> Bind<B>(Func<A, Proxy<Void, Unit, REQ, RES, M, B>> f) =>
        Value.Bind(f);
            
    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public override Proxy<Void, Unit, REQ, RES, M, B> Map<B>(Func<A, B> f) =>
        Value.Map(f);

    /// <summary>
    /// Map the lifted monad
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public override Proxy<Void, Unit, REQ, RES, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        Value.MapM(f);
        
    /// <summary>
    /// Monadic bind operation, for chaining `Server` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Server<REQ, RES, M, B> Bind<B>(Func<A, Server<REQ, RES, M, B>> f) => 
        Value.Bind(f).ToServer();
        
    /// <summary>
    /// Monadic bind operation, for chaining `Server` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Server<REQ, RES, M, B> SelectMany<B>(Func<A, Server<REQ, RES, M, B>> f) => 
        Value.Bind(f).ToServer();
        
    /// <summary>
    /// Monadic bind operation, for chaining `Server` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Server<REQ, RES, M, C> SelectMany<B, C>(Func<A, Server<REQ, RES, M, B>> f, Func<A, B, C> project) => 
        Value.Bind(a => f(a).Map(b => project(a, b))).ToServer();
        
    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public new Server<REQ, RES, M, B> Select<B>(Func<A, B> f) => 
        Value.Map(f).ToServer();
        
    /// <summary>
    /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
    /// </summary>
    /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
    /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
    /// processing of the computation</param>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the function provided</returns>
    [Pure]
    public override Proxy<Void, Unit, C1, C, M, A> For<C1, C>(Func<RES, Proxy<Void, Unit, C1, C, M, REQ>> body) =>
        Value.For(body);

    /// <summary>
    /// Applicative action
    ///
    /// Invokes this `Proxy`, then the `Proxy r` 
    /// </summary>
    /// <param name="r">`Proxy` to run after this one</param>
    [Pure]
    public override Proxy<Void, Unit, REQ, RES, M, S> Action<S>(Proxy<Void, Unit, REQ, RES, M, S> r) =>
        Value.Action(r);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
    /// </remarks>
    [Pure]
    public override Proxy<UOutA, AUInA, REQ, RES, M, A> PairEachRequestWithRespond<UOutA, AUInA>(
        Func<Void, Proxy<UOutA, AUInA, Void, Unit, M, A>> lhs) =>
        Value.PairEachRequestWithRespond(lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<UOutA, AUInA, REQ, RES, M, A> ReplaceRequest<UOutA, AUInA>(
        Func<Void, Proxy<UOutA, AUInA, REQ, RES, M, Unit>> lhs) =>
        Value.ReplaceRequest(lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<Void, Unit, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<RES, Proxy<REQ, RES, DInC, DOutC, M, A>> rhs) =>
        Value.PairEachRespondWithRequest(rhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<Void, Unit, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(
        Func<RES, Proxy<Void, Unit, DInC, DOutC, M, REQ>> rhs) =>
        Value.ReplaceRespond(rhs);

    /// <summary>
    /// Reverse the arrows of the `Proxy` to find its dual.  
    /// </summary>
    /// <returns>The dual of `this`</returns>
    [Pure]
    public override Proxy<RES, REQ, Unit, Void, M, A> Reflect() =>
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
    public override Proxy<Void, Unit, REQ, RES, M, A> Observe() =>
        Value.Observe();

    [Pure]
    public void Deconstruct(out Proxy<Void, Unit, REQ, RES, M, A> value) =>
        value = Value;

    /// <summary>
    /// Chain one server after another
    /// </summary>
    [Pure]
    public static Server<REQ, RES, M, A> operator &(
        Server<REQ, RES, M, A> lhs,
        Server<REQ, RES, M, A> rhs) =>
        lhs.Bind(_ => rhs);
}
