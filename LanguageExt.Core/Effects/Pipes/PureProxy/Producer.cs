//
// This file contains a number of micro-free-monads that allow for creation of pure producers, consumers, and pipes.  
// They're used to facilitate the building of Proxy derived types without the need for typing the generic arguments endlessly
// The Haskell original could auto-infer the generic parameter types, the system here tries to replicate manually what
// Haskell can do automatically.  Hence why there are so many implementations of SelectMany!
//

using System;
using LanguageExt.Effects.Traits;
using LanguageExt.Common;

namespace LanguageExt.Pipes;

public abstract class Producer<OUT, A>
{
    public abstract Producer<OUT, B> Select<B>(Func<A, B> f);
    
    public abstract Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f);
    public abstract Producer<RT, OUT, B> Bind<RT, B>(Func<A, Producer<RT, OUT, B>> f) where RT : HasIO<RT, Error>;
    
    public abstract Producer<RT, OUT, A> Interpret<RT>() where RT : HasIO<RT, Error>;
    public abstract Pipe<IN, OUT, A> ToPipe<IN>();

    public Producer<OUT, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);
 
    public Producer<OUT, B> Bind<B>(Func<A, Transducer<Unit, B>> f) =>
        Bind(a => f(a).Map(Sum<Error, B>.Right));
 
    public Producer<OUT, B> Bind<B>(Func<A, Transducer<Unit, Sum<Error, B>>> f) =>
        Bind(a => new Producer<OUT, B>.Lift<B>(f(a), PureProxy.ProducerPure<OUT, B>));
 
    public Producer<RT, OUT, B> Bind<RT, B>(Func<A, Transducer<RT, B>> f) 
        where RT : HasIO<RT, Error> =>
        Interpret<RT>().Bind(f);

    public Producer<RT, OUT, B> Bind<RT, B>(Func<A, Transducer<RT, Sum<Error, B>>> f) 
        where RT : HasIO<RT, Error> =>
        Interpret<RT>().Bind(f);
    
    public Producer<OUT, B> Map<B>(Func<A, B> f) => 
        Select(f);
    
    public Producer<OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Producer<RT, OUT, C> SelectMany<RT, B, C>(Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Producer<OUT, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> project) =>
        Map(a => project(a, f(a).Value));
        
    public Producer<OUT, C> SelectMany<B, C>(Func<A, Transducer<Unit, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Producer<OUT, C> SelectMany<B, C>(Func<A, Transducer<Unit, Sum<Error, B>>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(mb => mb.Map(b => project(a, b))));
        
    public Producer<RT, OUT, C> SelectMany<RT, B, C>(Func<A, Transducer<RT, B>> f, Func<A, B, C> project)
        where RT : HasIO<RT, Error> =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Producer<RT, OUT, C> SelectMany<RT, B, C>(Func<A, Transducer<RT, Sum<Error, B>>> f, Func<A, B, C> project)
        where RT : HasIO<RT, Error> =>
        Bind(a => f(a).Select(mb => mb.Map(b => project(a, b))));
                        
    public static implicit operator Producer<OUT, A>(Pure<A> ma) =>
        new Pure(ma.Value);

    public static Producer<OUT, A> operator &(
        Producer<OUT, A> lhs,
        Producer<OUT, A> rhs) =>
        lhs.Bind(_ => rhs);

    public class Pure(A Value) : Producer<OUT, A>
    {
        public override Producer<OUT, B> Select<B>(Func<A, B> f) =>
            new Producer<OUT, B>.Pure(f(Value));

        public override Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            f(Value);

        public override Producer<RT, OUT, B> Bind<RT, B>(Func<A, Producer<RT, OUT, B>> f) =>
            f(Value);

        public override Producer<RT, OUT, A> Interpret<RT>() =>
            Producer.Pure<RT, OUT, A>(Value);

        public override Pipe<IN, OUT, A> ToPipe<IN>() =>
            new Pipe<IN, OUT, A>.Pure(Value);
    }

    public class Lift<X>(Transducer<Unit, Sum<Error, X>> Morphism, Func<X, Producer<OUT, A>> Next) : Producer<OUT, A>
    {
        public override Producer<OUT, B> Select<B>(Func<A, B> f) => 
            new Producer<OUT, B>.Lift<X>(Morphism, x => Next(x).Select(f));

        public override Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) => 
            new Producer<OUT, B>.Lift<X>(Morphism, x => Next(x).Bind(f));

        public override Producer<RT, OUT, B> Bind<RT, B>(Func<A, Producer<RT, OUT, B>> f) => 
            Producer.lift<RT, OUT, X>(Morphism).SelectMany(x => Next(x).Bind(f)).ToProducer();

        public override Producer<RT, OUT, A> Interpret<RT>() => 
            Producer.lift<RT, OUT, X>(Morphism).Bind(x => Next(x).Interpret<RT>());

        public override Pipe<IN, OUT, A> ToPipe<IN>() =>
            new Pipe<IN, OUT, A>.Lift<X>(Morphism, x => Next(x).ToPipe<IN>());
    }

    public class Yield(OUT Value, Func<Unit, Producer<OUT, A>> Next) : Producer<OUT, A>
    {
        public override Producer<OUT, B> Select<B>(Func<A, B> f) =>
            new Producer<OUT, B>.Yield(Value, n => Next(n).Select(f));

        public override Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            new Producer<OUT, B>.Yield(Value, n => Next(n).Bind(f));

        public override Producer<RT, OUT, B> Bind<RT, B>(Func<A, Producer<RT, OUT, B>> f) =>
            Interpret<RT>().Bind(f).ToProducer();

        public override Producer<RT, OUT, A> Interpret<RT>() =>
            Producer.yield<RT, OUT>(Value).Bind(x => Next(x).Interpret<RT>()).ToProducer();

        public override Pipe<IN, OUT, A> ToPipe<IN>() =>
            new Pipe<IN, OUT, A>.Yield(Value, x => Next(x).ToPipe<IN>());
    }
}
