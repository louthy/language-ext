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
