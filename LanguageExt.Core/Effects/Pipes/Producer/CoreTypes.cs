using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// Producers can only `yield`
/// </summary>
/// <remarks>
///       Upstream | Downstream
///           +---------+
///           |         |
///     Void <==       <== Unit
///           |         |
///     Unit ==>       ==> OUT
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public class Producer<RT, OUT, A> : Proxy<RT, Void, Unit, Unit, OUT, A> where RT : HasIO<RT, Error>
{
    public readonly Proxy<RT, Void, Unit, Unit, OUT, A> Value;
        
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value">Correctly shaped `Proxy` that represents a `Producer`</param>
    public Producer(Proxy<RT, Void, Unit, Unit, OUT, A> value) =>
        Value = value;

    /// <summary>
    /// Calling this will effectively cast the sub-type to the base.
    /// </summary>
    /// <remarks>This type wraps up a `Proxy` for convenience, and so it's a `Proxy` proxy.  So calling this method
    /// isn't exactly the same as a cast operation, as it unwraps the `Proxy` from within.  It has the same effect
    /// however, and removes a level of indirection</remarks>
    /// <returns>A general `Proxy` type from a more specialised type</returns>
    [Pure]
    public override Proxy<RT, Void, Unit, Unit, OUT, A> ToProxy() =>
        Value.ToProxy();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public override Proxy<RT, Void, Unit, Unit, OUT, S> Bind<S>(Func<A, Proxy<RT, Void, Unit, Unit, OUT, S>> f) =>
        Value.Bind(f);

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, Transducer<RT, B>> f) =>
        Value.Bind(x => Producer.lift<RT, OUT, B>(f(x))).ToProducer();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, Transducer<RT, Sum<Error, B>>> f) =>
        Value.Bind(x => Producer.lift<RT, OUT, B>(f(x))).ToProducer();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, Transducer<Unit, B>> f) =>
        Value.Bind(x => Producer.lift<RT, OUT, B>(f(x))).ToProducer();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, Transducer<Unit, Sum<Error, B>>> f) =>
        Value.Bind(x => Producer.lift<RT, OUT, B>(f(x))).ToProducer();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, Eff<B>> f) =>
        Value.Bind(x => Producer.lift<RT, OUT, B>(f(x))).ToProducer();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, Eff<RT, B>> f) =>
        Value.Bind(x => Producer.lift<RT, OUT, B>(f(x))).ToProducer();

    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public override Proxy<RT, Void, Unit, Unit, OUT, B> Map<B>(Func<A, B> f) =>
        Value.Map(f);
        
    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, Producer<RT, OUT, B>> f) => 
        Value.Bind(f).ToProducer();
        
    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<RT, OUT, B>> f) => 
        Value.Bind(f).ToProducer();
        
    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) => 
        Value.Bind(a => f(a).Map(b => project(a, b))).ToProducer();
        
    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public new Producer<RT, OUT, B> Select<B>(Func<A, B> f) => 
        Value.Map(f).ToProducer();

    /// <summary>
    /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
    /// </summary>
    /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
    /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
    /// processing of the computation</param>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the function provided</returns>
    [Pure]
    public override Proxy<RT, Void, Unit, C1, C, A> For<C1, C>(Func<OUT, Proxy<RT, Void, Unit, C1, C, Unit>> body) =>
        Value.ReplaceRespond(body);

    /// <summary>
    /// Applicative action
    ///
    /// Invokes this `Proxy`, then the `Proxy r` 
    /// </summary>
    /// <param name="r">`Proxy` to run after this one</param>
    [Pure]
    public override Proxy<RT, Void, Unit, Unit, OUT, B> Action<B>(Proxy<RT, Void, Unit, Unit, OUT, B> r) =>
        Value.Action(r);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
    /// </remarks>
    [Pure]
    public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> PairEachRequestWithRespond<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, A>> lhs) =>
        Value.PairEachRequestWithRespond(lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ReplaceRequest<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Unit, OUT, Unit>> lhs) =>
        Value.ReplaceRequest(lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<RT, Void, Unit, DInC, DOutC, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<OUT, Proxy<RT, Unit, OUT, DInC, DOutC, A>> rhs) =>
        Value.PairEachRespondWithRequest(rhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<RT, Void, Unit, DInC, DOutC, A> ReplaceRespond<DInC, DOutC>(
        Func<OUT, Proxy<RT, Void, Unit, DInC, DOutC, Unit>> rhs) =>
        Value.ReplaceRespond(rhs);

    /// <summary>
    /// Reverse the arrows of the `Proxy` to find its dual.  
    /// </summary>
    /// <returns>The dual of `this`</returns>
    [Pure]
    public override Proxy<RT, OUT, Unit, Unit, Void, A> Reflect() =>
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
    public override Proxy<RT, Void, Unit, Unit, OUT, A> Observe() =>
        Value.Observe();

    /// <summary>
    /// Compose a `Producer` and a `Consumer` together into an `Effect`.  
    /// </summary>
    /// <param name="p1">`Producer`</param>
    /// <param name="p2">`Consumer`</param>
    /// <returns>`Effect`</returns>
    [Pure]
    public static Effect<RT, A> operator |(Producer<RT, OUT, A> p1, Consumer<RT, OUT, A> p2) => 
        Proxy.compose(p1, p2);
        
    /// <summary>
    /// Compose a `Producer` and a `Consumer` together into an `Effect`.  
    /// </summary>
    /// <param name="p1">`Producer`</param>
    /// <param name="p2">`Consumer`</param>
    /// <returns>`Effect`</returns>
    [Pure]
    public static Effect<RT, A> operator |(Producer<RT, OUT, A> p1, Consumer<OUT, A> p2) => 
        Proxy.compose(p1, p2);
        
    [Pure]
    public void Deconstruct(out Proxy<RT, Void, Unit, Unit, OUT, A> value) =>
        value = Value;

    /// <summary>
    /// Conversion operator from the _pure_ `Producer` type, to the monad transformer version of the `Producer`
    /// </summary>
    /// <param name="p">Pure `Producer`</param>
    /// <returns>Monad transformer version of the `Producer`</returns>
    [Pure]
    public static implicit operator Producer<RT, OUT, A>(Producer<OUT, A> p) =>
        p.Interpret<RT>();

    /// <summary>
    /// Conversion operator from the `Pure` type, to the monad transformer version of the `Producer`
    /// </summary>
    /// <param name="p">`Pure` value</param>
    /// <returns>Monad transformer version of the `Producer`</returns>
    [Pure]
    public static implicit operator Producer<RT, OUT, A>(Pure<A> p) =>
        Producer.Pure<RT, OUT, A>(p.Value);

    /// <summary>
    /// Conversion operator from the `Pure` type, to the monad transformer version of the `Producer`
    /// </summary>
    /// <param name="t">Transducer</param>
    /// <returns>Monad transformer version of the `Producer`</returns>
    [Pure]
    public static implicit operator Producer<RT, OUT, A>(Transducer<RT, A> t) =>
        Producer.lift<RT, OUT, A>(t);

    /// <summary>
    /// Conversion operator from the `Pure` type, to the monad transformer version of the `Producer`
    /// </summary>
    /// <param name="t">Transducer</param>
    /// <returns>Monad transformer version of the `Producer`</returns>
    [Pure]
    public static implicit operator Producer<RT, OUT, A>(Transducer<RT, Sum<Error, A>> t) =>
        Producer.lift<RT, OUT, A>(t);

    /// <summary>
    /// Conversion operator from the `Pure` type, to the monad transformer version of the `Producer`
    /// </summary>
    /// <param name="t">Transducer</param>
    /// <returns>Monad transformer version of the `Producer`</returns>
    [Pure]
    public static implicit operator Producer<RT, OUT, A>(Transducer<Unit, A> t) =>
        Producer.lift<RT, OUT, A>(t);

    /// <summary>
    /// Conversion operator from the `Pure` type, to the monad transformer version of the `Producer`
    /// </summary>
    /// <param name="t">Transducer</param>
    /// <returns>Monad transformer version of the `Producer`</returns>
    [Pure]
    public static implicit operator Producer<RT, OUT, A>(Transducer<Unit, Sum<Error, A>> t) =>
        Producer.lift<RT, OUT, A>(t);

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> bind) =>
        Value.Bind(a => bind(a).Interpret<RT>()).ToProducer();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> bind, Func<A, B, C> project) =>
        SelectMany(a => bind(a).Select(b => project(a, b)));

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Transducer<RT, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Transducer<Unit, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Transducer<RT, Sum<Error, B>>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(my => my.Map(y => project(x, y))));

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Transducer<Unit, Sum<Error, B>>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(my => my.Map(y => project(x, y))));

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Eff<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Chain one producer's set of yields after another
    /// </summary>
    [Pure]
    public static Producer<RT, OUT, A> operator &(
        Producer<RT, OUT, A> lhs,
        Producer<RT, OUT, A> rhs) =>
        lhs.Bind(_ => rhs);
}
