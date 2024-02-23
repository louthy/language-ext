using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Traits;
using static LanguageExt.Transducer;

namespace LanguageExt.Pipes;

/// <summary>
/// A `Proxy` is a monad transformer that receives and sends information on both
/// an upstream and downstream interface.  It is the base type for all of the key
/// other important types in the Pipes ecosystem, like `Producer`, `Consumer`,
/// `Pipe`, etc.
/// 
/// Diagrammatically, you can think of a `Proxy` as having the following shape:
/// 
///         Upstream | Downstream
///             +---------+
///             |         |
///       UOut ◄--       ◄-- DIn
///             |         |
///       UIn  --►       --► DOut
///             |    |    |
///             +----|----+
///                  A
///
/// You can connect proxies together in five different ways:
/// 
///   1. Connect pull-based streams
///   2. Connect push-based streams
///   3. Chain folds
///   4. Chain unfolds
///   5. Sequence proxies
/// 
/// The type variables signify:
///
/// * `UOut` and `Uin` - The upstream interface, where `UOut` go out and `UIn` come in
/// * `DOut` and `DIn` - The downstream interface, where `DOut` go out and `DIn` come in
/// * `RT` - The runtime of the transformed effect monad
/// * `A` - The return value    
/// </summary>
/// <typeparam name="UOut">Upstream out type</typeparam>
/// <typeparam name="UIn">Upstream in type</typeparam>
/// <typeparam name="DIn">Downstream in type</typeparam>
/// <typeparam name="DOut">Downstream uut type</typeparam>
/// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
/// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
///
/// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
public abstract record Proxy<UOut, UIn, DIn, DOut, M, A> //: K<M, A>
    where M : Monad<M>
{
    /// <summary>
    /// When working with sub-types, like `Producer`, calling this will effectively cast the sub-type to the base.
    /// </summary>
    /// <returns>A general `Proxy` type from a more specialised type</returns>
    public abstract Proxy<UOut, UIn, DIn, DOut, M, A> ToProxy();
        
    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    public abstract Proxy<UOut, UIn, DIn, DOut, M, B> Bind<B>(Func<A, Proxy<UOut, UIn, DIn, DOut, M, B>> f);
        
    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    public abstract Proxy<UOut, UIn, DIn, DOut, M, B> Map<B>(Func<A, B> f);
        
    /// <summary>
    /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
    /// </summary>
    /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
    /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
    /// processing of the computation</param>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the function provided</returns>
    public abstract Proxy<UOut, UIn, C1, C, M, A> For<C1, C>(Func<DOut, Proxy<UOut, UIn, C1, C, M, DIn>> body);
        
    /// <summary>
    /// Applicative action
    ///
    /// Invokes this `Proxy`, then the `Proxy r` 
    /// </summary>
    /// <param name="r">`Proxy` to run after this one</param>
    public abstract Proxy<UOut, UIn, DIn, DOut, M, B> Action<B>(Proxy<UOut, UIn, DIn, DOut, M, B> r);
        
    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
    /// </remarks>
    public abstract Proxy<UOutA, AUInA, DIn, DOut, M, A> PairEachRequestWithRespond<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, UOut, UIn, M, A>> lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    public abstract Proxy<UOutA, AUInA, DIn, DOut, M, A> ReplaceRequest<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, DIn, DOut, M, UIn>> lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    public abstract Proxy<UOut, UIn, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<DOut, Proxy<DIn, DOut, DInC, DOutC, M, A>> rhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    public abstract Proxy<UOut, UIn, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(
        Func<DOut, Proxy< UOut, UIn, DInC, DOutC, M, DIn>> rhs);

    /// <summary>
    /// Reverse the arrows of the `Proxy` to find its dual.  
    /// </summary>
    /// <returns>The dual of `this`</returns>
    public abstract Proxy<DOut, DIn, UIn, UOut, M, A> Reflect();
        
    /// <summary>
    /// 
    ///     Observe(lift (Pure(r))) = Observe(Pure(r))
    ///     Observe(lift (m.Bind(f))) = Observe(lift(m.Bind(x => lift(f(x)))))
    /// 
    /// This correctness comes at a small cost to performance, so use this function sparingly.
    /// This function is a convenience for low-level pipes implementers.  You do not need to
    /// use observe if you stick to the safe API.        
    /// </summary>
    public abstract Proxy<UOut, UIn, DIn, DOut, M, A> Observe();

    /// <summary>
    /// Monadic bind operation, followed by a mapping projection, enables usage in LINQ expressions
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <param name="prject">The mapping projection function</param>
    /// <typeparam name="C">The new bound value type</typeparam>
    /// <returns>The result of the bind and mapping composition</returns>
    [Pure]
    public Proxy<UOut, UIn, DIn, DOut, M, C> SelectMany<B, C>(
        Func<A, Proxy<UOut, UIn, DIn, DOut, M, B>> f, 
        Func<A, B, C> project) =>
        Bind(a => f(a).Map(b => project(a, b)));

    /// <summary>
    /// Functor map operation, enables usage in LINQ expressions
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="B">The new bound value type</typeparam>
    /// <returns>The result of the lifted function applied to the bound value</returns>
    [Pure]
    public Proxy<UOut, UIn, DIn, DOut, M, B> Select<B>(Func<A, B> f) =>
        Map(f);
}
    
