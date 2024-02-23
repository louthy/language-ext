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

public abstract class Pipe<IN, OUT, A>
{
    public abstract Pipe<IN, OUT, B> Select<B>(Func<A, B> f);
    public abstract Pipe<IN, OUT, M, A> Interpret<M>() where M : Monad<M>;
    
    public abstract Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f);
    public abstract Pipe<IN, OUT, M, B> Bind<M, B>(Func<A, Pipe<IN, OUT, M, B>> f) where M : Monad<M>;
    public abstract Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f);
    public abstract Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f);
    
    public Pipe<IN, OUT, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);
  
    public Pipe<IN, OUT, B> Bind<B>(Func<A, Fail<Error>> f) =>
        Bind(x => new Pipe<IN, OUT, B>.Fail(f(x).Value));

    public Pipe<IN, OUT, M, B> Bind<M, B>(Func<A, K<M, B>> f) 
        where M : Monad<M> =>
        Interpret<M>().Bind(f);

    public Pipe<IN, OUT, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => PureProxy.PipeLiftIO<IN, OUT, B>(f(x)));
    
    public Pipe<IN, OUT, B> Map<B>(Func<A, B> f) => 
        Select(f);

    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> project) =>
        Map(a => project(a, f(a).Value));
                
    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Fail<Error>> f, Func<A, B, C> project) =>
        Bind<C>(f);

    public Pipe<IN, OUT, M, C> SelectMany<M, B, C>(Func<A, K<M, B>> f, Func<A, B, C> project) 
        where M : Monad<M> =>
        Bind(a => M.Map(b => project(a, b), f(a)));

    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> project) =>
        Bind(x => f(x).Map(y => project(x, y)));
        
    public Pipe<IN, OUT, M, C> SelectMany<M, B, C>(Func<A, Pipe<IN, OUT, M, B>> f, Func<A, B, C> project) where M : Monad<M> =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
        
    public Pipe<IN, OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
        Bind(a => f(a).Select(b => project(a, b)));
    
    public Pipe<IN, OUT, C> SelectMany<C>(Func<A, Guard<Error, Unit>> bind, Func<A, Unit, C> project ) =>
        Map(a => bind(a) switch
                 {
                     { Flag: true } => project(a, default),
                     var g          => g.OnFalse().Throw<C>()
                 });

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

        public override Pipe<IN, OUT, M, B> Bind<M, B>(Func<A, Pipe<IN, OUT, M, B>> f) =>
            f(Value);

        public override Pipe<IN, OUT, M, A> Interpret<M>() =>
            Pipe.Pure<IN, OUT, M, A>(Value);

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f) =>
            f(Value).ToPipe<OUT>();

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            f(Value).ToPipe<IN>();
    }

    public class Fail(Error Error) : Pipe<IN, OUT, A>
    {
        public override Pipe<IN, OUT, B> Select<B>(Func<A, B> _) =>
            new Pipe<IN, OUT, B>.Fail(Error);

        public override Pipe<IN, OUT, M, A> Interpret<M>() => 
            PureProxy.PipeLift<IN, OUT, A>(Error.Throw<A>);
        
        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) => 
            new Pipe<IN, OUT, B>.Fail(Error);

        public override Pipe<IN, OUT, M, B> Bind<M, B>(Func<A, Pipe<IN, OUT, M, B>> f) => 
            PureProxy.PipeLift<IN, OUT, A>(Error.Throw<A>).Bind(f);

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> _) =>
            new Pipe<IN, OUT, B>.Fail(Error);

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) => 
            new Pipe<IN, OUT, B>.Fail(Error);
    }

    public class Lift<X>(Func<X> Function, Func<X, Pipe<IN, OUT, A>> Next) : Pipe<IN, OUT, A>
    {
        public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) => 
            new Pipe<IN, OUT, B>.Lift<X>(Function, x => Next(x).Select(f));

        public override Pipe<IN, OUT, M, A> Interpret<M>() => 
            Pipe.lift<IN, OUT, M, X>(M.Pure(Function())).Bind(x => Next(x).Interpret<M>());

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) => 
            new Pipe<IN, OUT, B>.Lift<X>(Function, x => Next(x).Bind(f));

        public override Pipe<IN, OUT, M, B> Bind<M, B>(Func<A, Pipe<IN, OUT, M, B>> f) => 
            Pipe.lift<IN, OUT, M, X>(M.Pure(Function())).SelectMany(x => Next(x).Bind(f)).ToPipe();

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f) =>
            new Pipe<IN, OUT, B>.Lift<X>(Function, x => Next(x).Bind(y => f(y).ToPipe<OUT>()));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            new Pipe<IN, OUT, B>.Lift<X>(Function, x => Next(x).Bind(y => f(y).ToPipe<IN>()));
    }

    public class LiftIO<X>(IO<X> Effect, Func<X, Pipe<IN, OUT, A>> Next) : Pipe<IN, OUT, A>
    {
        public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) => 
            new Pipe<IN, OUT, B>.LiftIO<X>(Effect, x => Next(x).Select(f));

        public override Pipe<IN, OUT, M, A> Interpret<M>() => 
            Pipe.lift<IN, OUT, M, X>(M.LiftIO(Effect)).Bind(x => Next(x).Interpret<M>());

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) => 
            new Pipe<IN, OUT, B>.LiftIO<X>(Effect, x => Next(x).Bind(f));

        public override Pipe<IN, OUT, M, B> Bind<M, B>(Func<A, Pipe<IN, OUT, M, B>> f) => 
            Pipe.lift<IN, OUT, M, X>(M.LiftIO(Effect)).SelectMany(x => Next(x).Bind(f)).ToPipe();

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f) =>
            new Pipe<IN, OUT, B>.LiftIO<X>(Effect, x => Next(x).Bind(y => f(y).ToPipe<OUT>()));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            new Pipe<IN, OUT, B>.LiftIO<X>(Effect, x => Next(x).Bind(y => f(y).ToPipe<IN>()));
    }

    public class Await(Func<IN, Pipe<IN, OUT, A>> Next) : Pipe<IN, OUT, A>
    {
        public override Pipe<IN, OUT, B> Select<B>(Func<A, B> f) =>
            new Pipe<IN, OUT, B>.Await(x => Next(x).Select(f));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Pipe<IN, OUT, B>> f) =>
            new Pipe<IN, OUT, B>.Await(x => Next(x).Bind(f));

        public override Pipe<IN, OUT, M, B> Bind<M, B>(Func<A, Pipe<IN, OUT, M, B>> f) =>
            from x in Interpret<M>()
            from r in f(x)
            select r;

        public override Pipe<IN, OUT, M, A> Interpret<M>() =>
            from x in Pipe.awaiting<M, IN, OUT>()
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

        public override Pipe<IN, OUT, M, B> Bind<M, B>(Func<A, Pipe<IN, OUT, M, B>> f) =>
            from x in Interpret<M>()
            from r in f(x)
            select r;

        public override Pipe<IN, OUT, M, A> Interpret<M>() =>
            from x in Pipe.yield<IN, OUT, M>(Value)
            from r in Next(x) 
            select r;

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Consumer<IN, B>> f) =>
            new Pipe<IN, OUT, B>.Yield(Value, x => Next(x).Bind(f));

        public override Pipe<IN, OUT, B> Bind<B>(Func<A, Producer<OUT, B>> f) =>
            new Pipe<IN, OUT, B>.Yield(Value, x => Next(x).Bind(f));
    }
}
