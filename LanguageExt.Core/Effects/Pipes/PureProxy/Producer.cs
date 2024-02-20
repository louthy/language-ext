//
// This file contains a number of micro-free-monads that allow for creation of pure producers, consumers, and pipes.  
// They're used to facilitate the building of Proxy derived types without the need for typing the generic arguments endlessly
// The Haskell original could auto-infer the generic parameter types, the system here tries to replicate manually what
// Haskell can do automatically.  Hence why there are so many implementations of SelectMany!
//

using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public abstract class Producer<OUT, A>
{
    public abstract Producer<OUT, B> Select<B>(Func<A, B> f);
    
    public abstract Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f);
    public abstract Producer<OUT, M, B> Bind<M, B>(Func<A, Producer<OUT, M, B>> f) where M : Monad<M>;
    
    public abstract Producer<OUT, M, A> Interpret<M>() where M : Monad<M>;
    public abstract Pipe<IN, OUT, A> ToPipe<IN>();

    public Producer<OUT, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);
  
    public Producer<OUT, B> Bind<B>(Func<A, Fail<Error>> f) =>
        Bind(x => new Producer<OUT, B>.Fail(f(x).Value));

    public Producer<OUT, M, B> Bind<M, B>(Func<A, K<M, B>> f) 
        where M : Monad<M> =>
        Interpret<M>().Bind(f);

    public Producer<OUT, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => PureProxy.ProducerLiftIO<OUT, B>(f(x)));
    
    public Producer<OUT, B> Map<B>(Func<A, B> f) => 
        Select(f);
    
    public Producer<OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Producer<OUT, M, C> SelectMany<M, B, C>(Func<A, Producer<OUT, M, B>> f, Func<A, B, C> project) where M : Monad<M> =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Producer<OUT, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> project) =>
        Map(a => project(a, f(a).Value));
        
    public Producer<OUT, C> SelectMany<B, C>(Func<A, Fail<Error>> f, Func<A, B, C> project) =>
        Bind<C>(f);
    
    public Producer<OUT, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> project) =>
        Bind(x => f(x).Map(y => project(x, y)));
        
    public Producer<OUT, M, C> SelectMany<B, M, C>(Func<A, K<M, B>> f, Func<A, B, C> project)
        where M : Monad<M> =>
        Bind(a => M.Map(b => project(a, b), f(a)));
                        
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

        public override Producer<OUT, M, B> Bind<M, B>(Func<A, Producer<OUT, M, B>> f) =>
            f(Value);

        public override Producer<OUT, M, A> Interpret<M>() =>
            Producer.Pure<OUT, M, A>(Value);

        public override Pipe<IN, OUT, A> ToPipe<IN>() =>
            new Pipe<IN, OUT, A>.Pure(Value);
    }

    public class Fail(Error Error) : Producer<OUT, A>
    {
        public override Producer<OUT, B> Select<B>(Func<A, B> _) =>
            new Producer<OUT, B>.Fail(Error);

        public override Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) => 
            new Producer<OUT, B>.Fail(Error);

        public override Producer<OUT, M, B> Bind<M, B>(Func<A, Producer<OUT, M, B>> f) =>
            PureProxy.ProducerLift<OUT, A>(Error.Throw<A>).Bind(f);

        public override Producer<OUT, M, A> Interpret<M>() => 
            PureProxy.ProducerLift<OUT, A>(Error.Throw<A>);

        public override Pipe<IN, OUT, A> ToPipe<IN>() => 
            new Pipe<IN, OUT, A>.Fail(Error);
    }

    public class Lift<X>(Func<X> Function, Func<X, Producer<OUT, A>> Next) : Producer<OUT, A>
    {
        public override Producer<OUT, B> Select<B>(Func<A, B> f) => 
            new Producer<OUT, B>.Lift<X>(Function, x => Next(x).Select(f));

        public override Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) => 
            new Producer<OUT, B>.Lift<X>(Function, x => Next(x).Bind(f));

        public override Producer<OUT, M, B> Bind<M, B>(Func<A, Producer<OUT, M, B>> f) => 
            Producer.lift<OUT, M, X>(M.Pure(Function())).SelectMany(x => Next(x).Bind(f)).ToProducer();

        public override Producer<OUT, M, A> Interpret<M>() => 
            Producer.lift<OUT, M, X>(M.Pure(Function())).Bind(x => Next(x).Interpret<M>());

        public override Pipe<IN, OUT, A> ToPipe<IN>() =>
            new Pipe<IN, OUT, A>.Lift<X>(Function, x => Next(x).ToPipe<IN>());
    }

    public class LiftIO<X>(IO<X> Effect, Func<X, Producer<OUT, A>> Next) : Producer<OUT, A>
    {
        public override Producer<OUT, B> Select<B>(Func<A, B> f) => 
            new Producer<OUT, B>.LiftIO<X>(Effect, x => Next(x).Select(f));

        public override Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) => 
            new Producer<OUT, B>.LiftIO<X>(Effect, x => Next(x).Bind(f));

        public override Producer<OUT, M, B> Bind<M, B>(Func<A, Producer<OUT, M, B>> f) => 
            Producer.lift<OUT, M, X>(M.LiftIO(Effect)).SelectMany(x => Next(x).Bind(f)).ToProducer();

        public override Producer<OUT, M, A> Interpret<M>() => 
            Producer.lift<OUT, M, X>(M.LiftIO(Effect)).Bind(x => Next(x).Interpret<M>());

        public override Pipe<IN, OUT, A> ToPipe<IN>() =>
            new Pipe<IN, OUT, A>.LiftIO<X>(Effect, x => Next(x).ToPipe<IN>());
    }

    public class Yield(OUT Value, Func<Unit, Producer<OUT, A>> Next) : Producer<OUT, A>
    {
        public override Producer<OUT, B> Select<B>(Func<A, B> f) =>
            new Producer<OUT, B>.Yield(Value, n => Next(n).Select(f));

        public override Producer<OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            new Producer<OUT, B>.Yield(Value, n => Next(n).Bind(f));

        public override Producer<OUT, M, B> Bind<M, B>(Func<A, Producer<OUT, M, B>> f) =>
            Interpret<M>().Bind(f).ToProducer();

        public override Producer<OUT, M, A> Interpret<M>() =>
            Producer.yield<OUT, M>(Value).Bind(x => Next(x).Interpret<M>()).ToProducer();

        public override Pipe<IN, OUT, A> ToPipe<IN>() =>
            new Pipe<IN, OUT, A>.Yield(Value, x => Next(x).ToPipe<IN>());
    }
}