/// <summary>
/// One of the algebraic cases of the `Proxy` type.  This type represents a pure value.  It can be thought of as the
/// terminating value of the computation, as there's not continuation attached to this case. 
/// </summary>
/// <typeparam name="UOut">Upstream out type</typeparam>
/// <typeparam name="UIn">Upstream in type</typeparam>
/// <typeparam name="DIn">Downstream in type</typeparam>
/// <typeparam name="DOut">Downstream uut type</typeparam>
/// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
/// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
///
/// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
public record Pure<UOut, UIn, DIn, DOut, M, A>(A Value) : Proxy<UOut, UIn, DIn, DOut, M, A>
    where M : Monad<M>
{
    /// <summary>
    /// When working with sub-types, like `Producer`, calling this will effectively cast the sub-type to the base.
    /// </summary>
    /// <returns>A general `Proxy` type from a more specialised type</returns>
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, A> ToProxy() => this;

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, B> Bind<B>(Func<A, Proxy<UOut, UIn, DIn, DOut, M, B>> f) =>
        f(Value);

    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, B> Map<B>(Func<A, B> f) =>
        new Pure<UOut, UIn, DIn, DOut, M, B>(f(Value));

    /// <summary>
    /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
    /// </summary>
    /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
    /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
    /// processing of the computation</param>
    /// <returns></returns>
    [Pure]
    public override Proxy<UOut, UIn, C1, C, M, A> For<C1, C>(Func<DOut, Proxy<UOut, UIn, C1, C, M, DIn>> body) =>
        ReplaceRespond(body);

    /// <summary>
    /// Applicative action
    ///
    /// Invokes this `Proxy`, then the `Proxy r` 
    /// </summary>
    /// <param name="r">`Proxy` to run after this one</param>
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, B> Action<B>(Proxy<UOut, UIn, DIn, DOut, M, B> r) =>
        r;

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
    /// </remarks>
    [Pure]
    public override Proxy<UOutA, AUInA, DIn, DOut, M, A> PairEachRequestWithRespond<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, UOut, UIn, M, A>> _) =>
        new Pure<UOutA, AUInA, DIn, DOut, M, A>(Value);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<UOutA, AUInA, DIn, DOut, M, A> ReplaceRequest<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, DIn, DOut, M, UIn>> _) =>
        new Pure<UOutA, AUInA, DIn, DOut, M, A>(Value).ToProxy();
                
    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<UOut, UIn, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<DOut, Proxy<DIn, DOut, DInC, DOutC, M, A>> _) =>
        new Pure<UOut, UIn, DInC, DOutC, M, A>(Value);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<UOut, UIn, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(
        Func<DOut, Proxy<UOut, UIn, DInC, DOutC, M, DIn>> _) =>
        new Pure<UOut, UIn, DInC, DOutC, M, A>(Value);

    /// <summary>
    /// Reverse the arrows of the `Proxy` to find its dual.  
    /// </summary>
    /// <returns>The dual of `this1</returns>
    [Pure]
    public override Proxy<DOut, DIn, UIn, UOut, M, A> Reflect() =>
        new Pure<DOut, DIn, UIn, UOut, M, A>(Value);

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
    public override Proxy<UOut, UIn, DIn, DOut, M, A> Observe() =>
        new ProxyM<UOut, UIn, DIn, DOut, M, A>(M.Pure(Proxy.Pure<UOut, UIn, DIn, DOut, M, A>(Value)));
        

    [Pure]
    public void Deconstruct(out A value) =>
        value = Value;
}

