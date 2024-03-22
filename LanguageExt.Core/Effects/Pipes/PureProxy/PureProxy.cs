//
// This file contains a number of micro-free-monads that allow for creation of pure producers, consumers, and pipes.  
// They're used to facilitate the building of Proxy derived types without the need for typing the generic arguments endlessly
// The Haskell original could auto-infer the generic parameter types, the system here tries to replicate manually what
// Haskell can do automatically.  Hence why there are so many implementations of SelectMany!
//

using System;
using LanguageExt.Effects.Traits;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static class PureProxy
{
    public static Pipe<IN, OUT, A> PipePure<IN, OUT, A>(A value) =>
        new Pipe<IN, OUT, A>.Pure(value);

    public static Producer<OUT, A> ProducerPure<OUT, A>(A value) =>
        new Producer<OUT, A>.Pure(value);

    public static Consumer<IN, A> ConsumerPure<IN, A>(A value) =>
        new Consumer<IN, A>.Pure(value);


    public static Pipe<IN, OUT, A> PipeFail<IN, OUT, A>(Error value) =>
        new Pipe<IN, OUT, A>.Fail(value);

    public static Producer<OUT, A> ProducerFail<OUT, A>(Error value) =>
        new Producer<OUT, A>.Fail(value);

    public static Consumer<IN, A> ConsumerFail<IN, A>(Error value) =>
        new Consumer<IN, A>.Fail(value);


    public static Pipe<IN, OUT, A> PipeLift<IN, OUT, A>(Func<A> t) =>
        new Pipe<IN, OUT, A>.Lift<A>(t, PipePure<IN, OUT, A>);

    public static Producer<OUT, A> ProducerLift<OUT, A>(Func<A> t) =>
        new Producer<OUT, A>.Lift<A>(t, ProducerPure<OUT, A>);

    public static Consumer<IN, A> ConsumerLift<IN, A>(Func<A> t) =>
        new Consumer<IN, A>.Lift<A>(t, ConsumerPure<IN, A>);


    public static Pipe<IN, OUT, A> PipeLiftIO<IN, OUT, A>(IO<A> ma) =>
        new Pipe<IN, OUT, A>.LiftIO<A>(ma, PipePure<IN, OUT, A>);

    public static Producer<OUT, A> ProducerLiftIO<OUT, A>(IO<A> ma) =>
        new Producer<OUT, A>.LiftIO<A>(ma, ProducerPure<OUT, A>);

    public static Consumer<IN, A> ConsumerLiftIO<IN, A>(IO<A> ma) =>
        new Consumer<IN, A>.LiftIO<A>(ma, ConsumerPure<IN, A>);


    public static Pipe<IN, OUT, Unit> PipeFold<IN, F, OUT>(K<F, OUT> items)
        where F : Foldable<F> =>
        new Pipe<IN, OUT, Unit>.Fold<F, OUT>(
            items, 
            PipeYield<IN, OUT>, 
            () => PipePure<IN, OUT, Unit>(Prelude.unit));

    public static Producer<OUT, Unit> ProducerFold<F, OUT>(K<F, OUT> items)
        where F : Foldable<F> =>
        new Producer<OUT, Unit>.Fold<F, OUT>(
            items, 
            ProducerYield, 
            () => ProducerPure<OUT, Unit>(Prelude.unit));

    public static Consumer<IN, Unit> ConsumerFold<IN, F, A>(K<F, A> items)
        where F : Foldable<F> =>
        new Consumer<IN, Unit>.Fold<F, A>(
            items, 
            _ => ConsumerPure<IN, Unit>(Prelude.unit), 
            () => ConsumerPure<IN, Unit>(Prelude.unit));

    
    public static Consumer<IN, IN> ConsumerAwait<IN>() =>
        new Consumer<IN, IN>.Await(ConsumerPure<IN, IN>);

    public static Pipe<IN, OUT, IN> PipeAwait<IN, OUT>() =>
        new Pipe<IN, OUT, IN>.Await(PipePure<IN, OUT, IN>);

    
    public static Producer<OUT, Unit> ProducerYield<OUT>(OUT value) =>
        new Producer<OUT, Unit>.Yield(value, ProducerPure<OUT, Unit>);

    public static Pipe<IN, OUT, Unit> PipeYield<IN, OUT>(OUT value) =>
        new Pipe<IN, OUT, Unit>.Yield(value, PipePure<IN, OUT, Unit>);

    // HERE
    public static Consumer<IN, M, C> SelectMany<IN, M, A, B, C>(this K<M, A> ma, Func<A, Consumer<IN, B>> f, Func<A, B, C> project) where M : Monad<M> =>
        from a in Consumer.lift<IN, M, A>(ma)
        from b in f(a).Interpret<M>()
        select project(a, b);

    public static Producer<OUT, M, C> SelectMany<OUT, M, A, B, C>(this K<M, A> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) where M : Monad<M> =>
        from a in Producer.lift<OUT, M, A>(ma)
        from b in f(a).Interpret<M>()
        select project(a, b);

    public static Pipe<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(this K<M, A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) where M : Monad<M> =>
        from a in Pipe.lift<IN, OUT, M, A>(ma)
        from b in f(a).Interpret<M>()
        select project(a, b);

    public static Consumer<IN, M, C> SelectMany<IN, M, A, B, C>(this K<M, A> ma, Func<A, Consumer<IN, M, B>> f, Func<A, B, C> project) where M : Monad<M> =>
        from a in Consumer.lift<IN, M, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Producer<OUT, M, C> SelectMany<OUT, M, A, B, C>(this K<M, A> ma, Func<A, Producer<OUT, M, B>> f, Func<A, B, C> project) where M : Monad<M> =>
        from a in Producer.lift<OUT, M, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Pipe<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(this K<M, A> ma, Func<A, Pipe<IN, OUT, M, B>> f, Func<A, B, C> project) where M : Monad<M> =>
        from a in Pipe.lift<IN, OUT, M, A>(ma)
        from b in f(a)
        select project(a, b);
}
