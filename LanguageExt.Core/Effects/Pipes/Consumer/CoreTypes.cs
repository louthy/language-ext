using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// Consumers both can only be `awaiting` 
/// </summary>
/// <remarks>
///       Upstream | Downstream
///           +---------+
///           |         |
///     Unit <==       <== Unit
///           |         |
///      IN  ==>       ==> Void
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public record Consumer<IN, M, A> : Proxy<Unit, IN, Unit, Void, M, A>
    where M : Monad<M>
{
    public readonly Proxy<Unit, IN, Unit, Void, M, A> Value;
        
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value">Correctly shaped `Proxy` that represents a `Consumer`</param>
    public Consumer(Proxy<Unit, IN, Unit, Void, M, A> value) =>
        Value = value;

    /// <summary>
    /// Calling this will effectively cast the sub-type to the base.
    /// </summary>
    /// <remarks>This type wraps up a `Proxy` for convenience, and so it's a `Proxy` proxy.  So calling this method
    /// isn't exactly the same as a cast operation, as it unwraps the `Proxy` from within.  It has the same effect
    /// however, and removes a level of indirection</remarks>
    /// <returns>A general `Proxy` type from a more specialised type</returns>
    [Pure]
    public override Proxy<Unit, IN, Unit, Void, M, A> ToProxy() =>
        Value.ToProxy();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public override Proxy<Unit, IN, Unit, Void, M, S> Bind<S>(Func<A, Proxy<Unit, IN, Unit, Void, M, S>> f) =>
        Value.Bind(f);

    /// <summary>
    /// Map the lifted monad
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public override Proxy<Unit, IN, Unit, Void, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        Value.MapM(f);
                    
    /// <summary>
    /// Extract the lifted IO monad (if there is one)
    /// </summary>
    /// <param name="f">The map function</param>
    /// <returns>A new `Proxy` that represents the innermost IO monad, if it exists.</returns>
    /// <exception cref="ExceptionalException">`Errors.UnliftIONotSupported` if there's no IO monad in the stack</exception>
    [Pure]
    public override Proxy<Unit, IN, Unit, Void, M, IO<A>> ToIO() =>
        Value.ToIO();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Consumer<IN, M, B> Bind<B>(Func<A, Consumer<IN, M, B>> f) => 
        Value.Bind(f).ToConsumer();
        
    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Consumer<IN, M, B> Bind<B>(Func<A, K<M, B>> f) => 
        Value.Bind(x => Consumer.lift<IN, M, B>(f(x))).ToConsumer();

    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public override Proxy<Unit, IN, Unit, Void, M, S> Map<S>(Func<A, S> f) =>
        Value.Map(f);
        
    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Consumer<IN, M, C> SelectMany<B, C>(Func<A, Consumer<IN, M, B>> f, Func<A, B, C> project) => 
        Value.Bind(a => f(a).Map(b => project(a, b))).ToConsumer();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Consumer<IN, M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> project) => 
        Value.Bind(x => Consumer.lift<IN, M, C>(M.Map(y => project(x, y), f(x)))).ToConsumer();
    
    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public new Consumer<IN, M, B> Select<B>(Func<A, B> f) => 
        Value.Map(f).ToConsumer();        

    /// <summary>
    /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
    /// </summary>
    /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
    /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
    /// processing of the computation</param>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the function provided</returns>
    [Pure]
    public override Proxy<Unit, IN, C1, C, M, A> For<C1, C>(Func<Void, Proxy<Unit, IN, C1, C, M, Unit>> body) =>
        Value.For(body);

    /// <summary>
    /// Applicative action
    ///
    /// Invokes this `Proxy`, then the `Proxy r` 
    /// </summary>
    /// <param name="r">`Proxy` to run after this one</param>
    [Pure]
    public override Proxy<Unit, IN, Unit, Void, M, S> Action<S>(Proxy<Unit, IN, Unit, Void, M, S> r) =>
        Value.Action(r);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
    /// </remarks>
    [Pure]
    public override Proxy<UOutA, AUInA, Unit, Void, M, A> PairEachRequestWithRespond<UOutA, AUInA>(
        Func<Unit, Proxy<UOutA, AUInA, Unit, IN, M, A>> lhs) =>
        Value.PairEachRequestWithRespond(lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<UOutA, AUInA, Unit, Void, M, A> ReplaceRequest<UOutA, AUInA>(
        Func<Unit, Proxy<UOutA, AUInA, Unit, Void, M, IN>> lhs) =>
        Value.ReplaceRequest(lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<Unit, IN, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<Void, Proxy<Unit, Void, DInC, DOutC, M, A>> rhs) =>
        Value.PairEachRespondWithRequest(rhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<Unit, IN, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(
        Func<Void, Proxy<Unit, IN, DInC, DOutC, M, Unit>> rhs) =>
        Value.ReplaceRespond(rhs);
        
    /// <summary>
    /// Reverse the arrows of the `Proxy` to find its dual.  
    /// </summary>
    /// <returns>The dual of `this`</returns>
    [Pure]
    public override Proxy<Void, Unit, IN, Unit, M, A> Reflect() =>
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
    public override Proxy<Unit, IN, Unit, Void, M, A> Observe() =>
        Value.Observe();

    [Pure]
    public void Deconstruct(out Proxy<Unit, IN, Unit, Void, M, A> value) =>
        value = Value;

    /// <summary>
    /// Conversion operator from the _pure_ `Consumer` type, to the monad transformer version of the `Consumer`
    /// </summary>
    /// <param name="p">Pure `Consumer`</param>
    /// <returns>Monad transformer version of the `Consumer`</returns>
    [Pure]
    public static implicit operator Consumer<IN, M, A>(Consumer<IN, A> c) =>
        c.Interpret<M>();

    /// <summary>
    /// Conversion operator from the `Pure` type, to the monad transformer version of the `Consumer`
    /// </summary>
    /// <param name="p">`Pure` value</param>
    /// <returns>Monad transformer version of the `Consumer`</returns>
    [Pure]
    public static implicit operator Consumer<IN, M, A>(Pure<A> p) =>
        Consumer.Pure<IN, M, A>(p.Value);

    /// <summary>
    /// Compose an `IN` and a `Consumer` together into an `Effect`.  This effectively creates a singleton `Producer`
    /// from the `IN` value, and composes it with the `Consumer` into an `Effect` that can be run. 
    /// </summary>
    /// <param name="p1">Single input value</param>
    /// <param name="p2">`Consumer`</param>
    /// <returns>`Effect`</returns>
    [Pure]
    public static Effect<M, A?> operator |(IN p1, Consumer<IN, M, A?> p2) => 
        Proxy.compose(Producer.yield<IN, M>(p1).Map(_ => default(A)), p2).ToEffect();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Consumer<IN, M, C> SelectMany<B, C>(Func<A, Consumer<IN, B>> bind, Func<A, B, C> project) =>
        Value.Bind(a =>  bind(a).Interpret<M>().Map(b => project(a, b))).ToConsumer();

    /// <summary>
    /// Chain one consumer's set of awaits after another
    /// </summary>
    [Pure]
    public static Consumer<IN, M, A> operator &(
        Consumer<IN, M, A> lhs,
        Consumer<IN, M, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    [Pure]
    public override string ToString() => 
        "consumer";
}