/// <summary>
/// One of the algebraic cases of the `Proxy` type.  This type lifts a `Transducer` computation into the
/// `Proxy` monad-transformer.  This is how the `Proxy` system can cause real-world effects.
/// </summary>
/// <typeparam name="UOut">Upstream out type</typeparam>
/// <typeparam name="UIn">Upstream in type</typeparam>
/// <typeparam name="DIn">Downstream in type</typeparam>
/// <typeparam name="DOut">Downstream uut type</typeparam>
/// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
/// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
///
/// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
internal record ProxyM<UOut, UIn, DIn, DOut, M, A>(K<M, Proxy<UOut, UIn, DIn, DOut, M, A>> Value) : 
    Proxy<UOut, UIn, DIn, DOut, M, A>
    where M : Monad<M>
{
    /// <summary>
    /// When working with sub-types, like `Producer`, calling this will effectively cast the sub-type to the base.
    /// </summary>
    /// <returns>A general `Proxy` type from a more specialised type</returns>
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, A> ToProxy() => 
        this;

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, S> Bind<S>(Func<A, Proxy<UOut, UIn, DIn, DOut, M, S>> f) =>
        new ProxyM<UOut, UIn, DIn, DOut, M, S>(M.Map(p => p.Bind(f), Value));

    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, S> Map<S>(Func<A, S> f) =>
        new ProxyM<UOut, UIn, DIn, DOut, M, S>(M.Map(p => p.Map(f), Value));

    /// <summary>
    /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
    /// </summary>
    /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
    /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
    /// processing of the computation</param>
    /// <returns></returns>
    [Pure]
    public override Proxy<UOut, UIn, C1, C, M, A> For<C1, C>(Func<DOut, Proxy<UOut, UIn, C1, C, M, DIn>> body) =>
        ReplaceRespond(body);

    /// <summary>
    /// Applicative action
    ///
    /// Invokes this `Proxy`, then the `Proxy r` 
    /// </summary>
    /// <param name="r">`Proxy` to run after this one</param>
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, S> Action<S>(Proxy<UOut, UIn, DIn, DOut, M, S> r) =>
        new ProxyM<UOut, UIn, DIn, DOut, M, S>(M.Map(p => p.Action(r), Value));

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `fb1`.
    /// </remarks>
    [Pure]
    public override Proxy<UOutA, AUInA, DIn, DOut, M, A> PairEachRequestWithRespond<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, UOut, UIn, M, A>> fb1) =>
        new ProxyM<UOutA, AUInA, DIn, DOut, M, A>(M.Map(p1 => p1.PairEachRequestWithRespond(fb1), Value));

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<UOutA, AUInA, DIn, DOut, M, A> ReplaceRequest<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, DIn, DOut, M, UIn>> lhs) =>
        new ProxyM<UOutA, AUInA, DIn, DOut, M, A>(M.Map(p => p.ReplaceRequest(lhs), Value));

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<UOut, UIn, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<DOut, Proxy<DIn, DOut, DInC, DOutC, M, A>> rhs) =>
        new ProxyM<UOut, UIn, DInC, DOutC, M, A>(M.Map(p1 => p1.PairEachRespondWithRequest(rhs), Value));

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<UOut, UIn, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(
        Func<DOut, Proxy<UOut, UIn, DInC, DOutC, M, DIn>> rhs) =>
        new ProxyM<UOut, UIn, DInC, DOutC, M, A>(M.Map(p => p.ReplaceRespond(rhs), Value));

    /// <summary>
    /// Reverse the arrows of the `Proxy` to find its dual.  
    /// </summary>
    /// <returns>The dual of `this1</returns>
    [Pure]
    public override Proxy<DOut, DIn, UIn, UOut, M, A> Reflect() =>
        new ProxyM<DOut, DIn, UIn, UOut, M, A>(M.Map(p => p.Reflect(), Value));

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
    public override Proxy<UOut, UIn, DIn, DOut, M, A> Observe() =>
        new ProxyM<UOut, UIn, DIn, DOut, M, A>(M.Map(p => p.Observe(), Value));
        
    [Pure]
    public void Deconstruct(out K<M, Proxy<UOut, UIn, DIn, DOut, M, A>> value) =>
        value = Value;
}
