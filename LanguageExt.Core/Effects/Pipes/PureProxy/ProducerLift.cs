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
    public abstract class ProducerLift<RT, OUT, A> where RT : struct, HasCancel<RT>
    {
        public abstract ProducerLift<RT, OUT, B> Select<B>(Func<A, B> f);
        public abstract ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f);
        public abstract ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, ProducerLift<RT, OUT, B>> f);
        public abstract Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<RT, OUT, B>> f);
        public abstract Producer<RT, OUT, A> Interpret();
        public abstract Pipe<RT, IN, OUT, A> ToPipe<IN>();

        public ProducerLift<RT, OUT, B> Map<B>(Func<A, B> f) => Select(f);
        public ProducerLift<RT, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) => SelectMany(f);
        public ProducerLift<RT, OUT, B> Bind<B>(Func<A, ProducerLift<RT, OUT, B>> f) => SelectMany(f);
        public Producer<RT, OUT, B> Bind<B>(Func<A, Producer<RT, OUT, B>> f) => SelectMany(f);

        public ProducerLift<RT, OUT, C> SelectMany<B, C>(Func<A, ProducerLift<RT, OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
                        
        public static implicit operator ProducerLift<RT, OUT, A>(Pipes.Pure<A> ma) =>
            new ProducerLift<RT, OUT, A>.Pure(ma.Value);

        public class Pure : ProducerLift<RT, OUT, A> 
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;

            public override ProducerLift<RT, OUT, B> Select<B>(Func<A, B> f) =>
                new ProducerLift<RT, OUT, B>.Pure(f(Value));

            public override ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                f(Value).ToProducerLift<RT>();

            public override ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, ProducerLift<RT, OUT, B>> f) =>
                f(Value);

            public override Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<RT, OUT, B>> f) =>
                f(Value);

            public override Producer<RT, OUT, A> Interpret() =>
                Producer.Pure<RT, OUT, A>(Value);

            public override Pipe<RT, IN, OUT, A> ToPipe<IN>() =>
                Pipe.Pure<RT, IN, OUT, A>(Value);
        }

        public class Enumerate<X> : ProducerLift<RT, OUT, A> 
        {
            internal readonly EnumerateData<X> Values;
            public readonly Func<X, ProducerLift<RT, OUT, A>> Next;
            
            internal Enumerate(EnumerateData<X> values, Func<X, ProducerLift<RT, OUT, A> > next) =>
                (Values, Next) = (values, next);

            public Enumerate(IEnumerable<X> values, Func<X, ProducerLift<RT, OUT, A> > next) =>
                (Values, Next) = (new EnumerateEnumerable<X>(values), next);

            public Enumerate(IAsyncEnumerable<X> values, Func<X, ProducerLift<RT, OUT, A> > next) =>
                (Values, Next) = (new EnumerateAsyncEnumerable<X>(values), next);

            public Enumerate(IObservable<X> values, Func<X, ProducerLift<RT, OUT, A> > next) =>
                (Values, Next) = (new EnumerateObservable<X>(values), next);

            public override ProducerLift<RT, OUT, B> Select<B>(Func<A, B> f) =>
                new ProducerLift<RT, OUT, B>.Enumerate<X>(Values, n => Next(n).Select(f));

            public override ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new ProducerLift<RT, OUT, B>.Enumerate<X>(Values, n => Next(n).SelectMany(f));

            public override ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, ProducerLift<RT, OUT, B>> f) =>
                new ProducerLift<RT, OUT, B>.Enumerate<X>(Values, n => Next(n).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<RT, OUT, B>> f) =>
                Interpret().Bind(f).ToProducer();

            public override Producer<RT, OUT, A> Interpret() =>
                Producer.enumerate<RT, OUT, X>(Values)
                        .Bind(x => Next(x).Interpret()).ToProducer();

            public override Pipe<RT, IN, OUT, A> ToPipe<IN>() =>
                Pipe.enumerate<RT, IN, OUT, X>(Values).Bind(x => Next(x).ToPipe<IN>());
        }
        
        public class Yield : ProducerLift<RT, OUT, A>
        {
            public readonly OUT Value;
            public readonly Func<Unit, ProducerLift<RT, OUT, A>> Next;
            
            public Yield(OUT value, Func<Unit, ProducerLift<RT, OUT, A>> next) =>
                (Value, Next) = (value, next);

            public override ProducerLift<RT, OUT, B> Select<B>(Func<A, B> f) =>
                new ProducerLift<RT, OUT, B>.Yield(Value, n => Next(n).Select(f));

            public override ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new ProducerLift<RT, OUT, B>.Yield(Value, n => Next(n).SelectMany(f));

            public override ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, ProducerLift<RT, OUT, B>> f) =>
                new ProducerLift<RT, OUT, B>.Yield(Value, n => Next(n).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<RT, OUT, B>> f) =>
                Interpret().Bind(f).ToProducer();

            public override Producer<RT, OUT, A> Interpret() =>
                Producer.yield<RT, OUT>(Value).Bind(x => Next(x).Interpret()).ToProducer();

            public override Pipe<RT, IN, OUT, A> ToPipe<IN>() =>
                Pipe.yield<RT, IN, OUT>(Value).Bind(x => Next(x).ToPipe<IN>());
        }

        public class Release<X> : ProducerLift<RT, OUT, A>
        {
            readonly X Value;
            readonly Func<Unit, ProducerLift<RT, OUT, A>> Next;

            public Release(X value, Func<Unit, ProducerLift<RT, OUT, A>> next)
            {
                Value = value;
                Next  = next;
            }

            public override ProducerLift<RT, OUT, B> Select<B>(Func<A, B> f) =>
                new ProducerLift<RT, OUT, B>.Release<X>(Value, x => Next(x).Select(f));

            public override ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new ProducerLift<RT, OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, ProducerLift<RT, OUT, B>> f) =>
                new ProducerLift<RT, OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<RT, OUT, B>> f) =>
                Producer.release<RT, OUT, X>(Value).SelectMany(x => Next(x).Bind(f));

            public override Producer<RT, OUT, A> Interpret() =>
                Producer.release<RT, OUT, X>(Value).SelectMany(x => Next(x).Interpret());

            public override Pipe<RT, IN, OUT, A> ToPipe<IN>() =>
                Pipe.release<RT, IN, OUT, X>(Value).SelectMany(x => Next(x).ToPipe<IN>());
        }    
        
        public class Lift<X> : ProducerLift<RT, OUT, A> 
        {
            public readonly Aff<RT, X> Value;
            public readonly Func<X, ProducerLift<RT, OUT, A>> Next;

            public Lift(Aff<RT, X> value, Func<X, ProducerLift<RT, OUT, A>> next) =>
                (Value, Next) = (value, next);

            public override ProducerLift<RT, OUT, B> Select<B>(Func<A, B> f) =>
                new ProducerLift<RT, OUT, B>.Lift<X>(Value, x => Next(x).Select(f));

            public override ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new ProducerLift<RT, OUT, B>.Lift<X>(Value, x => Next(x).SelectMany(f));

            public override ProducerLift<RT, OUT, B> SelectMany<B>(Func<A, ProducerLift<RT, OUT, B>> f) =>
                new ProducerLift<RT, OUT, B>.Lift<X>(Value, x => Next(x).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<RT, OUT, B>> f) =>
                Producer.lift<RT, OUT, X>(Value).SelectMany(x => Next(x).Bind(f));

            public override Producer<RT, OUT, A> Interpret() =>
                Producer.lift<RT, OUT, X>(Value).SelectMany(x => Next(x).Interpret());

            public override Pipe<RT, IN, OUT, A> ToPipe<IN>() =>
                Pipe.lift<RT, IN, OUT, X>(Value).SelectMany(x => Next(x).ToPipe<IN>());
        }
    }
}
