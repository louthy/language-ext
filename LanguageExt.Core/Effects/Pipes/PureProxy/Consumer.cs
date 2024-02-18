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

public abstract class Consumer<IN, A>
{
    public abstract Consumer<IN, B> Select<B>(Func<A, B> f);
    
    public abstract Consumer<IN, B> Bind<B>(Func<A, Consumer<IN, B>> f);
    public abstract Consumer<IN, M, B> Bind<M, B>(Func<A, Consumer<IN, M, B>> f) where M : Monad<M>;
    public abstract Pipe<IN, OUT, B> Bind<OUT, B>(Func<A, Producer<OUT, B>> f);
    
    public Consumer<IN, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);
 
    public Consumer<IN, B> Bind<B>(Func<A, Fail<Error>> f) =>
        Bind(x => new Consumer<IN, B>.Fail(f(x).Value));
 
    public Consumer<IN, M, B> Bind<M, B>(Func<A, K<M, B>> f) 
        where M : Monad<M> =>
        Interpret<M>().Bind(f);

    public Consumer<IN, M, B> Bind<M, B>(Func<A, IO<B>> f)
        where M : Monad<M> =>
        Bind(x => M.LiftIO(f(x)));
 
    public abstract Consumer<IN, M, A> Interpret<M>() 
        where M : Monad<M>;
    
    public abstract Pipe<IN, OUT, A> ToPipe<OUT>();
        
    public Consumer<IN, B> Map<B>(Func<A, B> f) => Select(f);
        
    public Consumer<IN, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> project) =>
        Map(x => project(x, f(x).Value));
 
    public Consumer<IN, C> SelectMany<B, C>(Func<A, Fail<Error>> f, Func<A, B, C> project) =>
        Bind<C>(f);

    public Consumer<IN, M, C> SelectMany<M, B, C>(Func<A, IO<B>> f, Func<A, B, C> project) 
        where M : Monad<M> =>
        SelectMany(x => M.LiftIO(f(x)), project);
 
    public Consumer<IN, M, C> SelectMany<M, B, C>(Func<A, K<M, B>> f, Func<A, B, C> project)
        where M : Monad<M> =>
        Bind(a => M.Map(b => project(a, b), f(a)));
 
    public Consumer<IN, C> SelectMany<B, C>(Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Consumer<IN, M, C> SelectMany<M, B, C>(Func<A, Consumer<IN, M, B>> f, Func<A, B, C> project) 
        where M : Monad<M> =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Pipe<IN, OUT, C> SelectMany<OUT, B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
       
    public static implicit operator Consumer<IN, A>(Pure<A> ma) =>
        new Pure(ma.Value);
       
    public static Consumer<IN, A> operator &(
        Consumer<IN, A> lhs,
        Consumer<IN, A> rhs) =>
        lhs.Bind(_ => rhs);

    public class Pure(A Value) : Consumer<IN, A>
    {
        public override Consumer<IN, B> Select<B>(Func<A, B> f) =>
            new Consumer<IN, B>.Pure(f(Value));

        public override Consumer<IN, B> Bind<B>(Func<A, Consumer<IN, B>> f) =>
            f(Value);

        public override Consumer<IN, M, B> Bind<M, B>(Func<A, Consumer<IN, M, B>> f) =>
            f(Value);

        public override Pipe<IN, OUT, B> Bind<OUT, B>(Func<A, Producer<OUT, B>> f) =>
            f(Value).ToPipe<IN>();

        public override Consumer<IN, M, A> Interpret<M>() =>
            Consumer.Pure<IN, M, A>(Value);

        public override Pipe<IN, OUT, A> ToPipe<OUT>() =>
            new Pipe<IN, OUT, A>.Pure(Value);
    }

    public class Fail(Error Error) : Consumer<IN, A>
    {
        public override Consumer<IN, B> Select<B>(Func<A, B> _) =>
            new Consumer<IN, B>.Fail(Error);

        public override Consumer<IN, B> Bind<B>(Func<A, Consumer<IN, B>> _) =>
            new Consumer<IN, B>.Fail(Error);

        public override Consumer<IN, M, B> Bind<M, B>(Func<A, Consumer<IN, M, B>> f) =>
            PureProxy.ConsumerLift<IN, A>(Error.Throw<A>).Bind(f);
            
        public override Pipe<IN, OUT, B> Bind<OUT, B>(Func<A, Producer<OUT, B>> _) =>
            new Pipe<IN, OUT, B>.Fail(Error);

        public override Consumer<IN, M, A> Interpret<M>() =>
            PureProxy.ConsumerLift<IN, A>(Error.Throw<A>);

        public override Pipe<IN, OUT, A> ToPipe<OUT>() =>
            new Pipe<IN, OUT, A>.Fail(Error);
    }

    public class Lift<X>(Func<X> Function, Func<X, Consumer<IN, A>> Next) : Consumer<IN, A>
    {
        public override Consumer<IN, B> Select<B>(Func<A, B> f) => 
            new Consumer<IN, B>.Lift<X>(Function, x => Next(x).Select(f));

        public override Consumer<IN, B> Bind<B>(Func<A, Consumer<IN, B>> f) => 
            new Consumer<IN, B>.Lift<X>(Function, x => Next(x).Bind(f));

        public override Consumer<IN, M, B> Bind<M, B>(Func<A, Consumer<IN, M, B>> f) =>
            Consumer.lift<IN, M, X>(M.Pure(Function())).Bind(x => Next(x).Bind(f)).ToConsumer();

        public override Pipe<IN, OUT, B> Bind<OUT, B>(Func<A, Producer<OUT, B>> f) => 
            new Pipe<IN, OUT, B>.Lift<X>(Function, x => Next(x).Bind(f));

        public override Consumer<IN, M, A> Interpret<M>() =>
            Consumer.lift<IN, M, X>(M.Pure(Function())).Bind(x => Next(x).Interpret<M>());

        public override Pipe<IN, OUT, A> ToPipe<OUT>() => 
            new Pipe<IN, OUT, A>.Lift<X>(Function, x => Next(x).ToPipe<OUT>());
    }

    public class Await : Consumer<IN, A>
    {
        public readonly Func<IN, Consumer<IN, A>> Next;
        public Await(Func<IN, Consumer<IN, A>> next) =>
            Next = next;

        public override Consumer<IN, B> Select<B>(Func<A, B> f) =>
            new Consumer<IN, B>.Await(x => Next(x).Select(f));

        public override Consumer<IN, B> Bind<B>(Func<A, Consumer<IN, B>> f) =>
            new Consumer<IN, B>.Await(x => Next(x).Bind(f));

        public override Consumer<IN, M, B> Bind<M, B>(Func<A, Consumer<IN, M, B>> f) =>
            Interpret<M>().Bind(f).ToConsumer();

        public override Pipe<IN, OUT, B> Bind<OUT, B>(Func<A, Producer<OUT, B>> f) =>
            new Pipe<IN, OUT, B>.Await(x => Next(x).Bind(f));

        public override Consumer<IN, M, A> Interpret<M>() =>
            Consumer.awaiting<M, IN>().Bind(x => Next(x).Interpret<M>()).ToConsumer();

        public override Pipe<IN, OUT, A> ToPipe<OUT>() =>
            new Pipe<IN, OUT, A>.Await(x => Next(x).ToPipe<OUT>());
    }
}
