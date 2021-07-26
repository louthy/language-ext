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
                
        public static implicit operator Enumerate<A>(Pipes.Pure<A> ma) =>
            new Enumerate<A>.Pure(ma.Value);

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
}
