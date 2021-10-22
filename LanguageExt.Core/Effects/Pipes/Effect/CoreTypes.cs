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
    /// Effects represent a 'fused' set of producer, pipes, and consumer into one type.
    /// 
    /// It neither can neither `yield` nor be `awaiting`, it represents an entirely closed effect system.
    /// </summary>
    /// <remarks>
    /// 
    ///       Upstream | Downstream
    ///           +---------+
    ///           |         |
    ///     Void <==       <== Unit
    ///           |         |
    ///     Unit ==>       ==> Void
    ///           |    |    |
    ///           +----|----+
    ///                |
    ///                A
    /// 
    /// </remarks>
    public class Effect<RT, A> : Proxy<RT, Void, Unit, Unit, Void, A> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Void, Unit, Unit, Void, A> Value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Correctly shaped `Proxy` that represents an `Effect`</param>
        public Effect(Proxy<RT, Void, Unit, Unit, Void, A> value) =>
            Value = value;
        
        /// <summary>
        /// Calling this will effectively cast the sub-type to the base.
        /// </summary>
        /// <remarks>This type wraps up a `Proxy` for convenience, and so it's a `Proxy` proxy.  So calling this method
        /// isn't exactly the same as a cast operation, as it unwraps the `Proxy` from within.  It has the same effect
        /// however, and removes a level of indirection</remarks>
        /// <returns>A general `Proxy` type from a more specialised type</returns>
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, A> ToProxy() =>
            Value.ToProxy();

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, S> Bind<S>(Func<A, Proxy<RT, Void, Unit, Unit, Void, S>> f) =>
            Value.Bind(f);
            
        /// <summary>
        /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
        /// </summary>
        /// <param name="f">The map function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);

        /// <summary>
        /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
        /// </summary>
        /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
        /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
        /// processing of the computation</param>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the function provided</returns>
        [Pure]
        public override Proxy<RT, Void, Unit, C1, C, A> For<C1, C>(Func<Void, Proxy<RT, Void, Unit, C1, C, Unit>> body) =>
            Value.For(body);

        /// <summary>
        /// Applicative action
        ///
        /// Invokes this `Proxy`, then the `Proxy r` 
        /// </summary>
        /// <param name="r">`Proxy` to run after this one</param>
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, S> Action<S>(Proxy<RT, Void, Unit, Unit, Void, S> r) =>
            Value.Action(r);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, A>> lhs) =>
            Value.ComposeRight(lhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Unit, Void, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, Void, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Void, Unit, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        /// <summary>
        /// Reverse the arrows of the `Proxy` to find its dual.  
        /// </summary>
        /// <returns>The dual of `this`</returns>
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, A> Reflect() =>
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
        public override Proxy<RT, Void, Unit, Unit, Void, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Void, Unit, Unit, Void, A> value) =>
            value = Value;

        /// <summary>
        /// Monadic bind operation, for chaining `Effect` and `Aff` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Aff` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Aff<RT, C> SelectMany<B, C>(Func<A, Aff<RT, B>> bind, Func<A, B, C> project) =>
            this.RunEffect().Bind(a => bind(a).Map(b => project(a, b)));

        /// <summary>
        /// Monadic bind operation, for chaining `Effect` and `Aff` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Aff` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Aff<RT, C> SelectMany<B, C>(Func<A, Aff<B>> bind, Func<A, B, C> project) =>
            this.RunEffect().Bind(a => bind(a).Map(b => project(a, b)));

        /// <summary>
        /// Monadic bind operation, for chaining `Effect` and `Aff` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Aff` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Aff<RT, C> SelectMany<B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project) =>
            this.RunEffect().Bind(a => bind(a).Map(b => project(a, b)));

        /// <summary>
        /// Monadic bind operation, for chaining `Effect` and `Aff` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Aff` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Aff<RT, C> SelectMany<B, C>(Func<A, Eff<B>> bind, Func<A, B, C> project) =>
            this.RunEffect().Bind(a => bind(a).Map(b => project(a, b)));
    }
}
