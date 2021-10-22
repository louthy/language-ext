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
    /// <typeparam name="RT">Aff system runtime</typeparam>
    /// <typeparam name="UOut">Upstream out type</typeparam>
    /// <typeparam name="UIn">Upstream in type</typeparam>
    /// <typeparam name="DIn">Downstream in type</typeparam>
    /// <typeparam name="DOut">Downstream uut type</typeparam>
    /// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
    /// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
    ///
    /// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
    public abstract class Proxy<RT, UOut, UIn, DIn, DOut, A>  where RT : struct, HasCancel<RT>
    {
        /// <summary>
        /// When working with sub-types, like `Producer`, calling this will effectively cast the sub-type to the base.
        /// </summary>
        /// <returns>A general `Proxy` type from a more specialised type</returns>
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy();
        
        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, B> Bind<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f);
        
        /// <summary>
        /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
        /// </summary>
        /// <param name="f">The map function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, B> Map<B>(Func<A, B> f);
        
        /// <summary>
        /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
        /// </summary>
        /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
        /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
        /// processing of the computation</param>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the function provided</returns>
        public abstract Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> body);
        
        /// <summary>
        /// Applicative action
        ///
        /// Invokes this `Proxy`, then the `Proxy r` 
        /// </summary>
        /// <param name="r">`Proxy` to run after this one</param>
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, B> Action<B>(Proxy<RT, UOut, UIn, DIn, DOut, B> r);
        
        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        public abstract Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> lhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        public abstract Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        public abstract Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        public abstract Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT,  UOut, UIn, DInC, DOutC, DIn>> rhs);

        /// <summary>
        /// Reverse the arrows of the `Proxy` to find its dual.  
        /// </summary>
        /// <returns>The dual of `this`</returns>
        public abstract Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect();
        
        /// <summary>
        /// 
        ///     Observe(lift (Pure(r))) = Observe(Pure(r))
        ///     Observe(lift (m.Bind(f))) = Observe(lift(m.Bind(x => lift(f(x)))))
        /// 
        /// This correctness comes at a small cost to performance, so use this function sparingly.
        /// This function is a convenience for low-level pipes implementers.  You do not need to
        /// use observe if you stick to the safe API.        
        /// </summary>
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, A> Observe();
        
        /// <summary>
        /// Monadic bind operation, enables usage in LINQ expressions
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The new bound value type</typeparam>
        /// <returns>The result of the bind composition</returns>
        [Pure]
        public Proxy<RT, UOut, UIn, DIn, DOut, B> SelectMany<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f) =>
            Bind(f);

        /// <summary>
        /// Monadic bind operation, followed by a mapping projection, enables usage in LINQ expressions
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <param name="prject">The mapping projection function</param>
        /// <typeparam name="C">The new bound value type</typeparam>
        /// <returns>The result of the bind and mapping composition</returns>
        [Pure]
        public Proxy<RT, UOut, UIn, DIn, DOut, C> SelectMany<B, C>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f, Func<A, B, C> project) =>
            Bind(a => f(a).Map(b => project(a, b)));

        /// <summary>
        /// Functor map operation, enables usage in LINQ expressions
        /// </summary>
        /// <param name="f">Map function</param>
        /// <typeparam name="B">The new bound value type</typeparam>
        /// <returns>The result of the lifted function applied to the bound value</returns>
        [Pure]
        public Proxy<RT, UOut, UIn, DIn, DOut, B> Select<B>(Func<A, B> f) =>
            Map(f);
    }
    
    /// <summary>
    /// One of the algebraic cases of the `Proxy` type.  This type represents a pure value.  It can be thought of as the
    /// terminating value of the computation, as there's not continuation attached to this case. 
    /// </summary>
    /// <typeparam name="RT">Aff system runtime</typeparam>
    /// <typeparam name="UOut">Upstream out type</typeparam>
    /// <typeparam name="UIn">Upstream in type</typeparam>
    /// <typeparam name="DIn">Downstream in type</typeparam>
    /// <typeparam name="DOut">Downstream uut type</typeparam>
    /// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
    /// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
    ///
    /// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
    public class Pure<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        public readonly A Value;

        public Pure(A value) =>
            Value = value;

        /// <summary>
        /// When working with sub-types, like `Producer`, calling this will effectively cast the sub-type to the base.
        /// </summary>
        /// <returns>A general `Proxy` type from a more specialised type</returns>
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Bind<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f) =>
            f(Value);

        /// <summary>
        /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
        /// </summary>
        /// <param name="f">The map function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Map<B>(Func<A, B> f) =>
            new Pure<RT, UOut, UIn, DIn, DOut, B>(f(Value));

        /// <summary>
        /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
        /// </summary>
        /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
        /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
        /// processing of the computation</param>
        /// <returns></returns>
        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> body) =>
            new Pure<RT, UOut, UIn, C1, C, A>(Value);

        /// <summary>
        /// Applicative action
        ///
        /// Invokes this `Proxy`, then the `Proxy r` 
        /// </summary>
        /// <param name="r">`Proxy` to run after this one</param>
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Action<B>(Proxy<RT, UOut, UIn, DIn, DOut, B> r) =>
            r;

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new Pure<RT, UOutA, AUInA, DIn, DOut, A>(Value);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Pure<RT, UOutA, AUInA, DIn, DOut, A>(Value);
                
        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Pure<RT, UOut, UIn, DInC, DOutC, A>(Value);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Pure<RT, UOut, UIn, DInC, DOutC, A>(Value);

        /// <summary>
        /// Reverse the arrows of the `Proxy` to find its dual.  
        /// </summary>
        /// <returns>The dual of `this1</returns>
        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Pure<RT, DOut, DIn, UIn, UOut, A>(Value);

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
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>>.Success(this));

        [Pure]
        public void Deconstruct(out A value) =>
            value = Value;
    }

    /// <summary>
    /// One of the algebraic cases of the `Proxy` type.  This type lifts an `Aff<RT, A>` monadic computation into the
    /// `Proxy` monad.  This is how the `Proxy` system can cause real-world effects.
    /// </summary>
    /// <typeparam name="RT">Aff system runtime</typeparam>
    /// <typeparam name="UOut">Upstream out type</typeparam>
    /// <typeparam name="UIn">Upstream in type</typeparam>
    /// <typeparam name="DIn">Downstream in type</typeparam>
    /// <typeparam name="DOut">Downstream uut type</typeparam>
    /// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
    /// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
    ///
    /// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
    public class M<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A>  where RT : struct, HasCancel<RT>
    {
        public readonly Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>> Value;
        
        public M(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>> value) =>
            Value = value;
        
        /// <summary>
        /// When working with sub-types, like `Producer`, calling this will effectively cast the sub-type to the base.
        /// </summary>
        /// <returns>A general `Proxy` type from a more specialised type</returns>
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Bind(f)));

        /// <summary>
        /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
        /// </summary>
        /// <param name="f">The map function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Map(f)));

        /// <summary>
        /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
        /// </summary>
        /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
        /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
        /// processing of the computation</param>
        /// <returns></returns>
        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> body) =>
            new M<RT, UOut, UIn, C1, C, A>(Value.Map(mx => mx.For(body)));

        /// <summary>
        /// Applicative action
        ///
        /// Invokes this `Proxy`, then the `Proxy r` 
        /// </summary>
        /// <param name="r">`Proxy` to run after this one</param>
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Action(r)));

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new M<RT, UOutA, AUInA, DIn, DOut, A>(Value.Map(p1 => p1.ComposeRight(fb1)));

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new M<RT, UOutA, AUInA, DIn, DOut, A>(Value.Map(x => x.ComposeRight(lhs)));

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new M<RT, UOut, UIn, DInC, DOutC, A>(Value.Map(p1 => p1.ComposeLeft(rhs)));

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new M<RT, UOut, UIn, DInC, DOutC, A>(Value.Map(x => x.ComposeLeft(rhs)));

        /// <summary>
        /// Reverse the arrows of the `Proxy` to find its dual.  
        /// </summary>
        /// <returns>The dual of `this1</returns>
        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new M<RT, DOut, DIn, UIn, UOut, A>(Value.Map(x => x.Reflect()));
         
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
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Value.Bind(x => ((M<RT, UOut, UIn, DIn, DOut, A>)x.Observe()).Value));
        
        [Pure]
        public void Deconstruct(out Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>> value) =>
            value = Value;
    }
}
