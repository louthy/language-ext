using System;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.HKT;
using static LanguageExt.Prelude;

namespace LanguageExt;

//public delegate Fin<A> Reader<in Env, A>(Env env);

public static partial class ReaderT
{
    public static ReaderT<Env, M, A> As<Env, M, A>(this KArrow<MReaderT<Env, M>, Env, M, A> ma)
        where M : Monad<M> =>
        (ReaderT<Env, M, A>)ma;
}

public class MReaderT<Env, M> : 
    MonadReaderT<MReaderT<Env, M>, Env, M>
    where M : Monad<M>
{
    public static KArrow<MReaderT<Env, M>, Env, M, A> Lift<A>(Transducer<Env, KStar<M, A>> f) => 
        new ReaderT<Env, M, A>(f);

    public static ReaderT<Env, M, A> Lift<A>(KStar<M, A> ma) =>
        new(Transducer.constant<Env, KStar<M, A>>(ma));
    
    public static ReaderT<Env, M, Env> Ask =>
        MonadReaderT.ask<MReaderT<Env, M>, Env, M>().As();
}

public record ReaderT<Env, M, A>(Transducer<Env, KStar<M, A>> runReaderT) 
    : KArrow<MReaderT<Env, M>, Env, M, A> 
    where M : Monad<M>
{
    public static ReaderT<Env, M, Env> Ask =>
        MonadReaderT.ask<MReaderT<Env, M>, Env, M>().As();
    
    public ReaderT<Env, M, A> Local(Func<Env, Env> f) =>
        MonadReaderT.local(f, this).As();
    
    public static ReaderT<Env, M, A> Pure(A value) =>
        MonadReaderT.pure<MReaderT<Env, M>, Env, M, A>(value).As();
    
    public static ReaderT<Env, M, A> Lift(KStar<M, A> monad) => 
        MonadReaderT.lift<MReaderT<Env, M>, Env, M, A>(monad).As();

    public static ReaderT<Env, M, A> Lift(Transducer<Env, A> f) =>
        new (f.Map(M.Pure));

    public static ReaderT<Env, M, A> Lift(Func<Env, A> f) =>
        Lift(lift(f));
    
    public ReaderT<Env, M, B> Map<B>(Transducer<A, B> f) =>
        MonadReaderT.map(this, f).As();

    public ReaderT<Env, M, B> Map<B>(Func<A, B> f) =>
        MonadReaderT.map(this, f).As();

    public ReaderT<Env, M, B> Select<B>(Func<A, B> f) =>
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
    
    public static implicit operator ReaderT<Env, M, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    public static implicit operator ReaderT<Env, M, A>(Fail<Error> ma) =>
        Lift(Transducer.fail<Env, A>(ma.Value));

    public Transducer<Env, KStar<M, A>> Morphism =>
        runReaderT;
    
    public TResult<S> Run<S>(
        Env env,
        S initialState,
        Reducer<KStar<M, A>, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        runReaderT.Morphism.Run(env, initialState, reducer, token, syncContext);

    public Fin<KStar<M, A>> Run(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        Run(env, default, Reducer<KStar<M, A>>.last, token, syncContext)
           .Bind(ma => ma is null ? TResult.None<KStar<M, A>>() : TResult.Continue(ma))
           .ToFin(); 

    public Fin<Seq<KStar<M, A>>> RunMany(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        Run(env, default, Reducer<KStar<M, A>>.seq, token, syncContext).ToFin(); 
    
    public TResult<S> RunT<S>(
        Env env,
        S initialState,
        Reducer<A, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        M.Run(runReaderT.Invoke(env), initialState, reducer, token, syncContext); 
    
    public Fin<A> RunT(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        RunT(env, default, Reducer<A>.last, token, syncContext)
           .Bind(ma => ma is null ? TResult.None<A>() : TResult.Continue(ma))
           .ToFin(); 
    
    public Fin<Seq<A>> RunManyT(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        RunT(env, default, Reducer<A>.seq, token, syncContext).ToFin();     
}
