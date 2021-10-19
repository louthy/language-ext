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
    /// Pipes both await and yield
    /// </summary>
    /// <remarks>
    ///     Upstream | Downstream
    ///         +---------+
    ///         |         |
    ///     () <==       <== ()
    ///         |         |
    ///     A  ==>       ==> B
    ///         |    |    |
    ///         +----|----+
    ///              R
    /// </remarks>
    public class Pipe<RT, IN, OUT, A> : Proxy<RT, Unit, IN, Unit, OUT, A> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, IN, Unit, OUT, A> Value;
        
        public Pipe(Proxy<RT, Unit, IN, Unit, OUT, A> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, A> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Bind<S>(Func<A, Proxy<RT, Unit, IN, Unit, OUT, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);
        
        [Pure]
        public Pipe<RT, IN, OUT, B> Bind<B>(Func<A, Pipe<RT, IN, OUT, B>> f) => 
            Value.Bind(f).ToPipe();
        
        [Pure]
        public Pipe<RT, IN, OUT, B> SelectMany<B>(Func<A, Pipe<RT, IN, OUT, B>> f) => 
            Value.Bind(f).ToPipe();
        
        [Pure]
        public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) => 
            Value.Bind(a => f(a).Map(b => project(a, b))).ToPipe();
        
        [Pure]
        public new Pipe<RT, IN, OUT, B> Select<B>(Func<A, B> f) => 
            Value.Map(f).ToPipe();        

        [Pure]
        public override Proxy<RT, Unit, IN, C1, C, A> For<C1, C>(Func<OUT, Proxy<RT, Unit, IN, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Action<S>(Proxy<RT, Unit, IN, Unit, OUT, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, IN, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, OUT, IN>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, OUT, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, IN, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, OUT, Unit, IN, Unit, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Unit, IN, Unit, OUT, A> value) =>
            value = Value;
        
        [Pure]
        public static Producer<RT, OUT, A> operator |(Producer<RT, IN, A> p1, Pipe<RT, IN, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Producer<RT, OUT, A> operator |(Producer<IN, A> p1, Pipe<RT, IN, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Consumer<RT, IN, A> operator |(Pipe<RT, IN, OUT, A> p1, Consumer<OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Consumer<RT, IN, A> operator |(Pipe<RT, IN, OUT, A> p1, Consumer<RT, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Producer<RT, OUT, A> operator |(Producer<OUT, IN> p1, Pipe<RT, IN, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public Pipe<RT, IN, C, A> Then<C>(Pipe<RT, OUT, C, A> p2) =>
            Proxy.compose(this, p2);

        [Pure]
        public static implicit operator Pipe<RT, IN, OUT, A>(Pipe<IN, OUT, A> p) =>
            p.Interpret<RT>();

        [Pure]
        public static implicit operator Pipe<RT, IN, OUT, A>(Pure<A> p) =>
            Pipe.Pure<RT, IN, OUT, A>(p.Value);

        [Pure]
        public Pipe<RT, IN, OUT, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretPipe<RT, IN, OUT>()).ToPipe();

        [Pure]
        public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));

        [Pure]
        public Pipe<RT, IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> bind) =>
            Value.Bind(a => bind(a).Interpret<RT>()).ToPipe();

        [Pure]
        public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Pipe<IN, OUT, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
    }
}
