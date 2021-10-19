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
    /// Proxy Monad Transformer
    /// 
    /// Diagrammatically, you can think of a `Proxy` as having the following shape:
    /// 
    ///         Upstream | Downstream
    ///             +---------+
    ///             |         |
    ///       UOut <==       <== DIn
    ///             |         |
    ///       UIn  ==>       ==> DOut
    ///             |    |    |
    ///             +----|----+
    ///                  A
    ///
    /// You can connect proxies together in five different ways:
    /// 
    ///   1. connect pull-based streams
    ///   2. connect push-based streams
    ///   3. chain folds
    ///   4. chain unfolds
    ///   5. sequence proxies
    /// 
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
        /// When working with sub-types, like `Producer`, calling this will effectively cast the sub-type to the base
        /// </summary>
        /// <returns>A general `Proxy` type from a more specialised type</returns>
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy();
        
        /// <summary>
        /// Monadic bind operation, for chaining `Proxy` computations together
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
        
        public abstract Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f);
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, B> Action<B>(Proxy<RT, UOut, UIn, DIn, DOut, B> r);
        public abstract Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> lhs);
        public abstract Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs);
        public abstract Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs);
        public abstract Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT,  UOut, UIn, DInC, DOutC, DIn>> rhs);
        public abstract Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect();
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, A> Observe();
        
        [Pure]
        public Proxy<RT, UOut, UIn, DIn, DOut, B> SelectMany<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f) =>
            Bind(f);

        [Pure]
        public Proxy<RT, UOut, UIn, DIn, DOut, C> SelectMany<B, C>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f, Func<A, B, C> project) =>
            Bind(a => f(a).Map(b => project(a, b)));

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

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Bind<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f) =>
            f(Value);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Map<B>(Func<A, B> f) =>
            new Pure<RT, UOut, UIn, DIn, DOut, B>(f(Value));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Pure<RT, UOut, UIn, C1, C, A>(Value);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Action<B>(Proxy<RT, UOut, UIn, DIn, DOut, B> r) =>
            r;

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new Pure<RT, UOutA, AUInA, DIn, DOut, A>(Value);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Pure<RT, UOutA, AUInA, DIn, DOut, A>(Value);
                
        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Pure<RT, UOut, UIn, DInC, DOutC, A>(Value);

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Pure<RT, UOut, UIn, DInC, DOutC, A>(Value);

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Pure<RT, DOut, DIn, UIn, UOut, A>(Value);

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
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Bind(f)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Map(f)));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new M<RT, UOut, UIn, C1, C, A>(Value.Map(mx => mx.For(f)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Action(r)));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new M<RT, UOutA, AUInA, DIn, DOut, A>(Value.Map(p1 => p1.ComposeRight(fb1)));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new M<RT, UOutA, AUInA, DIn, DOut, A>(Value.Map(x => x.ComposeRight(lhs)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new M<RT, UOut, UIn, DInC, DOutC, A>(Value.Map(p1 => p1.ComposeLeft(rhs)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new M<RT, UOut, UIn, DInC, DOutC, A>(Value.Map(x => x.ComposeLeft(rhs)));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new M<RT, DOut, DIn, UIn, UOut, A>(Value.Map(x => x.Reflect()));
         
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Value.Bind(x => ((M<RT, UOut, UIn, DIn, DOut, A>)x.Observe()).Value));
        
        [Pure]
        public void Deconstruct(out Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>> value) =>
            value = Value;
    }
}
