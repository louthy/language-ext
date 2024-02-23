using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

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
public abstract record Proxy<UOut, UIn, DIn, DOut, M, A> : K<Proxy<UOut, UIn, DIn, DOut, M>, A>
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
    
    [Pure]
    public override string ToString() => 
        "proxy";
}
