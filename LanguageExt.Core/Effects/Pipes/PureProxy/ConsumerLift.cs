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
        
        public static implicit operator ConsumerLift<RT, IN, A>(Pipes.Pure<A> ma) =>
            new ConsumerLift<RT, IN, A>.Pure(ma.Value);

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
                f(Value).ToPipe<IN>();

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
                Pipe.awaiting<RT, IN, OUT>().SelectMany(x => Next(x).SelectMany(f));

            public override Consumer<RT, IN, A> Interpret() =>
                Consumer.awaiting<RT, IN>().Bind(x => Next(x).Interpret()).ToConsumer();

            public override Pipe<RT, IN, OUT, A> ToPipe<OUT>() =>
                Pipe.awaiting<RT, IN, OUT>().Bind(x => Next(x).ToPipe<OUT>()).ToPipe();
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
}
