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
    /// Pipes both can both be `await` and can `yield`
    /// </summary>
    /// <remarks>
    ///       Upstream | Downstream
    ///           +---------+
    ///           |         |
    ///     Unit <==       <== Unit
    ///           |         |
    ///      IN  ==>       ==> OUT
    ///           |    |    |
    ///           +----|----+
    ///                |
    ///                A
    /// </remarks>
    public class Pipe<RT, IN, OUT, A> : Proxy<RT, Unit, IN, Unit, OUT, A> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, IN, Unit, OUT, A> Value;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Correctly shaped `Proxy` that represents a `Pipe`</param>
        public Pipe(Proxy<RT, Unit, IN, Unit, OUT, A> value) =>
            Value = value;
        
        /// <summary>
        /// Calling this will effectively cast the sub-type to the base.
        /// </summary>
        /// <remarks>This type wraps up a `Proxy` for convenience, and so it's a `Proxy` proxy.  So calling this method
        /// isn't exactly the same as a cast operation, as it unwraps the `Proxy` from within.  It has the same effect
        /// however, and removes a level of indirection</remarks>
        /// <returns>A general `Proxy` type from a more specialised type</returns>
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, A> ToProxy() =>
            Value.ToProxy();

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Bind<S>(Func<A, Proxy<RT, Unit, IN, Unit, OUT, S>> f) =>
            Value.Bind(f);
            
        /// <summary>
        /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
        /// </summary>
        /// <param name="f">The map function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);
        
        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Pipe<RT, IN, OUT, B> Bind<B>(Func<A, Pipe<RT, IN, OUT, B>> f) => 
            Value.Bind(f).ToPipe();
        
        /// <summary>
        /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
        /// </summary>
        /// <param name="f">The map function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
        [Pure]
        public new Pipe<RT, IN, OUT, B> Select<B>(Func<A, B> f) => 
            Value.Map(f).ToPipe();        

        /// <summary>
        /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
        /// </summary>
        /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
        /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
        /// processing of the computation</param>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the function provided</returns>
        [Pure]
        public override Proxy<RT, Unit, IN, C1, C, A> For<C1, C>(Func<OUT, Proxy<RT, Unit, IN, C1, C, Unit>> body) =>
            Value.For(body);

        /// <summary>
        /// Applicative action
        ///
        /// Invokes this `Proxy`, then the `Proxy r` 
        /// </summary>
        /// <param name="r">`Proxy` to run after this one</param>
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Action<S>(Proxy<RT, Unit, IN, Unit, OUT, S> r) =>
            Value.Action(r);


        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, IN, A>> lhs) =>
            Value.ComposeRight(lhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, OUT, IN>> lhs) =>
            Value.ComposeRight(lhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, OUT, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        /// <summary>
        /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
        /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
        /// </summary>
        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, IN, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        /// <summary>
        /// Reverse the arrows of the `Proxy` to find its dual.  
        /// </summary>
        /// <returns>The dual of `this`</returns>
        [Pure]
        public override Proxy<RT, OUT, Unit, IN, Unit, A> Reflect() =>
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
        public override Proxy<RT, Unit, IN, Unit, OUT, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Unit, IN, Unit, OUT, A> value) =>
            value = Value;
        
        /// <summary>
        /// Compose a `Producer` and a `Pipe` together into a `Producer`.  
        /// </summary>
        /// <param name="p1">`Producer`</param>
        /// <param name="p2">`Pipe`</param>
        /// <returns>`Producer`</returns>
        [Pure]
        public static Producer<RT, OUT, A> operator |(Producer<RT, IN, A> p1, Pipe<RT, IN, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        /// <summary>
        /// Compose a `Producer` and a `Pipe` together into a `Producer`.  
        /// </summary>
        /// <param name="p1">`Producer`</param>
        /// <param name="p2">`Pipe`</param>
        /// <returns>`Producer`</returns>
        [Pure]
        public static Producer<RT, OUT, A> operator |(Producer<IN, A> p1, Pipe<RT, IN, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        /// <summary>
        /// Compose a `Producer` and a `Pipe` together into a `Producer`.  
        /// </summary>
        /// <param name="p1">`Producer`</param>
        /// <param name="p2">`Pipe`</param>
        /// <returns>`Producer`</returns>
        [Pure]
        public static Producer<RT, OUT, A> operator |(Producer<OUT, IN> p1, Pipe<RT, IN, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        /// <summary>
        /// Compose a `Pipe` and a `Consumer` together into a `Consumer`.  
        /// </summary>
        /// <param name="p1">`Pipe`</param>
        /// <param name="p2">`Consumer`</param>
        /// <returns>`Consumer`</returns>
        [Pure]
        public static Consumer<RT, IN, A> operator |(Pipe<RT, IN, OUT, A> p1, Consumer<OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        /// <summary>
        /// Compose a `Pipe` and a `Consumer` together into a `Consumer`.  
        /// </summary>
        /// <param name="p1">`Pipe`</param>
        /// <param name="p2">`Consumer`</param>
        /// <returns>`Consumer`</returns>
        [Pure]
        public static Consumer<RT, IN, A> operator |(Pipe<RT, IN, OUT, A> p1, Consumer<RT, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        /// <summary>
        /// Composition chaining of two pipes.  
        /// </summary>
        /// <remarks>
        /// We can't provide the operator overloading to chain two pipes together, because operators don't have
        /// generics of their own.  And so we can use this method to chain to pipes together.
        /// </remarks>
        /// <param name="pipe">Right hand side `Pipe`</param>
        /// <typeparam name="C">Type of value that the right hand side pipe yields</typeparam>
        /// <returns>A composition of this pipe and the `Pipe` provided</returns>
        [Pure]
        public Pipe<RT, IN, C, A> Then<C>(Pipe<RT, OUT, C, A> pipe) =>
            Proxy.compose(this, pipe);

        /// <summary>
        /// Conversion operator from the _pure_ `Pipe` type, to the `Aff` monad transformer version of the `Pipe`
        /// </summary>
        /// <param name="p">Pure `Pipe`</param>
        /// <returns>Monad transformer version of the `Pipe` that supports `Aff`</returns>
        [Pure]
        public static implicit operator Pipe<RT, IN, OUT, A>(Pipe<IN, OUT, A> p) =>
            p.Interpret<RT>();

        /// <summary>
        /// Conversion operator from the `Pure` type, to the `Aff` monad transformer version of the `Pipe`
        /// </summary>
        /// <param name="p">`Pure` value</param>
        /// <returns>Monad transformer version of the `Pipe` that supports `Aff`</returns>
        [Pure]
        public static implicit operator Pipe<RT, IN, OUT, A>(Pure<A> p) =>
            Pipe.Pure<RT, IN, OUT, A>(p.Value);

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Pipe<RT, IN, OUT, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretPipe<RT, IN, OUT>()).ToPipe();

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Pipe<RT, IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> bind) =>
            Value.Bind(a => bind(a).Interpret<RT>()).ToPipe();

        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Pipe<IN, OUT, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
        
        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Pipe<RT, IN, OUT, B> SelectMany<B>(Func<A, Pipe<RT, IN, OUT, B>> f) => 
            Value.Bind(f).ToPipe();
        
        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together.
        /// </summary>
        /// <param name="f">The bind function</param>
        /// <typeparam name="B">The mapped bound value type</typeparam>
        /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
        [Pure]
        public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) => 
            Value.Bind(a => f(a).Map(b => project(a, b))).ToPipe();
    }
}
