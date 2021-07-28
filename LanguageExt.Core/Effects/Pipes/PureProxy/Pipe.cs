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

        public static implicit operator Pipe<IN, OUT, A>(Pipes.Pure<A> ma) =>
            new Pipe<IN, OUT, A>.Pure(ma.Value);

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
                f(Value).ToPipe<IN>();
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
                from x in Pipe.awaiting<RT, IN, OUT>()
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
}
