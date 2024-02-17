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

public abstract class Pipe<IN, OUT, A>
{
    public abstract Pipe<IN, OUT, B> Select<B>(Func<A, B> f);
    public abstract Pipe<IN, OUT, M, A> Interpret<M>() where M : Monad<M>;
    
    public abstract Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f);
    public abstract Pipe<RT, IN, OUT, B> Bind<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) where M : Monad<M>;
    public abstract Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f);
    public abstract Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f);
    
    public Pipe<IN, OUT, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);
  
    public Pipe<IN, OUT, B> Bind<B>(Func<A, Fail<Error>> f) =>
        Bind(x => new Pipe<IN, OUT, B>.Fail(f(x).Value));

    public Pipe<IN, OUT, B> Bind<B>(Func<A, Transducer<Unit, B>> f) =>
        Bind(a => f(a).Map(Sum<Error, B>.Right));
 
    public Pipe<IN, OUT, B> Bind<B>(Func<A, Transducer<Unit, Sum<Error, B>>> f) =>
        Bind(a => new Pipe<IN, OUT, B>.Lift<B>(f(a), PureProxy.PipePure<IN, OUT, B>));
 
    public Pipe<RT, IN, OUT, B> Bind<RT, B>(Func<A, Transducer<RT, B>> f) 
        where M : Monad<M> =>
        Interpret<RT>().Bind(f);

    public Pipe<RT, IN, OUT, B> Bind<RT, B>(Func<A, Transducer<RT, Sum<Error, B>>> f) 
        where M : Monad<M> =>
        Interpret<RT>().Bind(f);
    
    public Pipe<IN, OUT, B> Map<B>(Func<A, B> f) => 
        Select(f);

    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> project) =>
        Map(a => project(a, f(a).Value));
                
    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Fail<Error>> f, Func<A, B, C> project) =>
        Bind<C>(f);

    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Transducer<Unit, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Transducer<Unit, Sum<Error, B>>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(mb => mb.Map(b => project(a, b))));
        
    public Pipe<RT, IN, OUT, C> SelectMany<RT, B, C>(Func<A, Transducer<RT, B>> f, Func<A, B, C> project)
        where M : Monad<M> =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Pipe<RT, IN, OUT, C> SelectMany<RT, B, C>(Func<A, Transducer<RT, Sum<Error, B>>> f, Func<A, B, C> project)
        where M : Monad<M> =>
        Bind(a => f(a).Select(mb => mb.Map(b => project(a, b))));
        
    public Pipe<RT, IN, OUT, C> SelectMany<RT, B, C>(Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where M : Monad<M> =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));

    public static implicit operator Pipe<IN, OUT, A>(Pure<A> ma) =>
        new Pure(ma.Value);
        
    public static Pipe<IN, OUT, A> operator &(
        Pipe<IN, OUT, A> lhs,
        Pipe<IN, OUT, A> rhs) =>
        lhs.Bind(_ => rhs);

    public class Pure(A Value) : Pipe<IN, OUT, A>
    {
        public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) =>
            new Pipe<IN, OUT, B>.Pure(f(Value));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) =>
            f(Value);

        public override Pipe<RT, IN, OUT, B> Bind<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
            f(Value);

        public override Pipe<RT, IN, OUT, A> Interpret<RT>() =>
            Pipe.Pure<RT, IN, OUT, A>(Value);

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f) =>
            f(Value).ToPipe<OUT>();

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            f(Value).ToPipe<IN>();
    }

    public class Fail(Error Error) : Pipe<IN, OUT, A>
    {
        public override Pipe<IN, OUT, B> Select<B>(Func<A, B> _) =>
            new Pipe<IN, OUT, B>.Fail(Error);

        public override Pipe<RT, IN, OUT, A> Interpret<RT>() => 
            Pipe.lift<RT, IN, OUT, A>(Transducer.constant<RT, Sum<Error, A>>(Sum<Error, A>.Left(Error)));
        
        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) => 
            new Pipe<IN, OUT, B>.Fail(Error);

        public override Pipe<RT, IN, OUT, B> Bind<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) => 
            Pipe.lift<RT, IN, OUT, B>(Transducer.constant<RT, Sum<Error, B>>(Sum<Error, B>.Left(Error)));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> _) =>
            new Pipe<IN, OUT, B>.Fail(Error);

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) => 
            new Pipe<IN, OUT, B>.Fail(Error);
    }

    public class Lift<X>(Transducer<Unit, Sum<Error, X>> Morphism, Func<X, Pipe<IN, OUT, A>> Next) : Pipe<IN, OUT, A>
    {
        public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) => 
            new Pipe<IN, OUT, B>.Lift<X>(Morphism, x => Next(x).Select(f));

        public override Pipe<RT, IN, OUT, A> Interpret<RT>() => 
            Pipe.lift<RT, IN, OUT, X>(Morphism).Bind(x => Next(x).Interpret<RT>());

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) => 
            new Pipe<IN, OUT, B>.Lift<X>(Morphism, x => Next(x).Bind(f));

        public override Pipe<RT, IN, OUT, B> Bind<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) => 
            Pipe.lift<RT, IN, OUT, X>(Morphism).SelectMany(x => Next(x).Bind(f)).ToPipe();

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f) =>
            new Pipe<IN, OUT, B>.Lift<X>(Morphism, x => Next(x).Bind(y => f(y).ToPipe<OUT>()));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            new Pipe<IN, OUT, B>.Lift<X>(Morphism, x => Next(x).Bind(y => f(y).ToPipe<IN>()));
    }

    public class Await(Func<IN, Pipe<IN, OUT, A>> Next) : Pipe<IN, OUT, A>
    {
        public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) =>
            new Pipe<IN, OUT, B>.Await(x => Next(x).Select(f));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) =>
            new Pipe<IN, OUT, B>.Await(x => Next(x).Bind(f));

        public override Pipe<RT, IN, OUT, B> Bind<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
            from x in Interpret<RT>()
            from r in f(x)
            select r;

        public override Pipe<RT, IN, OUT, A> Interpret<RT>() =>
            from x in Pipe.awaiting<RT, IN, OUT>()
            from r in Next(x) 
            select r;

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f) =>
            new Pipe<IN, OUT, B>.Await(x => Next(x).Bind(f));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            new Pipe<IN, OUT, B>.Await(x => Next(x).Bind(f));
    }

    public class Yield(OUT Value, Func<Unit, Pipe<IN, OUT, A>> Next) : Pipe<IN, OUT, A>
    {
        public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) =>
            new Pipe<IN, OUT, B>.Yield(Value, x => Next(x).Select(f));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) =>
            new Pipe<IN, OUT, B>.Yield(Value, x => Next(x).Bind(f));

        public override Pipe<RT, IN, OUT, B> Bind<RT, B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
            from x in Interpret<RT>()
            from r in f(x)
            select r;

        public override Pipe<RT, IN, OUT, A> Interpret<RT>() =>
            from x in Pipe.yield<RT, IN, OUT>(Value)
            from r in Next(x) 
            select r;

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f) =>
            new Pipe<IN, OUT, B>.Yield(Value, x => Next(x).Bind(f));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            new Pipe<IN, OUT, B>.Yield(Value, x => Next(x).Bind(f));
    }
}
