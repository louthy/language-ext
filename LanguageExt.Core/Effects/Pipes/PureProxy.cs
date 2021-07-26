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
    public class Pure<A>
    {
        public readonly A Value;
        public Pure(A value) =>
            Value = value;
    }

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

    public abstract class Enumerate<A>
    {
        public abstract Enumerate<B> Select<B>(Func<A, B> f);
        public abstract Enumerate<B> SelectMany<B>(Func<A, Enumerate<B>> f);
        public abstract Enumerate<B> SelectMany<B>(Func<A, Pipes.Release<B>> f);
        public abstract Producer<OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f);
        public abstract Producer<RT, OUT, B> SelectMany<RT, OUT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Pipe<IN, OUT, B> SelectMany<IN, OUT, B>(Func<A, Pipe<IN, OUT, B>> f);
        public abstract Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Producer<RT, OUT, A> Interpret<RT, OUT>() where RT : struct, HasCancel<RT>;
        public abstract Pipe<RT, IN, OUT, A> Interpret<RT, IN, OUT>() where RT : struct, HasCancel<RT>;
        
        public Enumerate<B> Map<B>(Func<A, B> f) => 
            Select(f);
        
        public Enumerate<B> Bind<B>(Func<A, Enumerate<B>> f) => 
            SelectMany(f);
        
        public Enumerate<B> Bind<B>(Func<A, Pipes.Release<B>> f) => 
            SelectMany(f);
        
        public Producer<OUT, B> Bind<OUT, B>(Func<A, Producer<OUT, B>> f) => 
            SelectMany(f);
        
        public Producer<RT, OUT, B> Bind<RT, OUT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT> => 
            SelectMany(f);
        
        public Pipe<RT, IN, OUT, B> Bind<RT, IN, OUT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT> => 
            SelectMany(f);
        
        public Enumerate<C> SelectMany<B, C>(Func<A, Enumerate<B>> f, Func<A, B, C> project) => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Enumerate<C> SelectMany<B, C>(Func<A, Pipes.Release<B>> f, Func<A, B, C> project) => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Producer<OUT, C> SelectMany<OUT, B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Producer<RT, OUT, C> SelectMany<RT, OUT, B, C>(Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<IN, OUT, C> SelectMany<IN, OUT, B, C>(Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, B, C>(Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public class Pure : Enumerate<A> 
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;
            
            public override Enumerate<B> Select<B>(Func<A, B> f) =>
                new Enumerate<B>.Pure(f(Value));

            public override Enumerate<B> SelectMany<B>(Func<A, Pipes.Release<B>> f) =>
                f(Value).ToEnumerate();

            public override Producer<OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                f(Value);

            public override Enumerate<B> SelectMany<B>(Func<A, Enumerate<B>> f) =>
                f(Value);

            public override Producer<RT, OUT, B> SelectMany<RT, OUT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                f(Value);

            public override Pipe<IN, OUT, B> SelectMany<IN, OUT, B>(Func<A, Pipe<IN, OUT, B>> f) =>
                f(Value);

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                f(Value);

            public override Producer<RT, OUT, A> Interpret<RT, OUT>() =>
                Producer.Pure<RT, OUT, A>(Value);

            public override Pipe<RT, IN, OUT, A> Interpret<RT, IN, OUT>() =>
                Pipe.Pure<RT, IN, OUT, A>(Value);
        }

        public class Do<X> : Enumerate<A>
        {
            internal readonly EnumerateData<X> Values;
            public readonly Func<X, Enumerate<A>> Next;
            
            internal Do(EnumerateData<X> values, Func<X, Enumerate<A>> next) =>
                (Values, Next) = (values, next);
            
            public Do(IEnumerable<X> values, Func<X, Enumerate<A>> next) =>
                (Values, Next) = (new EnumerateEnumerable<X>(values), next);
            
            public Do(IAsyncEnumerable<X> values, Func<X, Enumerate<A>> next) =>
                (Values, Next) = (new EnumerateAsyncEnumerable<X>(values), next);
            
            public Do(IObservable<X> values, Func<X, Enumerate<A>> next) =>
                (Values, Next) = (new EnumerateObservable<X>(values), next);

            public override Enumerate<B> Select<B>(Func<A, B> f) =>
                new Enumerate<B>.Do<X>(Values, n => Next(n).Select(f));

            public override Enumerate<B> SelectMany<B>(Func<A, Enumerate<B>> f) =>
                new Enumerate<B>.Do<X>(Values, x => Next(x).SelectMany(f));

            public override Enumerate<B> SelectMany<B>(Func<A, Pipes.Release<B>> f) =>
                new Enumerate<B>.Do<X>(Values, x => Next(x).SelectMany(f));

            public override Producer<OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                new Producer<OUT, B>.Enumerate<X>(Values, x => Next(x).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<RT, OUT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                Interpret<RT, OUT>().Bind(f).ToProducer();

            public override Pipe<IN, OUT, B> SelectMany<IN, OUT, B>(Func<A, Pipe<IN, OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Enumerate<X>(Values, x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                Interpret<RT, IN, OUT>().Bind(f).ToPipe();

            public override Producer<RT, OUT, A> Interpret<RT, OUT>() =>
                Producer.enumerate<RT, OUT, X>(Values).Bind(x => Next(x).Interpret<RT, OUT>()).ToProducer();

            public override Pipe<RT, IN, OUT, A> Interpret<RT, IN, OUT>() =>
                Pipe.enumerate<RT, IN, OUT, X>(Values).Bind(x => Next(x).Interpret<RT, IN, OUT>()).ToPipe();
        }

        public class Release<X> : Enumerate<A>
        {
            readonly X Value;
            readonly Func<Unit, Enumerate<A>> Next;

            public Release(X value, Func<Unit, Enumerate<A>> next)
            {
                Value = value;
                Next  = next;
            }

            public override Enumerate<B> Select<B>(Func<A, B> f) =>
                new Enumerate<B>.Release<X>(Value, x => Next(x).Select(f));

            public override Enumerate<B> SelectMany<B>(Func<A, Enumerate<B>> f) =>
                new Enumerate<B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Enumerate<B> SelectMany<B>(Func<A, Pipes.Release<B>> f) =>
                new Enumerate<B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Producer<OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                new Producer<OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<RT, OUT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                Producer.release<RT, OUT, X>(Value).Bind(x => Next(x).SelectMany(f)).ToProducer();

            public override Pipe<IN, OUT, B> SelectMany<IN, OUT, B>(Func<A, Pipe<IN, OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                Pipe.release<RT, IN, OUT, X>(Value).Bind(x => Next(x).SelectMany(f)).ToPipe();

            public override Producer<RT, OUT, A> Interpret<RT, OUT>() =>
                Producer.release<RT, OUT, X>(Value).Bind(x => Next(x).Interpret<RT, OUT>()).ToProducer();

            public override Pipe<RT, IN, OUT, A> Interpret<RT, IN, OUT>() =>
                Pipe.release<RT, IN, OUT, X>(Value).Bind(x => Next(x).Interpret<RT, IN, OUT>()).ToPipe();
        }
    }

    public abstract class Producer<OUT, A>
    {
        public abstract Producer<OUT, B> Select<B>(Func<A, B> f);
        public abstract Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f);
        public abstract Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Producer<RT, OUT, A> Interpret<RT>() where RT : struct, HasCancel<RT>;
        public abstract Pipe<IN, OUT, A> MakePipe<IN>();

        public Producer<OUT, B> Map<B>(Func<A, B> f) => Select(f);
        public Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) => SelectMany(f);
        public Producer<RT, OUT, B> Bind<RT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT> => SelectMany(f);

        public Producer<OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Producer<RT, OUT, C> SelectMany<RT, B, C>(Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public class Pure : Producer<OUT, A> 
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;

            public override Producer<OUT, B> Select<B>(Func<A, B> f) =>
                new Producer<OUT, B>.Pure(f(Value));

            public override Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                f(Value);

            public override Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                f(Value);

            public override Producer<RT, OUT, A> Interpret<RT>() =>
                Producer.Pure<RT, OUT, A>(Value);

            public override Pipe<IN, OUT, A> MakePipe<IN>() =>
                new Pipe<IN, OUT, A>.Pure(Value);
        }

        public class Enumerate<X> : Producer<OUT, A> 
        {
            internal readonly EnumerateData<X> Values;
            public readonly Func<X, Producer<OUT, A> > Next;
            
            internal Enumerate(EnumerateData<X> values, Func<X, Producer<OUT, A> > next) =>
                (Values, Next) = (values, next);

            public Enumerate(IEnumerable<X> values, Func<X, Producer<OUT, A> > next) =>
                (Values, Next) = (new EnumerateEnumerable<X>(values), next);

            public Enumerate(IAsyncEnumerable<X> values, Func<X, Producer<OUT, A> > next) =>
                (Values, Next) = (new EnumerateAsyncEnumerable<X>(values), next);

            public Enumerate(IObservable<X> values, Func<X, Producer<OUT, A> > next) =>
                (Values, Next) = (new EnumerateObservable<X>(values), next);

            public override Producer<OUT, B> Select<B>(Func<A, B> f) =>
                new Producer<OUT, B>.Enumerate<X>(Values, n => Next(n).Select(f));

            public override Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Producer<OUT, B>.Enumerate<X>(Values, n => Next(n).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                Interpret<RT>().Bind(f).ToProducer();

            public override Producer<RT, OUT, A> Interpret<RT>() =>
                Producer.enumerate<RT, OUT, X>(Values)
                        .Bind(x => Next(x).Interpret<RT>()).ToProducer();

            public override Pipe<IN, OUT, A> MakePipe<IN>() =>
                new Pipe<IN, OUT, A>.Enumerate<X>(Values, x => Next(x).MakePipe<IN>());
        }
        
        public class Yield : Producer<OUT, A>
        {
            public readonly OUT Value;
            public readonly Func<Unit, Producer<OUT, A>> Next;
            
            public Yield(OUT value, Func<Unit, Producer<OUT, A>> next) =>
                (Value, Next) = (value, next);

            public override Producer<OUT, B> Select<B>(Func<A, B> f) =>
                new Producer<OUT, B>.Yield(Value, n => Next(n).Select(f));

            public override Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Producer<OUT, B>.Yield(Value, n => Next(n).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                Interpret<RT>().Bind(f).ToProducer();

            public override Producer<RT, OUT, A> Interpret<RT>() =>
                Producer.yield<RT, OUT>(Value).Bind(x => Next(x).Interpret<RT>()).ToProducer();

            public override Pipe<IN, OUT, A> MakePipe<IN>() =>
                new Pipe<IN, OUT, A>.Yield(Value, x => Next(x).MakePipe<IN>());
        }

        public class Release<X> : Producer<OUT, A>
        {
            readonly X Value;
            readonly Func<Unit, Producer<OUT, A>> Next;

            public Release(X value, Func<Unit, Producer<OUT, A>> next)
            {
                Value = value;
                Next  = next;
            }

            public override Producer<OUT, B> Select<B>(Func<A, B> f) =>
                new Producer<OUT, B>.Release<X>(Value, x => Next(x).Select(f));

            public override Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Producer<OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                Producer.release<RT, OUT, X>(Value).Bind(x => Next(x).Bind(f)).ToProducer();

            public override Producer<RT, OUT, A> Interpret<RT>() =>
                Producer.release<RT, OUT, X>(Value).Bind(x => Next(x).Interpret<RT>()).ToProducer();

            public override Pipe<IN, OUT, A> MakePipe<IN>() =>
                new Pipe<IN, OUT, A>.Release<X>(Value, x => Next(x).MakePipe<IN>());
        }    
    }
    
    public abstract class Consumer<IN, A>
    {
        public abstract Consumer<IN, B> Select<B>(Func<A, B> f);
        public abstract Consumer<IN, B> SelectMany<B>(Func<A, Consumer<IN, B>> f);
        public abstract ConsumerLift<RT, IN, B> SelectMany<RT, B>(Func<A, ConsumerLift<RT, IN, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Consumer<RT, IN, B> SelectMany<RT, B>(Func<A, Consumer<RT, IN, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Pipe<IN, OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f);
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
                f(Value).MakePipe<IN>();

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

            public override Consumer<RT, IN, A> Interpret<RT>() =>
                Consumer.await<RT, IN>().Bind(x => Next(x).Interpret<RT>()).ToConsumer();

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

            public override Consumer<RT, IN, A> Interpret<RT>() =>
                Consumer.release<RT, IN, X>(Value).Bind(x => Next(x).Interpret<RT>()).ToConsumer();

            public override ConsumerLift<RT, IN, A> ToConsumerLift<RT>() =>
                new ConsumerLift<RT, IN, A>.Release<X>(Value, x => Next(x).ToConsumerLift<RT>());

            public override Pipe<IN, OUT, A> ToPipe<OUT>() =>
                new Pipe<IN, OUT, A>.Release<X>(Value, x => Next(x).ToPipe<OUT>());
        }    
    }
        
    public abstract class ConsumerLift<RT, IN, A> where RT: struct, HasCancel<RT>
    {
        public abstract ConsumerLift<RT, IN, B> Select<B>(Func<A, B> f);
        public abstract ConsumerLift<RT, IN, B> SelectMany<B>(Func<A, ConsumerLift<RT, IN, B>> f);
        public abstract Consumer<RT, IN, B> SelectMany<B>(Func<A, Consumer<RT, IN, B>> f);
        public abstract Pipe<RT, IN, OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f);
        public abstract Consumer<RT, IN, A> Interpret();
        public abstract Pipe<RT, IN, OUT, A> ToPipe<OUT>();

        public ConsumerLift<RT, IN, B> Map<B>(Func<A, B> f) => Select(f);
        public ConsumerLift<RT, IN, B> Bind<B>(Func<A, ConsumerLift<RT, IN, B>> f) => SelectMany(f);
        public Consumer<RT, IN, B> Bind<B>(Func<A, Consumer<RT, IN, B>> f) => SelectMany(f);
        public Pipe<RT, IN, OUT, B> Bind<OUT, B>(Func<A, Producer<OUT, B>> f) => SelectMany(f);
        
        public ConsumerLift<RT, IN, C> SelectMany<B, C>(Func<A, ConsumerLift<RT, IN, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<RT, IN, OUT, C> SelectMany<OUT, B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
 
        public class Pure : ConsumerLift<RT, IN, A> 
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;

            public override ConsumerLift<RT, IN, B> Select<B>(Func<A, B> f) =>
                new ConsumerLift<RT, IN, B>.Pure(f(Value));

            public override ConsumerLift<RT, IN, B> SelectMany<B>(Func<A, ConsumerLift<RT, IN, B>> f) =>
                f(Value);

            public override Consumer<RT, IN, B> SelectMany<B>(Func<A, Consumer<RT, IN, B>> f) =>
                f(Value);

            public override Pipe<RT, IN, OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                f(Value).MakePipe<IN>();

            public override Consumer<RT, IN, A> Interpret() =>
                Consumer.Pure<RT, IN, A>(Value);

            public override Pipe<RT, IN, OUT, A> ToPipe<OUT>() =>
                Pipe.Pure<RT, IN, OUT, A>(Value);
        }
        
        public class Lift<X> : ConsumerLift<RT, IN, A> 
        {
            public readonly Aff<RT, X> Value;
            public readonly Func<X, ConsumerLift<RT, IN, A>> Next;

            public Lift(Aff<RT, X> value, Func<X, ConsumerLift<RT, IN, A>> next) =>
                (Value, Next) = (value, next);

            public override ConsumerLift<RT, IN, B> Select<B>(Func<A, B> f) =>
                new ConsumerLift<RT, IN, B>.Lift<X>(Value, x => Next(x).Select(f));

            public override ConsumerLift<RT, IN, B> SelectMany<B>(Func<A, ConsumerLift<RT, IN, B>> f) =>
                new ConsumerLift<RT, IN, B>.Lift<X>(Value, x => Next(x).SelectMany(f));

            public override Consumer<RT, IN, B> SelectMany<B>(Func<A, Consumer<RT, IN, B>> f) =>
                Interpret().Bind(f).ToConsumer();

            public override Pipe<RT, IN, OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                Pipe.lift<RT, IN, OUT, X>(Value).SelectMany(x => Next(x).SelectMany(f));

            public override Consumer<RT, IN, A> Interpret() =>
                Consumer.lift<RT, IN, X>(Value).Bind(x => Next(x).Interpret()).ToConsumer();

            public override Pipe<RT, IN, OUT, A> ToPipe<OUT>() =>
                Pipe.lift<RT, IN, OUT, X>(Value).Bind(x => Next(x).ToPipe<OUT>()).ToPipe();
        }

        public class Await : ConsumerLift<RT, IN, A>
        {
            public readonly Func<IN, ConsumerLift<RT, IN, A>> Next;
            public Await(Func<IN, ConsumerLift<RT, IN, A>> next) =>
                Next = next;

            public override ConsumerLift<RT, IN, B> Select<B>(Func<A, B> f) =>
                new ConsumerLift<RT, IN, B>.Await(x => Next(x).Select(f));

            public override ConsumerLift<RT, IN, B> SelectMany<B>(Func<A, ConsumerLift<RT, IN, B>> f) =>
                new ConsumerLift<RT, IN, B>.Await(x => Next(x).SelectMany(f));

            public override Consumer<RT, IN, B> SelectMany<B>(Func<A, Consumer<RT, IN, B>> f) =>
                Interpret().Bind(f).ToConsumer();

            public override Pipe<RT, IN, OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                Pipe.await<RT, IN, OUT>().SelectMany(x => Next(x).SelectMany(f));

            public override Consumer<RT, IN, A> Interpret() =>
                Consumer.await<RT, IN>().Bind(x => Next(x).Interpret()).ToConsumer();

            public override Pipe<RT, IN, OUT, A> ToPipe<OUT>() =>
                Pipe.await<RT, IN, OUT>().Bind(x => Next(x).ToPipe<OUT>()).ToPipe();
        }

        public class Release<X> : ConsumerLift<RT, IN, A>
        {
            readonly X Value;
            readonly Func<Unit, ConsumerLift<RT, IN, A>> Next;

            public Release(X value, Func<Unit, ConsumerLift<RT, IN, A>> next)
            {
                Value = value;
                Next  = next;
            }

            public override ConsumerLift<RT, IN, B> Select<B>(Func<A, B> f) =>
                new ConsumerLift<RT, IN, B>.Release<X>(Value, x => Next(x).Select(f));

            public override ConsumerLift<RT, IN, B> SelectMany<B>(Func<A, ConsumerLift<RT, IN, B>> f) =>
                new ConsumerLift<RT, IN, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Consumer<RT, IN, B> SelectMany<B>(Func<A, Consumer<RT, IN, B>> f) =>
                Consumer.release<RT, IN, X>(Value).Bind(x => Next(x).Bind(f)).ToConsumer();

            public override Pipe<RT, IN, OUT, B> SelectMany<OUT, B>(Func<A, Producer<OUT, B>> f) =>
                Pipe.release<RT, IN, OUT, X>(Value).SelectMany(x => Next(x).SelectMany(f));

            public override Consumer<RT, IN, A> Interpret() =>
                Consumer.release<RT, IN, X>(Value).Bind(x => Next(x).Interpret()).ToConsumer();

            public override Pipe<RT, IN, OUT, A> ToPipe<OUT>() =>
                Pipe.release<RT, IN, OUT, X>(Value).Bind(x => Next(x).ToPipe<OUT>()).ToPipe();
        }    
    }

    public abstract class Pipe<IN, OUT, A>
    {
        public abstract Pipe<IN, OUT, B> Select<B>(Func<A, B> f);
        public abstract Pipe<RT, IN, OUT, A> Interpret<RT>() where RT : struct, HasCancel<RT>;
        public abstract Pipe<IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> f);
        public abstract Pipe<RT, IN, OUT, B> SelectMany<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Pipe<IN, OUT, B> SelectMany<B>(Func<A, Consumer<IN, B>> f);
        public abstract Pipe<IN, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f);
        
        public Pipe<IN, OUT, B> Map<B>(Func<A, B> f) => Select(f);
        public Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) => SelectMany(f);
        public Pipe<RT, IN, OUT, B> Bind<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT> => SelectMany(f);
        public Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f) => SelectMany(f);

        public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<RT, IN, OUT, C> SelectMany<RT, B, C>(Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));

        public class Pure : Pipe<IN, OUT, A> 
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;

            public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) =>
                new Pipe<IN, OUT, B>.Pure(f(Value));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> f) =>
                f(Value);

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                f(Value);

            public override Pipe<RT, IN, OUT, A> Interpret<RT>() =>
                Pipe.Pure<RT, IN, OUT, A>(Value);

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Consumer<IN, B>> f) =>
                f(Value).ToPipe<OUT>();

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                f(Value).MakePipe<IN>();
        }

        public class Await : Pipe<IN, OUT, A>
        {
            public readonly Func<IN, Pipe<IN, OUT, A>> Next;
            
            public Await(Func<IN, Pipe<IN, OUT, A>> next) =>
                Next = next;

            public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) =>
                new Pipe<IN, OUT, B>.Await(x => Next(x).Select(f));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Await(x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                from x in Interpret<RT>()
                from r in f(x)
                select r;

            public override Pipe<RT, IN, OUT, A> Interpret<RT>() =>
                from x in Pipe.await<RT, IN, OUT>()
                from r in Next(x) 
                select r;

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Consumer<IN, B>> f) =>
                new Pipe<IN, OUT, B>.Await(x => Next(x).SelectMany(f));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Await(x => Next(x).SelectMany(f));
        }

        public class Yield : Pipe<IN, OUT, A>
        {
            public readonly OUT Value;
            public readonly Func<Unit, Pipe<IN, OUT, A>> Next;
            
            public Yield(OUT value, Func<Unit, Pipe<IN, OUT, A>> next) =>
                (Value, Next) = (value, next);

            public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) =>
                new Pipe<IN, OUT, B>.Yield(Value, x => Next(x).Select(f));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Yield(Value, x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                from x in Interpret<RT>()
                from r in f(x)
                select r;

            public override Pipe<RT, IN, OUT, A> Interpret<RT>() =>
                from x in Pipe.yield<RT, IN, OUT>(Value)
                from r in Next(x) 
                select r;

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Consumer<IN, B>> f) =>
                new Pipe<IN, OUT, B>.Yield(Value, x => Next(x).SelectMany(f));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Yield(Value, x => Next(x).SelectMany(f));
        }

        public class Enumerate<X> : Pipe<IN, OUT, A>
        {
            internal readonly EnumerateData<X> Values;
            public readonly Func<X, Pipe<IN, OUT, A>> Next;
            
            internal Enumerate(EnumerateData<X> values, Func<X, Pipe<IN, OUT, A>> next) =>
                (Values, Next) = (values, next);

            internal Enumerate(IEnumerable<X> values, Func<X, Pipe<IN, OUT, A>> next) =>
                (Values, Next) = (new EnumerateEnumerable<X>(values), next);

            internal Enumerate(IAsyncEnumerable<X> values, Func<X, Pipe<IN, OUT, A>> next) =>
                (Values, Next) = (new EnumerateAsyncEnumerable<X>(values), next);

            internal Enumerate(IObservable<X> values, Func<X, Pipe<IN, OUT, A>> next) =>
                (Values, Next) = (new EnumerateObservable<X>(values), next);

            public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) =>
                new Pipe<IN, OUT, B>.Enumerate<X>(Values, x => Next(x).Select(f));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Enumerate<X>(Values, x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                from x in Interpret<RT>()
                from r in f(x)
                select r;

            public override Pipe<RT, IN, OUT, A> Interpret<RT>() =>
                from x in Pipe.enumerate<RT, IN, OUT, X>(Values)
                from r in Next(x)
                select r;

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Consumer<IN, B>> f) =>
                new Pipe<IN, OUT, B>.Enumerate<X>(Values, x => Next(x).SelectMany(f));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Enumerate<X>(Values, x => Next(x).SelectMany(f));
        }

        public class Release<X> : Pipe<IN, OUT, A>
        {
            readonly X Value;
            readonly Func<Unit, Pipe<IN, OUT, A>> Next;

            public Release(X value, Func<Unit, Pipe<IN, OUT, A>> next)
            {
                Value = value;
                Next  = next;
            }

            public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) =>
                new Pipe<IN, OUT, B>.Release<X>(Value, x => Next(x).Select(f));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                Pipe.release<RT, IN, OUT, X>(Value).Bind(x => Next(x).Bind(f)).ToPipe();

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Consumer<IN, B>> f) =>
                new Pipe<IN, OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, A> Interpret<RT>() =>
                Pipe.release<RT, IN, OUT, X>(Value).Bind(x => Next(x).Interpret<RT>()).ToPipe();
        }    
    }

    public static class PureProxy
    {
        public static Pure<A> Pure<A>(A value) =>
            new Pure<A>(value);

        public static Release<A> ReleasePure<A>(A value) =>
            new Release<A>.Pure(value);

        public static Pipe<IN, OUT, A> PipePure<IN, OUT, A>(A value) =>
            new Pipe<IN, OUT, A>.Pure(value);

        public static Producer<OUT, A> ProducerPure<OUT, A>(A value) =>
            new Producer<OUT, A>.Pure(value);

        public static Consumer<IN, A> ConsumerPure<IN, A>(A value) =>
            new Consumer<IN, A>.Pure(value);

        public static ConsumerLift<RT, IN, A> ConsumerLiftPure<RT, IN, A>(A value) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, A>.Pure(value);

        public static Enumerate<A> EnumeratePure<A>(A value) =>
            new Enumerate<A>.Pure(value);

        public static Consumer<IN, IN> ConsumerAwait<IN>() =>
            new Consumer<IN, IN>.Await(ConsumerPure<IN, IN>);

        public static Pipe<IN, OUT, IN> PipeAwait<IN, OUT>() =>
            new Pipe<IN, OUT, IN>.Await(PipePure<IN, OUT, IN>);

        public static Producer<OUT, Unit> ProducerYield<OUT>(OUT value) =>
            new Producer<OUT, Unit>.Yield(value, ProducerPure<OUT, Unit>);

        public static Producer<OUT, X> ProducerEnumerate<OUT, X>(IEnumerable<X> xs) =>
            new Producer<OUT, X>.Enumerate<X>(xs, ProducerPure<OUT, X>);

        public static Producer<OUT, X> ProducerEnumerate<OUT, X>(IAsyncEnumerable<X> xs) =>
            new Producer<OUT, X>.Enumerate<X>(xs, ProducerPure<OUT, X>);

        public static Producer<OUT, X> ProducerObserve<OUT, X>(IObservable<X> xs) =>
            new Producer<OUT, X>.Enumerate<X>(xs, ProducerPure<OUT, X>);

        public static Producer<X, X> ProducerEnumerate<X>(IEnumerable<X> xs) =>
            new Producer<X, X>.Enumerate<X>(xs, ProducerPure<X, X>);

        public static Producer<X, X> ProducerEnumerate<X>(IAsyncEnumerable<X> xs) =>
            new Producer<X, X>.Enumerate<X>(xs, ProducerPure<X, X>);

        public static Pipe<IN, OUT, Unit> PipeYield<IN, OUT>(OUT value) =>
            new Pipe<IN, OUT, Unit>.Yield(value, PipePure<IN, OUT, Unit>);

        public static Consumer<IN, B> Select<IN, A, B>(this Consumer<IN, A> ma, Func<A, B> f) =>
            ma switch
            {
                Consumer<IN, A>.Pure v  => ConsumerPure<IN, B>(f(v.Value)),
                Consumer<IN, A>.Await v => new Consumer<IN, B>.Await(n => v.Next(n).Select(f)),
                _                          => throw new System.InvalidOperationException()
            };
        
        public static Client<RT, REQ, RES, B> SelectMany<RT, REQ, RES, A, B>(this Client<RT, REQ, RES, A> ma, Func<A, Release<B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).InterpretClient<RT, REQ, RES>()
            select r;

        public static Client<RT, REQ, RES, C> SelectMany<RT, REQ, RES, A, B, C>(this Client<RT, REQ, RES, A> ma, Func<A, Release<B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).InterpretClient<RT, REQ, RES>()
            select project(a, r);
        
        public static Server<RT, REQ, RES, B> SelectMany<RT, REQ, RES, A, B>(this Server<RT, REQ, RES, A> ma, Func<A, Release<B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).InterpretServer<RT, REQ, RES>()
            select r;

        public static Server<RT, REQ, RES, C> SelectMany<RT, REQ, RES, A, B, C>(this Server<RT, REQ, RES, A> ma, Func<A, Release<B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).InterpretServer<RT, REQ, RES>()
            select project(a, r);
        
        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<RT, IN, A> ma, Func<A, Release<B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).InterpretConsumer<RT, IN>()
            select r;

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<RT, IN, A> ma, Func<A, Release<B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).InterpretConsumer<RT, IN>()
            select project(a, r);
        
        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<RT, OUT, A> ma, Func<A, Release<B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).InterpretProducer<RT, OUT>()
            select r;

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<RT, OUT, A> ma, Func<A, Release<B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).InterpretProducer<RT, OUT>()
            select project(a, r);

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<RT, IN, OUT, A> ma, Func<A, Release<B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).InterpretPipe<RT, IN, OUT>()
            select r;

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<RT, IN, OUT, A> ma, Func<A, Release<B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).InterpretPipe<RT, IN, OUT>()
            select project(a, r);

        
        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<RT, IN, OUT, A> ma, Func<A, Pipe<IN, OUT, B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT>()
            select r;

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<RT, IN, OUT, A> ma, Func<A, Pipe<IN, OUT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT>()
            select project(a, r);

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<RT, OUT, A> ma, Func<A, Producer<OUT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT>()
            select project(a, r);

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<RT, IN, A> ma, Func<A, Consumer<IN, B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT>()
            select r;

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<RT, IN, A> ma, Func<A, Consumer<IN, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT>()
            select project(a, r);

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<RT, IN, A> ma, Func<A, ConsumerLift<RT, IN, B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret()
            select r;

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<RT, IN, A> ma, Func<A, ConsumerLift<RT, IN, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret()
            select project(a, r);

        
        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this ConsumerLift<RT, IN, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, B>.Lift<B>(f(a), ConsumerLiftPure<RT, IN, B>));

        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<IN, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, B>.Lift<B>(f(a), ConsumerLiftPure<RT, IN, B>));

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<OUT, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT>()
            from b in Producer.lift<RT, OUT, B>(f(a))
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<IN, OUT, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT>()
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select b;

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<RT, IN, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Consumer.lift<RT, IN, B>(f(a))
            select b;

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<RT, OUT, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Producer.lift<RT, OUT, B>(f(a))
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<RT, IN, OUT, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select b;
        
        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this ConsumerLift<RT, IN, A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, B>.Lift<B>(f(a), ConsumerLiftPure<RT, IN, B>));

        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<IN, A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, B>.Lift<B>(f(a), ConsumerLiftPure<RT, IN, B>));

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<OUT, A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT>()
            from b in Producer.lift<RT, OUT, B>(f(a))
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<IN, OUT, A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT>()
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select b;

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<RT, IN, A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Consumer.lift<RT, IN, B>(f(a))
            select b;

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<RT, OUT, A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Producer.lift<RT, OUT, B>(f(a))
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<RT, IN, OUT, A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select b;
        
        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this ConsumerLift<RT, IN, A> ma, Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, B>.Lift<B>(f(a), ConsumerLiftPure<RT, IN, B>));

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<RT, IN, A> ma, Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Consumer.lift<RT, IN, B>(f(a))
            select b;

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<RT, OUT, A> ma, Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Producer.lift<RT, OUT, B>(f(a))
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<RT, IN, OUT, A> ma, Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select b;
        
        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this ConsumerLift<RT, IN, A> ma, Func<A, Eff<B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, B>.Lift<B>(f(a), ConsumerLiftPure<RT, IN, B>));

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<RT, IN, A> ma, Func<A, Eff<B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Consumer.lift<RT, IN, B>(f(a))
            select b;

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<RT, OUT, A> ma, Func<A, Eff<B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Producer.lift<RT, OUT, B>(f(a))
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<RT, IN, OUT, A> ma, Func<A, Eff<B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select b;

        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this Aff<RT, A> ma, Func<A, ConsumerLift<RT, IN, B>> f) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, B>.Lift<A>(ma, f);

        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this Aff<RT, A> ma, Func<A, Consumer<IN, B>> f) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, B>.Lift<A>(ma, x => f(x).ToConsumerLift<RT>());

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Aff<RT, A> ma, Func<A, Producer<OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a).Interpret<RT>()
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Aff<RT, A> ma, Func<A, Pipe<IN, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a).Interpret<RT>()
            select b;

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Aff<RT, A> ma, Func<A, Consumer<RT, IN, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Consumer.lift<RT, IN, A>(ma)
            from b in f(a)
            select b;

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Aff<RT, A> ma, Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a)
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Aff<RT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a)
            select b;

        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this Eff<RT, A> ma, Func<A, ConsumerLift<RT, IN, B>> f) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, B>.Lift<A>(ma, f);

        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this Eff<RT, A> ma, Func<A, Consumer<IN, B>> f) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, B>.Lift<A>(ma, x => f(x).ToConsumerLift<RT>());

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Eff<RT, A> ma, Func<A, Producer<OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a).Interpret<RT>()
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Eff<RT, A> ma, Func<A, Pipe<IN, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a).Interpret<RT>()
            select b;

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Eff<RT, A> ma, Func<A, Consumer<RT, IN, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Consumer.lift<RT, IN, A>(ma)
            from b in f(a)
            select b;

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Eff<RT, A> ma, Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a)
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Eff<RT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a)
            select b;

        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this Aff<A> ma, Func<A, ConsumerLift<RT, IN, B>> f) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, B>.Lift<A>(ma, f);

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Aff<A> ma, Func<A, Consumer<RT, IN, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Consumer.lift<RT, IN, A>(ma)
            from b in f(a)
            select b;

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Aff<A> ma, Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a)
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Aff<A> ma, Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a)
            select b;

        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this Eff<A> ma, Func<A, ConsumerLift<RT, IN, B>> f) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, B>.Lift<A>(ma, f);

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Eff<A> ma, Func<A, Pipe<IN, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a).Interpret<RT>()
            select b;

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Eff<A> ma, Func<A, Consumer<RT, IN, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Consumer.lift<RT, IN, A>(ma)
            from b in f(a)
            select b;

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Eff<A> ma, Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a)
            select b;

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Eff<A> ma, Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a)
            select b;
        

    
        
        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this ConsumerLift<RT, IN, A> ma, Func<A, Aff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, C>.Lift<B>(f(a), b => ConsumerLiftPure<RT, IN, C>(project(a, b))));

        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<IN, A> ma, Func<A, Aff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, C>.Lift<B>(f(a), b => ConsumerLiftPure<RT, IN, C>(project(a, b))));

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<OUT, A> ma, Func<A, Aff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT>()
            from b in Producer.lift<RT, OUT, B>(f(a))
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<IN, OUT, A> ma, Func<A, Aff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT>()
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select project(a, b);

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<RT, IN, A> ma, Func<A, Aff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Consumer.lift<RT, IN, B>(f(a))
            select project(a, b);

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<RT, OUT, A> ma, Func<A, Aff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Producer.lift<RT, OUT, B>(f(a))
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<RT, IN, OUT, A> ma, Func<A, Aff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select project(a, b);
        
        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this ConsumerLift<RT, IN, A> ma, Func<A, Eff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, C>.Lift<B>(f(a), b => ConsumerLiftPure<RT, IN, C>(project(a, b))));

        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<IN, A> ma, Func<A, Eff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, C>.Lift<B>(f(a), b => ConsumerLiftPure<RT, IN, C>(project(a, b))));

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<OUT, A> ma, Func<A, Eff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT>()
            from b in Producer.lift<RT, OUT, B>(f(a))
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<IN, OUT, A> ma, Func<A, Eff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT>()
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select project(a, b);

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<RT, IN, A> ma, Func<A, Eff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Consumer.lift<RT, IN, B>(f(a))
            select project(a, b);

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<RT, OUT, A> ma, Func<A, Eff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Producer.lift<RT, OUT, B>(f(a))
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<RT, IN, OUT, A> ma, Func<A, Eff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select project(a, b);
        
        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this ConsumerLift<RT, IN, A> ma, Func<A, Aff<B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, C>.Lift<B>(f(a), b => ConsumerLiftPure<RT, IN, C>(project(a, b))));

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<RT, IN, A> ma, Func<A, Aff<B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Consumer.lift<RT, IN, B>(f(a))
            select project(a, b);

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<RT, OUT, A> ma, Func<A, Aff<B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Producer.lift<RT, OUT, B>(f(a))
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<RT, IN, OUT, A> ma, Func<A, Aff<B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select project(a, b);
        
        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this ConsumerLift<RT, IN, A> ma, Func<A, Eff<B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => new ConsumerLift<RT, IN, C>.Lift<B>(f(a), b => ConsumerLiftPure<RT, IN, C>(project(a, b))));

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<RT, IN, A> ma, Func<A, Eff<B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Consumer.lift<RT, IN, B>(f(a))
            select project(a, b);

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<RT, OUT, A> ma, Func<A, Eff<B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Producer.lift<RT, OUT, B>(f(a))
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<RT, IN, OUT, A> ma, Func<A, Eff<B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in Pipe.lift<RT, IN, OUT, B>(f(a))
            select project(a, b);

        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Aff<RT, A> ma, Func<A, ConsumerLift<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, C>.Lift<A>(ma, a => f(a).Map(b => project(a, b)));

        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Aff<RT, A> ma, Func<A, Consumer<IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, C>.Lift<A>(ma, a => f(a).ToConsumerLift<RT>().Map(b => project(a, b)));

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Aff<RT, A> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a).Interpret<RT>()
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Aff<RT, A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a).Interpret<RT>()
            select project(a, b);

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Aff<RT, A> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Consumer.lift<RT, IN, A>(ma)
            from b in f(a)
            select project(a, b);

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Aff<RT, A> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a)
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Aff<RT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a)
            select project(a, b);

        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Eff<RT, A> ma, Func<A, ConsumerLift<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, C>.Lift<A>(ma, a => f(a).Map(b => project(a, b)));

        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Eff<RT, A> ma, Func<A, Consumer<IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, C>.Lift<A>(ma, a => f(a).ToConsumerLift<RT>().Map(b => project(a, b)));

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Eff<RT, A> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a).Interpret<RT>()
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Eff<RT, A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a).Interpret<RT>()
            select project(a, b);

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Eff<RT, A> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Consumer.lift<RT, IN, A>(ma)
            from b in f(a)
            select project(a, b);

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Eff<RT, A> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a)
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Eff<RT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a)
            select project(a, b);

        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Aff<A> ma, Func<A, ConsumerLift<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, C>.Lift<A>(ma, a => f(a).Map(b => project(a, b)));

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Aff<A> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Consumer.lift<RT, IN, A>(ma)
            from b in f(a)
            select project(a, b);

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Aff<A> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a)
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Aff<A> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a)
            select project(a, b);

        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Eff<A> ma, Func<A, ConsumerLift<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            new ConsumerLift<RT, IN, C>.Lift<A>(ma, a => f(a).Map(b => project(a, b)));

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Eff<A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a).Interpret<RT>()
            select project(a, b);

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Eff<A> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Consumer.lift<RT, IN, A>(ma)
            from b in f(a)
            select project(a, b);

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Eff<A> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Producer.lift<RT, OUT, A>(ma)
            from b in f(a)
            select project(a, b);

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Eff<A> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in Pipe.lift<RT, IN, OUT, A>(ma)
            from b in f(a)
            select project(a, b);
    
    }
}
