//
// This file contains a number of micro-free-monads that allow for creation of pure producers, consumers, and pipes.  
// They're used to facilitate the building of Proxy derived types without the need for typing the generic arguments endlessly
// The Haskell original could auto-infer the generic parameter types, the system here tries to replicate manually what
// Haskell can do automatically.  Hence why there are so many implementations of SelectMany!
//

using System;
using LanguageExt.Effects.Traits;
using System.Collections.Generic;
using LanguageExt.Common;

namespace LanguageExt.Pipes
{
    public abstract class Producer<OUT, A>
    {
        public abstract Producer<OUT, B> Select<B>(Func<A, B> f);
        public abstract Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f);
        public abstract Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : HasIO<RT, Error>;
        public abstract Producer<RT, OUT, A> Interpret<RT>() where RT : HasIO<RT, Error>;
        public abstract Pipe<IN, OUT, A> ToPipe<IN>();
        public abstract ProducerLift<RT, OUT, A> ToProducerLift<RT>() where RT : HasIO<RT, Error>;

        public Producer<OUT, B> Map<B>(Func<A, B> f) => Select(f);
        public Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) => SelectMany(f);
        public Producer<RT, OUT, B> Bind<RT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : HasIO<RT, Error> => SelectMany(f);

        public Producer<OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public ProducerLift<RT, OUT, B> SelectMany<RT, B>(Func<A, ProducerLift<RT, OUT, B>> f) where RT : HasIO<RT, Error> =>
            ToProducerLift<RT>().SelectMany(f);

        public ProducerLift<RT, OUT, C> SelectMany<RT, B, C>(Func<A, ProducerLift<RT, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Producer<RT, OUT, C> SelectMany<RT, B, C>(Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
            SelectMany(a => f(a).Select(b => project(a, b)));
                        
        public static implicit operator Producer<OUT, A>(Pure<A> ma) =>
            new Pure(ma.Value);

        public static Producer<OUT, A> operator &(
            Producer<OUT, A> lhs,
            Producer<OUT, A> rhs) =>
            lhs.Bind(_ => rhs);

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

            public override Pipe<IN, OUT, A> ToPipe<IN>() =>
                new Pipe<IN, OUT, A>.Pure(Value);

            public override ProducerLift<RT, OUT, A> ToProducerLift<RT>() =>
                new ProducerLift<RT, OUT, A>.Pure(Value);
        }

        public class Enumerate : Producer<OUT, A> 
        {
            internal readonly EnumerateData<OUT> Values;
            public readonly Func<Unit, Producer<OUT, A> > Next;
            
            internal Enumerate(EnumerateData<OUT> values, Func<Unit, Producer<OUT, A> > next) =>
                (Values, Next) = (values, next);

            public Enumerate(IEnumerable<OUT> values, Func<Unit, Producer<OUT, A> > next) =>
                (Values, Next) = (new EnumerateEnumerable<OUT>(values), next);

            public Enumerate(IAsyncEnumerable<OUT> values, Func<Unit, Producer<OUT, A> > next) =>
                (Values, Next) = (new EnumerateAsyncEnumerable<OUT>(values), next);

            public Enumerate(IObservable<OUT> values, Func<Unit, Producer<OUT, A> > next) =>
                (Values, Next) = (new EnumerateObservable<OUT>(values), next);

            public override Producer<OUT, B> Select<B>(Func<A, B> f) =>
                new Producer<OUT, B>.Enumerate(Values, n => Next(n).Select(f));

            public override Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Producer<OUT, B>.Enumerate(Values, n => Next(n).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                Interpret<RT>().Bind(f).ToProducer();

            public override Producer<RT, OUT, A> Interpret<RT>() =>
                Producer.yieldAll<RT, OUT>(Values)
                        .Bind(x => Next(x).Interpret<RT>()).ToProducer();

            public override Pipe<IN, OUT, A> ToPipe<IN>() =>
                new Pipe<IN, OUT, A>.Enumerate(Values, x => Next(x).ToPipe<IN>());

            public override ProducerLift<RT, OUT, A> ToProducerLift<RT>() =>
                new ProducerLift<RT, OUT, A>.Enumerate(Values, x => Next(x).ToProducerLift<RT>());
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

            public override Pipe<IN, OUT, A> ToPipe<IN>() =>
                new Pipe<IN, OUT, A>.Yield(Value, x => Next(x).ToPipe<IN>());

            public override ProducerLift<RT, OUT, A> ToProducerLift<RT>() =>
                new ProducerLift<RT, OUT, A>.Yield(Value, x => Next(x).ToProducerLift<RT>());
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

            public override Pipe<IN, OUT, A> ToPipe<IN>() =>
                new Pipe<IN, OUT, A>.Release<X>(Value, x => Next(x).ToPipe<IN>());

            public override ProducerLift<RT, OUT, A> ToProducerLift<RT>() =>
                new ProducerLift<RT, OUT, A>.Release<X>(Value, x => Next(x).ToProducerLift<RT>());
        }    
    }
}
