//
// This file contains a number of micro-free-monads that allow for creation of pure producers, consumers, and pipes.  
// They're used to facilitate the building of Proxy derived types without the need for typing the generic arguments endlessly
// The Haskell original could auto-infer the generic parameter types, the system here tries to replicate manually what
// Haskell can do automatically.  Hence why there are so many implementations of SelectMany!
//

using System;
using LanguageExt.Effects.Traits;
using System.Collections.Generic;

namespace LanguageExt.Pipes
{
    public abstract class Enumerate<OUT, A>
    {
        public abstract Enumerate<OUT, B> Select<B>(Func<A, B> f);
        public abstract Enumerate<OUT, B> SelectMany<B>(Func<A, Enumerate<OUT, B>> f);
        public abstract Enumerate<OUT, B> SelectMany<B>(Func<A, Pipes.Release<B>> f);
        public abstract Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f);
        public abstract Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Pipe<IN, OUT, B> SelectMany<IN, B>(Func<A, Pipe<IN, OUT, B>> f);
        public abstract Pipe<RT, IN, OUT, B> SelectMany<RT, IN, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT>;
        public abstract Producer<RT, OUT, A> Interpret<RT>() where RT : struct, HasCancel<RT>;
        public abstract Pipe<RT, IN, OUT, A> Interpret<RT, IN>() where RT : struct, HasCancel<RT>;
        
        public Enumerate<OUT, B> Map<B>(Func<A, B> f) => 
            Select(f);
        
        public Enumerate<OUT, B> Bind<B>(Func<A, Enumerate<OUT, B>> f) => 
            SelectMany(f);
        
        public Enumerate<OUT, B> Bind<B>(Func<A, Pipes.Release<B>> f) => 
            SelectMany(f);
        
