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
        
        public Producer(Proxy<RT, Void, Unit, Unit, OUT, A> value) =>
            Value = value;

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, A> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, S> Bind<S>(Func<A, Proxy<RT, Void, Unit, Unit, OUT, S>> f) =>
            Value.Bind(f);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, B> Map<B>(Func<A, B> f) =>
            Value.Map(f);
        
        [Pure]
        public Producer<RT, OUT, B> Bind<B>(Func<A, Producer<RT, OUT, B>> f) => 
            Value.Bind(f).ToProducer();
        
        [Pure]
        public Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<RT, OUT, B>> f) => 
            Value.Bind(f).ToProducer();
        
        [Pure]
        public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) => 
            Value.Bind(a => f(a).Map(b => project(a, b))).ToProducer();
        
        [Pure]
        public new Producer<RT, OUT, B> Select<B>(Func<A, B> f) => 
            Value.Map(f).ToProducer();

        [Pure]
        public override Proxy<RT, Void, Unit, C1, C, A> For<C1, C>(Func<OUT, Proxy<RT, Void, Unit, C1, C, Unit>> body) =>
            Value.For(body);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, B> Action<B>(Proxy<RT, Void, Unit, Unit, OUT, B> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Unit, OUT, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, OUT, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Void, Unit, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, OUT, Unit, Unit, Void, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, A> Observe() =>
            Value.Observe();

        [Pure]
        public static Effect<RT, A> operator |(Producer<RT, OUT, A> p1, Consumer<RT, OUT, A> p2) => 
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Effect<RT, A> operator |(Producer<RT, OUT, A> p1, Consumer<OUT, A> p2) => 
            Proxy.compose(p1, p2);
        
        [Pure]
        public void Deconstruct(out Proxy<RT, Void, Unit, Unit, OUT, A> value) =>
            value = Value;

        [Pure]
        public static implicit operator Producer<RT, OUT, A>(Producer<OUT, A> p) =>
            p.Interpret<RT>();

        [Pure]
        public static implicit operator Producer<RT, OUT, A>(ProducerLift<RT, OUT, A> p) =>
            p.Interpret();

        [Pure]
        public static implicit operator Producer<RT, OUT, A>(Pure<A> p) =>
            Producer.Pure<RT, OUT, A>(p.Value);

        [Pure]
        public Producer<RT, OUT, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretProducer<RT, OUT>()).ToProducer();

        [Pure]
        public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));

        [Pure]
        public Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> bind) =>
            Value.Bind(a => bind(a).Interpret<RT>()).ToProducer();

        [Pure]
        public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));

        [Pure]
        public static Producer<RT, OUT, A> operator +(Producer<RT, OUT, A> ma, Producer<RT, OUT, A> mb) =>
            Producer.merge<RT, OUT, A>(ma, mb);
    }
}
