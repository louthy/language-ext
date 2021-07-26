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
    public abstract class Release<A>
    {
        public abstract Release<B> Select<B>(Func<A, B> f);
        public abstract Release<B> SelectMany<B>(Func<A, Release<B>> f);
        public abstract Enumerate<B> SelectMany<B>(Func<A, Enumerate<B>> f);
        public abstract Consumer<IN, B> SelectMany<IN, B>(Func<A, Consumer<IN, B>> f);
        public abstract Consumer<RT, IN, B> SelectMany<RT, IN, B>(Func<A, Consumer<RT, IN, B>> f) where RT : struct, HasCancel<RT>;
        public abstract ConsumerLift<RT, IN, B> SelectMany<RT, IN, B>(Func<A, ConsumerLift<RT, IN, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Producer<OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f);
        public abstract Producer<RT, OUT, B> SelectMany<RT, OUT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Pipe<IN, OUT, B> SelectMany<IN, OUT, B>(Func<A, Pipe<IN, OUT, B>> f);
        public abstract Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT>;

        public abstract Enumerate<A> ToEnumerate();
        public abstract Consumer<IN, A> ToConsumer<IN>();
        public abstract Producer<OUT, A> ToProducer<OUT>();
        public abstract Pipe<IN, OUT, A> ToPipe<IN, OUT>();
        public abstract ConsumerLift<RT, IN, A> ToConsumerLift<RT, IN>() where RT : struct, HasCancel<RT>;

        public abstract Consumer<RT, IN, A> InterpretConsumer<RT, IN>() where RT : struct, HasCancel<RT>;
        public abstract Producer<RT, OUT, A> InterpretProducer<RT, OUT>() where RT : struct, HasCancel<RT>;
        public abstract Pipe<RT, IN, OUT, A> InterpretPipe<RT, IN, OUT>() where RT : struct, HasCancel<RT>;
        public abstract Client<RT, REQ, RES, A> InterpretClient<RT, REQ, RES>() where RT : struct, HasCancel<RT>;
        public abstract Server<RT, REQ, RES, A> InterpretServer<RT, REQ, RES>() where RT : struct, HasCancel<RT>;
            
        public static implicit operator Release<A>(Pipes.Pure<A> ma) =>
            new Release<A>.Pure(ma.Value);

        public Release<B> Map<B>(Func<A, B> f) =>
            Select(f);
        
        public Release<B> Bind<B>(Func<A, Release<B>> f) =>
            SelectMany(f);
        
        public Enumerate<B> Bind<B>(Func<A, Enumerate<B>> f) =>
            SelectMany(f);
        
        public Consumer<IN, B> Bind<IN, B>(Func<A, Consumer<IN, B>> f) =>
            SelectMany(f);
        
        public Consumer<RT, IN, B> Bind<RT, IN, B>(Func<A, Consumer<RT, IN, B>> f) where RT : struct, HasCancel<RT> =>
            SelectMany(f);
        
        public Producer<OUT, B> Bind<OUT, B>(Func<A, Producer<OUT, B>> f) =>
            SelectMany(f);
        
        public Producer<RT, OUT, B> Bind<RT, OUT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            SelectMany(f);
        
        public Pipe<IN, OUT, B> Bind<IN, OUT, B>(Func<A, Pipe<IN, OUT, B>> f) =>
            SelectMany(f);
        
        public Pipe<RT, IN, OUT, B> Bind<RT, IN, OUT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            SelectMany(f);

        public Release<C> SelectMany<B, C>(Func<A, Release<B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));    
        
        public Enumerate<C> SelectMany<B, C>(Func<A, Enumerate<B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));    
        
        public Consumer<IN, C> SelectMany<IN, B, C>(Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));    
        
        public Consumer<RT, IN, C> SelectMany<RT, IN, B, C>(Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            SelectMany(a => f(a).Select(b => project(a, b)));    
        
        public Producer<OUT, C> SelectMany<OUT, B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));    
        
        public Producer<RT, OUT, C> SelectMany<RT, OUT, B, C>(Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            SelectMany(a => f(a).Select(b => project(a, b)));    
        
        public Pipe<IN, OUT, C> SelectMany<IN, OUT, B, C>(Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));    
        
        public Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, B, C>(Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            SelectMany(a => f(a).Select(b => project(a, b)));

        public class Pure : Release<A>
        {
            public readonly A Value;

            public Pure(A value) =>
                Value = value;

            public override Release<B> Select<B>(Func<A, B> f) =>
                new Release<B>.Pure(f(Value));

            public override Release<B> SelectMany<B>(Func<A, Release<B>> f) =>
                f(Value);

            public override Enumerate<B> SelectMany<B>(Func<A, Enumerate<B>> f) =>
                f(Value);

            public override Consumer<IN, B> SelectMany<IN, B>(Func<A, Consumer<IN, B>> f) =>
                f(Value);

            public override ConsumerLift<RT, IN, B> SelectMany<RT, IN, B>(Func<A, ConsumerLift<RT, IN, B>> f) =>
                f(Value);

            public override Consumer<RT, IN, B> SelectMany<RT, IN, B>(Func<A, Consumer<RT, IN, B>> f) =>
                f(Value);

            public override Producer<OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                f(Value);

            public override Producer<RT, OUT, B> SelectMany<RT, OUT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                f(Value);

            public override Pipe<IN, OUT, B> SelectMany<IN, OUT, B>(Func<A, Pipe<IN, OUT, B>> f) =>
                f(Value);

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                f(Value);

            public override Enumerate<A> ToEnumerate() =>
                new Enumerate<A>.Pure(Value);

            public override Consumer<IN, A> ToConsumer<IN>() =>
                new Consumer<IN, A>.Pure(Value);

            public override Producer<OUT, A> ToProducer<OUT>() =>
                new Producer<OUT, A>.Pure(Value);

            public override Pipe<IN, OUT, A> ToPipe<IN, OUT>() =>
                new Pipe<IN, OUT, A>.Pure(Value);

            public override ConsumerLift<RT, IN, A> ToConsumerLift<RT, IN>() =>
                new ConsumerLift<RT, IN, A>.Pure(Value);

            public override Consumer<RT, IN, A> InterpretConsumer<RT, IN>() =>
                Consumer.Pure<RT, IN, A>(Value);

            public override Producer<RT, OUT, A> InterpretProducer<RT, OUT>() =>
                Producer.Pure<RT, OUT, A>(Value);

            public override Pipe<RT, IN, OUT, A> InterpretPipe<RT, IN, OUT>() =>
                Pipe.Pure<RT, IN, OUT, A>(Value);

            public override Client<RT, REQ, RES, A> InterpretClient<RT, REQ, RES>() =>
                Client.Pure<RT, REQ, RES, A>(Value);

            public override Server<RT, REQ, RES, A> InterpretServer<RT, REQ, RES>() =>
                Server.Pure<RT, REQ, RES, A>(Value);
        }

        public class Do<X> : Release<A>
        {
            readonly X Value;
            readonly Func<Unit, Release<A>> Next; 
            
            public Do(X value, Func<Unit, Release<A>> next)
            {
                Value = value;
                Next  = next;
            }

            public override Release<B> Select<B>(Func<A, B> f) =>
                new Release<B>.Do<X>(Value, x => Next(x).Map(f));

            public override Release<B> SelectMany<B>(Func<A, Release<B>> f) =>
                new Release<B>.Do<X>(Value, x => Next(x).Bind(f));

            public override Enumerate<B> SelectMany<B>(Func<A, Enumerate<B>> f) =>
                new Enumerate<B>.Release<X>(Value, x => Next(x).Bind(f));

            public override Consumer<IN, B> SelectMany<IN, B>(Func<A, Consumer<IN, B>> f) =>
                new Consumer<IN, B>.Release<X>(Value, x => Next(x).Bind(f));

            public override ConsumerLift<RT, IN, B> SelectMany<RT, IN, B>(Func<A, ConsumerLift<RT, IN, B>> f) =>
                new ConsumerLift<RT, IN, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Consumer<RT, IN, B> SelectMany<RT, IN, B>(Func<A, Consumer<RT, IN, B>> f) =>
                Consumer.release<RT, IN, X>(Value).Bind(x => Next(x).Bind(f)).ToConsumer();

            public override Producer<OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                new Producer<OUT, B>.Release<X>(Value, x => Next(x).Bind(f));

            public override Producer<RT, OUT, B> SelectMany<RT, OUT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                Producer.release<RT, OUT, X>(Value).Bind(x => Next(x).Bind(f)).ToProducer();

            public override Pipe<IN, OUT, B> SelectMany<IN, OUT, B>(Func<A, Pipe<IN, OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Release<X>(Value, x => Next(x).Bind(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                Pipe.release<RT, IN, OUT, X>(Value).Bind(x => Next(x).Bind(f)).ToPipe();

            public override Enumerate<A> ToEnumerate() =>
                new Enumerate<A>.Release<X>(Value, x => Next(x).ToEnumerate());

            public override Consumer<IN, A> ToConsumer<IN>() =>
                new Consumer<IN, A>.Release<X>(Value, x => Next(x).ToConsumer<IN>());

            public override ConsumerLift<RT, IN, A> ToConsumerLift<RT, IN>() =>
                new ConsumerLift<RT, IN, A>.Release<X>(Value, x => Next(x).ToConsumerLift<RT, IN>());

            public override Consumer<RT, IN, A> InterpretConsumer<RT, IN>() =>
                Consumer.release<RT, IN, X>(Value).Bind(x => Next(x).InterpretConsumer<RT, IN>()).ToConsumer();

            public override Producer<RT, OUT, A> InterpretProducer<RT, OUT>() =>
                Producer.release<RT, OUT, X>(Value).Bind(x => Next(x).InterpretProducer<RT, OUT>()).ToProducer();

            public override Pipe<RT, IN, OUT, A> InterpretPipe<RT, IN, OUT>() =>
                Pipe.release<RT, IN, OUT, X>(Value).Bind(x => Next(x).InterpretPipe<RT, IN, OUT>()).ToPipe();

            public override Client<RT, REQ, RES, A> InterpretClient<RT, REQ, RES>() =>
                Client.release<RT, REQ, RES, X>(Value).Bind(x => Next(x).InterpretClient<RT, REQ, RES>()).ToClient();

            public override Server<RT, REQ, RES, A> InterpretServer<RT, REQ, RES>() =>
                Server.release<RT, REQ, RES, X>(Value).Bind(x => Next(x).InterpretServer<RT, REQ, RES>()).ToServer();

            public override Producer<OUT, A> ToProducer<OUT>() =>
                new Producer<OUT, A>.Release<X>(Value, x => Next(x).ToProducer<OUT>());

            public override Pipe<IN, OUT, A> ToPipe<IN, OUT>() =>
                new Pipe<IN, OUT, A>.Release<X>(Value, x => Next(x).ToPipe<IN, OUT>());
        }
    }
}
