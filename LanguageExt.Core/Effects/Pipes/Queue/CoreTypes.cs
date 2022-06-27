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
    public class Queue<RT, OUT, A> : Producer<RT, OUT, A> 
        where RT : struct, HasCancel<RT>
    {
        /// <summary>
        /// Single queue channel
        /// </summary>
        readonly Channel<OUT> channel;

        /// <summary>
        /// Enqueue an item 
        /// </summary>
        public Unit Enqueue(OUT value) =>
            channel.Post(value);

        /// <summary>
        /// Enqueue an item 
        /// </summary>
        public Eff<RT, Unit> EnqueueEff(OUT value) =>
            Prelude.SuccessEff(Enqueue(value));

        /// <summary>
        /// Mark the Queue as done and cancel any Effect that it is in
        /// </summary>
        public Unit Done() =>
            channel.Stop();

        /// <summary>
        /// Mark the Queue as done and cancel any Effect that it is in
        /// </summary>
        public Eff<RT, Unit> DoneEff =>
            Prelude.SuccessEff(Done());
        
        internal Queue(Proxy<RT, Void, Unit, Unit, OUT, A> value, Channel<OUT> channel) : base(value) =>
            this.channel = channel;
        
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
        public static Effect<RT, A> operator |(Queue<RT, OUT, A> p1, Consumer<RT, OUT, A> p2) => 
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Effect<RT, A> operator |(Queue<RT, OUT, A> p1, Consumer<OUT, A> p2) => 
            Proxy.compose(p1, p2);

        [Pure]
        public static Producer<RT, OUT, A> operator +(Queue<RT, OUT, A> ma, Queue<RT, OUT, A> mb) =>
            Producer.merge(ma, mb);
    }
}
