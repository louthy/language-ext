using System;
using LanguageExt.Common;
using LanguageExt.HKT;
using static LanguageExt.Prelude;

namespace LanguageExt;

//public delegate Fin<A> Reader<in Env, A>(Env env);

public static class Testing
{
    public static void Test()
    {
        var m1 = ReaderT<string, Maybe, int>.Lift(Maybe<int>.Just(123));
        var m2 = ReaderT<string, Maybe, int>.Lift(Maybe<int>.Just(123));
        
        var m3 = from x in m1
                 from y in m2
                 from z in Maybe<int>.Just(100)
                 from e in MReaderT<string, Maybe>.Ask
                 select $"{e}: {x + y + x}";
        
        var m4 = from x in m1
                 from y in m2
                 from z in Maybe<int>.Nothing
                 from e in MReaderT<string, Maybe>.Ask
                 select $"{e}: {x + y + x}";

    }
}

public static partial class ReaderT
{
    public static ReaderT<Env, M, A> As<Env, M, A>(this KArr<MReaderT<Env, M>, Env, M, A> ma)
        where M : Monad<M> =>
        (ReaderT<Env, M, A>)ma;
}

public class MReaderT<Env, M> : MonadReaderT<MReaderT<Env, M>, Env, M>
    where M : Monad<M>
{
    public static KArr<MReaderT<Env, M>, Env, M, A> Lift<A>(Transducer<Env, KStar<M, A>> f) => 
        new ReaderT<Env, M, A>(f);

    public static ReaderT<Env, M, A> Lift<A>(KStar<M, A> ma) =>
        new(Transducer.constant<Env, KStar<M, A>>(ma));
    
    public static ReaderT<Env, M, Env> Ask =>
        MonadReaderT.ask<MReaderT<Env, M>, Env, M>().As();
}

public record ReaderT<Env, M, A>(Transducer<Env, KStar<M, A>> runReaderT) 
    : KArr<MReaderT<Env, M>, Env, M, A> 
    where M : Monad<M>
{
    public static ReaderT<Env, M, Env> Ask =>
        MonadReaderT.ask<MReaderT<Env, M>, Env, M>().As();
    
    public static ReaderT<Env, M, A> Pure(A value) =>
        MonadReaderT.pure<MReaderT<Env, M>, Env, M, A>(value).As();
    
    public static ReaderT<Env, M, A> Lift(KStar<M, A> monad) => 
        MonadReaderT.lift<MReaderT<Env, M>, Env, M, A>(monad).As();
    
    public ReaderT<Env, M, B> Map<B>(Transducer<A, B> f) =>
        MonadReaderT.map(this, f).As();

    public ReaderT<Env, M, B> Map<B>(Func<A, B> f) =>
        MonadReaderT.map(this, f).As();

    public ReaderT<Env, M, B> Bind<B>(Transducer<A, ReaderT<Env, M, B>> f) =>
        MonadReaderT.bind<
            MReaderT<Env, M>, 
            ReaderT<Env, M, B>,
            Env, M, A, B>(this, f);

    public ReaderT<Env, M, B> Bind<B>(Func<A, ReaderT<Env, M, B>> f) =>
        MonadReaderT.bind<
            MReaderT<Env, M>, 
            ReaderT<Env, M, B>,
            Env, M, A, B>(this, f);

    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, KStar<M, B>> bind, Func<A, B, C> project) =>
        Bind(x => MReaderT<Env, M>.Lift(bind(x)).Map(y => project(x, y)));
    
    public static implicit operator ReaderT<Env, M, A>(Transducer<Env, KStar<M, A>> runReaderT) =>
        new (runReaderT);

    public Transducer<Env, KStar<M, A>> Morphism =>
        runReaderT;
}

public class Maybe : Monad<Maybe>
{
    public static KStar<Maybe, A> Lift<A>(Transducer<Unit, A> f) => 
        new Maybe<A>(f);
}

public record Maybe<A>(Transducer<Unit, A> Monad) : KStar<Maybe, A>
{
    public Transducer<Unit, A> Morphism { get; } = Monad;

    public static Maybe<A> Just(A value) =>
        new(Transducer.constant<Unit, A>(value));
    
    public static readonly Maybe<A> Nothing = 
        new(Transducer.fail<Unit, A>(Errors.None));
}


/*
public override Reducer<Env, S> Transform<S>(Reducer<A, S> reduce) => 
    new Reducer1<S>(runReaderT, reduce);

record Reducer1<S>(Transducer<Env, KStar<M, A>> runReaderT, Reducer<A, S> reduce) : Reducer<Env, S>
{
    public override TResult<S> Run(TState state, S stateValue, Env env) =>
        runReaderT.Transform(new Reducer2<S>(reduce)).Run(state, stateValue, env);
}

record Reducer2<S>(Reducer<A, S> reduce) : Reducer<KStar<M, A>, S>
{
    public override TResult<S> Run(TState state, S stateValue, KStar<M, A> monad) => 
        monad.Morphism.Transform(reduce).Run(state, stateValue, default);
}
*/
