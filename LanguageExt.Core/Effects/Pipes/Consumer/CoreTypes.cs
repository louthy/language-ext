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
        
        public Consumer(Proxy<RT, Unit, IN, Unit, Void, A> value) =>
            Value = value;

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, A> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Bind<S>(Func<A, Proxy<RT, Unit, IN, Unit, Void, S>> f) =>
            Value.Bind(f);

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);
        
        [Pure]
        public Consumer<RT, IN, B> Bind<B>(Func<A, Consumer<RT, IN, B>> f) => 
            Value.Bind(f).ToConsumer();
        
        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, Consumer<RT, IN, B>> f) => 
            Value.Bind(f).ToConsumer();
        
        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) => 
            Value.Bind(a => f(a).Map(b => project(a, b))).ToConsumer();
        
        [Pure]
        public new Consumer<RT, IN, B> Select<B>(Func<A, B> f) => 
            Value.Map(f).ToConsumer();        

        [Pure]
        public override Proxy<RT, Unit, IN, C1, C, A> For<C1, C>(Func<Void, Proxy<RT, Unit, IN, C1, C, Unit>> body) =>
            Value.For(body);

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Action<S>(Proxy<RT, Unit, IN, Unit, Void, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, IN, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, Void, IN>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, Void, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, IN, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);
        
        [Pure]
        public override Proxy<RT, Void, Unit, IN, Unit, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Unit, IN, Unit, Void, A> value) =>
            value = Value;

        [Pure]
        public static implicit operator Consumer<RT, IN, A>(Consumer<IN, A> c) =>
            c.Interpret<RT>();

        [Pure]
        public static implicit operator Consumer<RT, IN, A>(ConsumerLift<RT, IN, A> c) =>
            c.Interpret();

        [Pure]
        public static implicit operator Consumer<RT, IN, A>(Pure<A> p) =>
            Consumer.Pure<RT, IN, A>(p.Value);

        [Pure]
        public static Effect<RT, A> operator |(IN p1, Consumer<RT, IN, A> p2) => 
            Proxy.compose(Producer.yield<RT, IN>(p1).Map(_ => default(A)), p2).ToEffect();

        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretConsumer<RT, IN>()).ToConsumer();

        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
        
        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, Consumer<IN, B>> bind) =>
            Value.Bind(a =>  bind(a).Interpret<RT>()).ToConsumer();

        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Consumer<IN, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));

        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, ConsumerLift<RT, IN, B>> bind) =>
            Value.Bind(a =>  bind(a).Interpret()).ToConsumer();

        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, ConsumerLift<RT, IN, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
    }
}
