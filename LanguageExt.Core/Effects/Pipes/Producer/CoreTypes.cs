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
    public class Producer<RT, OUT, A> : Proxy<RT, Void, Unit, Unit, OUT, A> where RT : struct, HasCancel<RT>
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
            Value.For(body);

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
        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, A>> lhs) =>
            Value.ComposeRight(lhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Unit, OUT, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, OUT, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Void, Unit, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

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
        /// Conversion operator from the _pure_ `Producer` type, to the `Aff` monad transformer version of the `Producer`
        /// </summary>
        /// <param name="p">Pure `Producer`</param>
        /// <returns>Monad transformer version of the `Producer` that supports `Aff`</returns>
        [Pure]
        public static implicit operator Producer<RT, OUT, A>(Producer<OUT, A> p) =>
            p.Interpret<RT>();

        /// <summary>
        /// Conversion operator from the `ProducerLift` type, to the `Aff` monad transformer version of the `Producer`
        /// </summary>
        /// <param name="p">`ProducerLift` which represents a `Producer` that has also had an `Aff` operation lifted into it</param>
        /// <returns>Monad transformer version of the `Producer` that supports `Aff`</returns>
        [Pure]
        public static implicit operator Producer<RT, OUT, A>(ProducerLift<RT, OUT, A> p) =>
            p.Interpret();

        /// <summary>
        /// Conversion operator from the `Pure` type, to the `Aff` monad transformer version of the `Producer`
        /// </summary>
        /// <param name="p">`Pure` value</param>
        /// <returns>Monad transformer version of the `Producer` that supports `Aff`</returns>
        [Pure]
        public static implicit operator Producer<RT, OUT, A>(Pure<A> p) =>
            Producer.Pure<RT, OUT, A>(p.Value);

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Producer<RT, OUT, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretProducer<RT, OUT>()).ToProducer();

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));

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
        /// Merge two producers together into one.  This is effectively like getting to producing streams and merging
        /// them into one stream.  If you're doing this with many producers it's more efficient to call `Producer.merge(...)`
        /// with many `Producers` than just summing them.  If you're merging 2 or 3, then this is fine.
        /// </summary>
        /// <param name="ma">First `Producer` to merge</param>
        /// <param name="mb">Second `Producer` to merge</param>
        /// <returns>A new `Producer` with both inputs merged</returns>
        [Pure]
        public static Producer<RT, OUT, A> operator +(Producer<RT, OUT, A> ma, Producer<RT, OUT, A> mb) =>
            Producer.merge<RT, OUT, A>(ma, mb);
    }
}
