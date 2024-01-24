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
    public abstract class Pipe<IN, OUT, A>
    {
        public abstract Pipe<IN, OUT, B> Select<B>(Func<A, B> f);
        public abstract Pipe<RT, IN, OUT, A> Interpret<RT>() where RT : HasIO<RT, Error>;
        public abstract Pipe<IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> f);
        public abstract Pipe<RT, IN, OUT, B> SelectMany<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where RT : HasIO<RT, Error>;
        public abstract Pipe<IN, OUT, B> SelectMany<B>(Func<A, Consumer<IN, B>> f);
        public abstract Pipe<IN, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f);
        
        public Pipe<IN, OUT, B> Map<B>(Func<A, B> f) => Select(f);
        public Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) => SelectMany(f);
        public Pipe<RT, IN, OUT, B> Bind<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where RT : HasIO<RT, Error> => SelectMany(f);
        public Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f) => SelectMany(f);

        public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<RT, IN, OUT, C> SelectMany<RT, B, C>(Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));
        
        public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
            SelectMany(a => f(a).Select(b => project(a, b)));

        public static implicit operator Pipe<IN, OUT, A>(Pure<A> ma) =>
            new Pure(ma.Value);
        
        public static Pipe<IN, OUT, A> operator &(
            Pipe<IN, OUT, A> lhs,
            Pipe<IN, OUT, A> rhs) =>
            lhs.Bind(_ => rhs);

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

        public class Enumerate : Pipe<IN, OUT, A>
        {
            internal readonly EnumerateData<OUT> Values;
            public readonly Func<Unit, Pipe<IN, OUT, A>> Next;
            
            internal Enumerate(EnumerateData<OUT> values, Func<Unit, Pipe<IN, OUT, A>> next) =>
                (Values, Next) = (values, next);

            internal Enumerate(IEnumerable<OUT> values, Func<Unit, Pipe<IN, OUT, A>> next) =>
                (Values, Next) = (new EnumerateEnumerable<OUT>(values), next);

            internal Enumerate(IAsyncEnumerable<OUT> values, Func<Unit, Pipe<IN, OUT, A>> next) =>
                (Values, Next) = (new EnumerateAsyncEnumerable<OUT>(values), next);

            internal Enumerate(IObservable<OUT> values, Func<Unit, Pipe<IN, OUT, A>> next) =>
                (Values, Next) = (new EnumerateObservable<OUT>(values), next);

            public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) =>
                new Pipe<IN, OUT, B>.Enumerate(Values, x => Next(x).Select(f));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Enumerate(Values, x => Next(x).SelectMany(f));

            public override Pipe<RT, IN, OUT, B> SelectMany<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
                from x in Interpret<RT>()
                from r in f(x)
                select r;

            public override Pipe<RT, IN, OUT, A> Interpret<RT>() =>
                from x in Pipe.yieldAll<RT, IN, OUT>(Values)
                from r in Next(x)
                select r;

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Consumer<IN, B>> f) =>
                new Pipe<IN, OUT, B>.Enumerate(Values, x => Next(x).SelectMany(f));

            public override Pipe<IN, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> f) =>
                new Pipe<IN, OUT, B>.Enumerate(Values, x => Next(x).SelectMany(f));
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
