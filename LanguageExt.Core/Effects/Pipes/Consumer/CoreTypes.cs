using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Pipes
{
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
    public class Consumer<RT, IN, A> : Proxy<RT, Unit, IN, Unit, Void, A>  where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, IN, Unit, Void, A> Value;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Correctly shaped `Proxy` that represents a `Consumer`</param>
        public Consumer(Proxy<RT, Unit, IN, Unit, Void, A> value) =>
            Value = value;

        /// <summary>
        /// Calling this will effectively cast the sub-type to the base.
        /// </summary>
        /// <remarks>This type wraps up a `Proxy` for convenience, and so it's a `Proxy` proxy.  So calling this method
        /// isn't exactly the same as a cast operation, as it unwraps the `Proxy` from within.  It has the same effect
        /// however, and removes a level of indirection</remarks>
        /// <returns>A general `Proxy` type from a more specialised type</returns>
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, A> ToProxy() =>
            Value.ToProxy();

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Bind<S>(Func<A, Proxy<RT, Unit, IN, Unit, Void, S>> f) =>
            Value.Bind(f);
        
        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Consumer<RT, IN, B> Bind<B>(Func<A, Consumer<RT, IN, B>> f) => 
            Value.Bind(f).ToConsumer();

        /// <summary>
        /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
        /// </summary>
        /// <param name="f">The map function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);
        
        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, Consumer<RT, IN, B>> f) => 
            Value.Bind(f).ToConsumer();
        
        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) => 
            Value.Bind(a => f(a).Map(b => project(a, b))).ToConsumer();
        
        /// <summary>
        /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
        /// </summary>
        /// <param name="f">The map function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
        [Pure]
        public new Consumer<RT, IN, B> Select<B>(Func<A, B> f) => 
            Value.Map(f).ToConsumer();        

        /// <summary>
        /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
        /// </summary>
        /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
        /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
        /// processing of the computation</param>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the function provided</returns>
        [Pure]
        public override Proxy<RT, Unit, IN, C1, C, A> For<C1, C>(Func<Void, Proxy<RT, Unit, IN, C1, C, Unit>> body) =>
            Value.For(body);

        /// <summary>
        /// Applicative action
        ///
        /// Invokes this `Proxy`, then the `Proxy r` 
        /// </summary>
        /// <param name="r">`Proxy` to run after this one</param>
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Action<S>(Proxy<RT, Unit, IN, Unit, Void, S> r) =>
            Value.Action(r);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, IN, A>> lhs) =>
            Value.ComposeRight(lhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, Void, IN>> lhs) =>
            Value.ComposeRight(lhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, Void, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, IN, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);
        
        /// <summary>
        /// Reverse the arrows of the `Proxy` to find its dual.  
        /// </summary>
        /// <returns>The dual of `this`</returns>
        [Pure]
        public override Proxy<RT, Void, Unit, IN, Unit, A> Reflect() =>
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
        public override Proxy<RT, Unit, IN, Unit, Void, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Unit, IN, Unit, Void, A> value) =>
            value = Value;

        /// <summary>
        /// Conversion operator from the _pure_ `Consumer` type, to the `Aff` monad transformer version of the `Consumer`
        /// </summary>
        /// <param name="p">Pure `Consumer`</param>
        /// <returns>Monad transformer version of the `Consumer` that supports `Aff`</returns>
        [Pure]
        public static implicit operator Consumer<RT, IN, A>(Consumer<IN, A> c) =>
            c.Interpret<RT>();

        /// <summary>
        /// Conversion operator from the `ConsumerLift` type, to the `Aff` monad transformer version of the `Consumer`
        /// </summary>
        /// <param name="p">`ConsumerLift` which represents a `Consumer` that has also had an `Aff` operation lifted into it</param>
        /// <returns>Monad transformer version of the `Consumer` that supports `Aff`</returns>
        [Pure]
        public static implicit operator Consumer<RT, IN, A>(ConsumerLift<RT, IN, A> c) =>
            c.Interpret();

        /// <summary>
        /// Conversion operator from the `Pure` type, to the `Aff` monad transformer version of the `Producer`
        /// </summary>
        /// <param name="p">`Pure` value</param>
        /// <returns>Monad transformer version of the `Producer` that supports `Aff`</returns>
        [Pure]
        public static implicit operator Consumer<RT, IN, A>(Pure<A> p) =>
            Consumer.Pure<RT, IN, A>(p.Value);

        /// <summary>
        /// Compose an `IN` and a `Consumer` together into an `Effect`.  This effectively creates a singleton `Producer`
        /// from the `IN` value, and composes it with the `Consumer` into an `Effect` that can be run. 
        /// </summary>
        /// <param name="p1">Single input value</param>
        /// <param name="p2">`Consumer`</param>
        /// <returns>`Effect`</returns>
        [Pure]
        public static Effect<RT, A> operator |(IN p1, Consumer<RT, IN, A> p2) => 
            Proxy.compose(Producer.yield<RT, IN>(p1).Map(_ => default(A)), p2).ToEffect();

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="bind">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretConsumer<RT, IN>()).ToConsumer();

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="bind">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
        
        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, Consumer<IN, B>> bind) =>
            Value.Bind(a =>  bind(a).Interpret<RT>()).ToConsumer();

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Consumer<IN, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, ConsumerLift<RT, IN, B>> bind) =>
            Value.Bind(a =>  bind(a).Interpret()).ToConsumer();

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, ConsumerLift<RT, IN, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
    }
}