        public Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) => 
            SelectMany(f);
        
        public Producer<RT, OUT, B> Bind<RT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT> => 
            SelectMany(f);
        
        public Pipe<RT, IN, OUT, B> Bind<RT, IN, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT> => 
            SelectMany(f);
        
        public Enumerate<OUT, C> SelectMany<B, C>(Func<A, Enumerate<OUT, B>> f, Func<A, B, C> project) => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Enumerate<OUT, C> SelectMany<B, C>(Func<A, Pipes.Release<B>> f, Func<A, B, C> project) => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Producer<OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Producer<RT, OUT, C> SelectMany<RT, B, C>(Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<IN, OUT, C> SelectMany<IN, B, C>(Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) => 
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<RT, IN, OUT, C> SelectMany<RT, IN, B, C>(Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> => 
            SelectMany(a => f(a).Select(b => project(a, b)));
                
        public static implicit operator Enumerate<OUT, A>(Pure<A> ma) =>
            new Pure(ma.Value);

        public class Pure : Enumerate<OUT, A> 
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;
            
            public override Enumerate<OUT, B> Select<B>(Func<A, B> f) =>
                new Enumerate<OUT, B>.Pure(f(Value));

            public override Enumerate<OUT, B> SelectMany<B>(Func<A, Pipes.Release<B>> f) =>
                f(Value).ToEnumerate<OUT>();

            public override Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                f(Value);

            public override Enumerate<OUT, B> SelectMany<B>(Func<A, Enumerate<OUT, B>> f) =>
                f(Value);

            public override Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                f(Value);

            public override Pipe<IN, OUT, B> SelectMany<IN, B>(Func<A, Pipe<IN, OUT, B>> f) =>
                f(Value);

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, IN, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                f(Value);

            public override Producer<RT, OUT, A> Interpret<RT>() =>
                Producer.Pure<RT, OUT, A>(Value);

            public override Pipe<RT, IN, OUT, A> Interpret<RT, IN>() =>
                Pipe.Pure<RT, IN, OUT, A>(Value);
        }

        public class Do : Enumerate<OUT, A>
        {
            internal readonly EnumerateData<OUT> Values;
            public readonly Func<Unit, Enumerate<OUT, A>> Next;
            
            internal Do(EnumerateData<OUT> values, Func<Unit, Enumerate<OUT, A>> next) =>
                (Values, Next) = (values, next);
            
            public Do(IEnumerable<OUT> values, Func<Unit, Enumerate<OUT, A>> next) =>
                (Values, Next) = (new EnumerateEnumerable<OUT>(values), next);
            
            public Do(IAsyncEnumerable<OUT> values, Func<Unit, Enumerate<OUT, A>> next) =>
                (Values, Next) = (new EnumerateAsyncEnumerable<OUT>(values), next);
            
            public Do(IObservable<OUT> values, Func<Unit, Enumerate<OUT, A>> next) =>
                (Values, Next) = (new EnumerateObservable<OUT>(values), next);

            public override Enumerate<OUT, B> Select<B>(Func<A, B> f) =>
                new Enumerate<OUT, B>.Do(Values, n => Next(n).Select(f));

            public override Enumerate<OUT, B> SelectMany<B>(Func<A, Enumerate<OUT, B>> f) =>
                new Enumerate<OUT, B>.Do(Values, x => Next(x).SelectMany(f));

            public override Enumerate<OUT, B> SelectMany<B>(Func<A, Pipes.Release<B>> f) =>
                new Enumerate<OUT, B>.Do(Values, x => Next(x).SelectMany(f));

            public override Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Producer<OUT, B>.Enumerate(Values, x => Next(x).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                Interpret<RT>().Bind(f).ToProducer();

            public override Pipe<IN, OUT, B> SelectMany<IN, B>(Func<A, Pipe<IN, OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Enumerate(Values, x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, IN, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                Interpret<RT, IN>().Bind(f).ToPipe();

            public override Producer<RT, OUT, A> Interpret<RT>() =>
                Producer.yieldAll<RT, OUT>(Values).Bind(x => Next(x).Interpret<RT>()).ToProducer();

            public override Pipe<RT, IN, OUT, A> Interpret<RT, IN>() =>
                Pipe.yieldAll<RT, IN, OUT>(Values).Bind(x => Next(x).Interpret<RT, IN>()).ToPipe();
        }

        public class Release<X> : Enumerate<OUT, A>
        {
            readonly X Value;
            readonly Func<Unit, Enumerate<OUT, A>> Next;

            public Release(X value, Func<Unit, Enumerate<OUT, A>> next)
            {
                Value = value;
                Next  = next;
            }

            public override Enumerate<OUT, B> Select<B>(Func<A, B> f) =>
                new Enumerate<OUT, B>.Release<X>(Value, x => Next(x).Select(f));

            public override Enumerate<OUT, B> SelectMany<B>(Func<A, Enumerate<OUT, B>> f) =>
                new Enumerate<OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Enumerate<OUT, B> SelectMany<B>(Func<A, Pipes.Release<B>> f) =>
                new Enumerate<OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Producer<OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Producer<OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Producer<RT, OUT, B> SelectMany<RT, B>(Func<A, Producer<RT, OUT, B>> f) =>
                Producer.release<RT, OUT, X>(Value).Bind(x => Next(x).SelectMany(f)).ToProducer();

            public override Pipe<IN, OUT, B> SelectMany<IN, B>(Func<A, Pipe<IN, OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Release<X>(Value, x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, IN, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                Pipe.release<RT, IN, OUT, X>(Value).Bind(x => Next(x).SelectMany(f)).ToPipe();

            public override Producer<RT, OUT, A> Interpret<RT>() =>
                Producer.release<RT, OUT, X>(Value).Bind(x => Next(x).Interpret<RT>()).ToProducer();

            public override Pipe<RT, IN, OUT, A> Interpret<RT, IN>() =>
                Pipe.release<RT, IN, OUT, X>(Value).Bind(x => Next(x).Interpret<RT, IN>()).ToPipe();
        }
    }
}
