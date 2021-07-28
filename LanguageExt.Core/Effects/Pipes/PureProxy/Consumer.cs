//
// This file contains a number of micro-free-monads that allow for creation of pure producers, consumers, and pipes.  
// They're used to facilitate the building of Proxy derived types without the need for typing the generic arguments endlessly
// The Haskell original could auto-infer the generic parameter types, the system here tries to replicate manually what
// Haskell can do automatically.  Hence why there are so many implementations of SelectMany!
//

using System;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;

namespace LanguageExt.Pipes
{
    public abstract class Consumer<IN, A>
    {
        public abstract Consumer<IN, B> Select<B>(Func<A, B> f);
        public abstract Consumer<IN, B> SelectMany<B>(Func<A, Consumer<IN, B>> f);
        public abstract ConsumerLift<RT, IN, B> SelectMany<RT, B>(Func<A, ConsumerLift<RT, IN, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Consumer<RT, IN, B> SelectMany<RT, B>(Func<A, Consumer<RT, IN, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Pipe<IN, OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f);
        public abstract Pipe<RT, IN, OUT, B> SelectMany<RT, OUT, B>(Func<A, ProducerLift<RT, OUT, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Consumer<RT, IN, A> Interpret<RT>() where RT : struct, HasCancel<RT>;
        public abstract ConsumerLift<RT, IN, A> ToConsumerLift<RT>() where RT : struct, HasCancel<RT>;
        public abstract Pipe<IN, OUT, A> ToPipe<OUT>();
        
        public Consumer<IN, B> Map<B>(Func<A, B> f) => Select(f);
        public Consumer<IN, B> Bind<B>(Func<A, Consumer<IN, B>> f) => SelectMany(f);
        public ConsumerLift<RT, IN, B> Bind<RT, B>(Func<A, ConsumerLift<RT, IN, B>> f) where RT : struct, HasCancel<RT> => SelectMany(f);
        public Consumer<RT, IN, B> Bind<RT, B>(Func<A, Consumer<RT, IN, B>> f) where RT : struct, HasCancel<RT> => SelectMany(f);
        public Pipe<IN, OUT, B> Bind<OUT, B>(Func<A, Producer<OUT, B>> f) => SelectMany(f);
 
        public Consumer<IN, C> SelectMany<B, C>(Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public ConsumerLift<RT, IN, C> SelectMany<RT, B, C>(Func<A, ConsumerLift<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Consumer<RT, IN, C> SelectMany<RT, B, C>(Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<IN, OUT, C> SelectMany<OUT, B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
       
        public static implicit operator Consumer<IN, A>(Pipes.Pure<A> ma) =>
            new Consumer<IN, A>.Pure(ma.Value);

        public class Pure : Consumer<IN, A> 
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;

            public override Consumer<IN, B> Select<B>(Func<A, B> f) =>
                new Consumer<IN, B>.Pure(f(Value));

            public override Consumer<IN, B> SelectMany<B>(Func<A, Consumer<IN, B>> f) =>
                f(Value);

            public override ConsumerLift<RT, IN, B> SelectMany<RT, B>(Func<A, ConsumerLift<RT, IN, B>> f) =>
                f(Value);

            public override Consumer<RT, IN, B> SelectMany<RT, B>(Func<A, Consumer<RT, IN, B>> f) =>
                f(Value);

            public override Pipe<IN, OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                f(Value).ToPipe<IN>();

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, OUT, B>(Func<A, ProducerLift<RT, OUT, B>> f) =>
                f(Value).ToPipe<IN>();

            public override Consumer<RT, IN, A> Interpret<RT>() =>
                Consumer.Pure<RT, IN, A>(Value);

            public override ConsumerLift<RT, IN, A> ToConsumerLift<RT>() =>
                new ConsumerLift<RT, IN, A>.Pure(Value);

            public override Pipe<IN, OUT, A> ToPipe<OUT>() =>
                new Pipe<IN, OUT, A>.Pure(Value);
        }

        public class Await : Consumer<IN, A>
        {
            public readonly Func<IN, Consumer<IN, A>> Next;
            public Await(Func<IN, Consumer<IN, A>> next) =>
                Next = next;

            public override Consumer<IN, B> Select<B>(Func<A, B> f) =>
                new Consumer<IN, B>.Await(x => Next(x).Select(f));

            public override Consumer<IN, B> SelectMany<B>(Func<A, Consumer<IN, B>> f) =>
                new Consumer<IN, B>.Await(x => Next(x).SelectMany(f));

            public override ConsumerLift<RT, IN, B> SelectMany<RT, B>(Func<A, ConsumerLift<RT, IN, B>> f) =>
                new ConsumerLift<RT, IN, B>.Await(x => Next(x).SelectMany(f));

            public override Consumer<RT, IN, B> SelectMany<RT, B>(Func<A, Consumer<RT, IN, B>> f) =>
                Interpret<RT>().Bind(f).ToConsumer();

            public override Pipe<IN, OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Await(x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, OUT, B>(Func<A, ProducerLift<RT, OUT, B>> f) =>
                Pipe.awaiting<RT, IN, OUT>().Bind(a => Next(a).SelectMany(f));

            public override Consumer<RT, IN, A> Interpret<RT>() =>
                Consumer.awaiting<RT, IN>().Bind(x => Next(x).Interpret<RT>()).ToConsumer();

            public override ConsumerLift<RT, IN, A> ToConsumerLift<RT>() =>
                new ConsumerLift<RT, IN, A>.Await(x => Next(x).ToConsumerLift<RT>());

            public override Pipe<IN, OUT, A> ToPipe<OUT>() =>
                new Pipe<IN, OUT, A>.Await(x => Next(x).ToPipe<OUT>());
        }

        public class Release<X> : Consumer<IN, A>
        {
            readonly X Value;
            readonly Func<Unit, Consumer<IN, A>> Next;

            public Release(X value, Func<Unit, Consumer<IN, A>> next)
            {
                Value = value;
                Next  = next;
            }

            public override Consumer<IN, B> Select<B>(Func<A, B> f) =>
                new Consumer<IN, B>.Release<X>(Value, x => Next(x).Select(f));

            public override Consumer<IN, B> SelectMany<B>(Func<A, Consumer<IN, B>> f) =>
                new Consumer<IN, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override ConsumerLift<RT, IN, B> SelectMany<RT, B>(Func<A, ConsumerLift<RT, IN, B>> f) =>
                new ConsumerLift<RT, IN, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Consumer<RT, IN, B> SelectMany<RT, B>(Func<A, Consumer<RT, IN, B>> f) =>
                Consumer.release<RT, IN, X>(Value).Bind(x => Next(x).Bind(f)).ToConsumer();

            public override Pipe<IN, OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, OUT, B>(Func<A, ProducerLift<RT, OUT, B>> f) =>
                Pipe.release<RT, IN, OUT, X>(Value).Bind(a => Next(a).SelectMany(f));

            public override Consumer<RT, IN, A> Interpret<RT>() =>
                Consumer.release<RT, IN, X>(Value).Bind(x => Next(x).Interpret<RT>()).ToConsumer();

            public override ConsumerLift<RT, IN, A> ToConsumerLift<RT>() =>
                new ConsumerLift<RT, IN, A>.Release<X>(Value, x => Next(x).ToConsumerLift<RT>());

            public override Pipe<IN, OUT, A> ToPipe<OUT>() =>
                new Pipe<IN, OUT, A>.Release<X>(Value, x => Next(x).ToPipe<OUT>());
        }    
    }
}
