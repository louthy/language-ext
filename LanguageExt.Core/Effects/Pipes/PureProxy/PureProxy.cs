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


    public static Pipe<IN, OUT, A> PipeLift<IN, OUT, A>(Transducer<Unit, Sum<Error, A>> t) =>
        new Pipe<IN, OUT, A>.Lift<A>(t, PipePure<IN, OUT, A>);

    public static Pipe<IN, OUT, A> PipeLift<IN, OUT, A>(Transducer<Unit, A> t) =>
        PipeLift<IN, OUT, A>(t.Map(Sum<Error, A>.Right));

    public static Producer<OUT, A> ProducerLift<OUT, A>(Transducer<Unit, Sum<Error, A>> t) =>
        new Producer<OUT, A>.Lift<A>(t, ProducerPure<OUT, A>);

    public static Producer<OUT, A> ProducerLift<OUT, A>(Transducer<Unit, A> t) =>
        ProducerLift<OUT, A>(t.Map(Sum<Error, A>.Right));

    public static Consumer<IN, A> ConsumerLift<IN, A>(Transducer<Unit, Sum<Error, A>> t) =>
        new Consumer<IN, A>.Lift<A>(t, ConsumerPure<IN, A>);

    public static Consumer<IN, A> ConsumerLift<IN, A>(Transducer<Unit, A> t) =>
        ConsumerLift<IN, A>(t.Map(Sum<Error, A>.Right));

    
    public static Consumer<IN, IN> ConsumerAwait<IN>() =>
        new Consumer<IN, IN>.Await(ConsumerPure<IN, IN>);

    public static Pipe<IN, OUT, IN> PipeAwait<IN, OUT>() =>
        new Pipe<IN, OUT, IN>.Await(PipePure<IN, OUT, IN>);

    
    public static Producer<OUT, Unit> ProducerYield<OUT>(OUT value) =>
        new Producer<OUT, Unit>.Yield(value, ProducerPure<OUT, Unit>);

    public static Pipe<IN, OUT, Unit> PipeYield<IN, OUT>(OUT value) =>
        new Pipe<IN, OUT, Unit>.Yield(value, PipePure<IN, OUT, Unit>);

    // HERE
    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Transducer<RT, Sum<Error, A>> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Producer.lift<RT, OUT, A>(ma)
        from b in f(a).Interpret<RT>()
        select project(a, b);

    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Transducer<RT, Sum<Error, A>> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Pipe.lift<RT, IN, OUT, A>(ma)
        from b in f(a).Interpret<RT>()
        select project(a, b);

    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Transducer<RT, Sum<Error, A>> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Consumer.lift<RT, IN, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Transducer<RT, Sum<Error, A>> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Producer.lift<RT, OUT, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Transducer<RT, Sum<Error, A>> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Pipe.lift<RT, IN, OUT, A>(ma)
        from b in f(a)
        select project(a, b);
    
    // HERE
    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Transducer<RT, A> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Producer.lift<RT, OUT, A>(ma)
        from b in f(a).Interpret<RT>()
        select project(a, b);

    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Transducer<RT, A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Pipe.lift<RT, IN, OUT, A>(ma)
        from b in f(a).Interpret<RT>()
        select project(a, b);

    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Transducer<RT, A> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Consumer.lift<RT, IN, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Transducer<RT, A> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Producer.lift<RT, OUT, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Transducer<RT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Pipe.lift<RT, IN, OUT, A>(ma)
        from b in f(a)
        select project(a, b);
    

    
    
    public static Consumer<IN, C> SelectMany<IN, A, B, C>(this Transducer<Unit, A> ma, Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
        from a in ConsumerLift<IN, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Producer<OUT, C> SelectMany<OUT, A, B, C>(this Transducer<Unit, A> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
        from a in ProducerLift<OUT, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Pipe<IN, OUT, C> SelectMany<IN, OUT, A, B, C>(this Transducer<Unit, A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) =>
        from a in PipeLift<IN, OUT, A>(ma)
        from b in f(a)
        select project(a, b);

    
    
    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Transducer<Unit, A> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Consumer.lift<RT, IN, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Transducer<Unit, A> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Producer.lift<RT, OUT, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Transducer<Unit, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Pipe.lift<RT, IN, OUT, A>(ma)
        from b in f(a)
        select project(a, b);


    public static Consumer<IN, C> SelectMany<IN, A, B, C>(this Transducer<Unit, Sum<Error, A>> ma, Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
        from a in ConsumerLift<IN, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Producer<OUT, C> SelectMany<OUT, A, B, C>(this Transducer<Unit, Sum<Error, A>> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
        from a in ProducerLift<OUT, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Pipe<IN, OUT, C> SelectMany<IN, OUT, A, B, C>(this Transducer<Unit, Sum<Error, A>> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) =>
        from a in PipeLift<IN, OUT, A>(ma)
        from b in f(a)
        select project(a, b);
    
    
    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Transducer<Unit, Sum<Error, A>> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Consumer.lift<RT, IN, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Transducer<Unit, Sum<Error, A>> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Producer.lift<RT, OUT, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Transducer<Unit, Sum<Error, A>> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Pipe.lift<RT, IN, OUT, A>(ma)
        from b in f(a)
        select project(a, b);


    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Eff<RT, A> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Consumer.lift<RT, IN, A>(ma)
        from b in f(a)
        select project(a, b);
    
    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Eff<RT, A> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Producer.lift<RT, OUT, A>(ma)
        from b in f(a).Interpret<RT>()
        select project(a, b);

    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Eff<RT, A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Pipe.lift<RT, IN, OUT, A>(ma)
        from b in f(a).Interpret<RT>()
        select project(a, b);

    
    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Eff<RT, A> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Producer.lift<RT, OUT, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Eff<RT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Pipe.lift<RT, IN, OUT, A>(ma)
        from b in f(a)
        select project(a, b);

    
        
    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Eff<A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Pipe.lift<RT, IN, OUT, A>(ma)
        from b in f(a).Interpret<RT>()
        select project(a, b);

    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Eff<A> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Consumer.lift<RT, IN, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Eff<A> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Producer.lift<RT, OUT, A>(ma)
        from b in f(a)
        select project(a, b);

    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Eff<A> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : HasIO<RT, Error> =>
        from a in Pipe.lift<RT, IN, OUT, A>(ma)
        from b in f(a)
        select project(a, b);
}
