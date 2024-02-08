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

public class MReaderT<Env, M> : MonadReaderT<MReaderT<Env, M>, Env, M>
    where M : Monad<M>
{
    public static ReaderT<Env, M, Env> Ask =>
        MonadReaderT.ask<MReaderT<Env, M>, Env, M>().As();

    public static KArrow<MReaderT<Env, M>, Env, M, A> Lift<A>(KStar<M, A> ma) => 
        ReaderT<Env, M, A>.Lift(ma);

    public static KArrow<MReaderT<Env, M>, Env, M, A> Lift<A>(Transducer<Env, A> ma) => 
        ReaderT<Env, M, A>.Lift(ma);

    public static TResult<S> Run<S, A>(
        KArrow<MReaderT<Env, M>, Env, M, A> ma,
        Env env,
        S initialState,
        Reducer<KStar<M, A>, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        ma.As().Run(env, initialState, reducer, token, syncContext);

    public static Transducer<Env1, KStar<M, A>> With<Env1, A>(
        Transducer<Env1, Env> f, 
        KArrow<MReaderT<Env, M>, Env, M, A> ma) => 
        ma.As().With(f).Morphism;

    public static KArrow<MReaderT<Env, M>, Env, M, B> Bind<A, B>(
        KArrow<MReaderT<Env, M>, Env, M, A> ma, 
        Transducer<A, KArrow<MReaderT<Env, M>, Env, M, B>> f) =>
        ma.As().Bind(f);
}

public record ReaderT<Env, M, A>(Transducer<Env, KStar<M, A>> runReaderT) 
    : KArrow<MReaderT<Env, M>, Env, M, A> 
    where M : Monad<M>
{
    public static ReaderT<Env, M, Env> Ask =>
        new (lift<Env, KStar<M, Env>>(e => M.Pure(e)));
    
    public ReaderT<Env1, M, A> With<Env1>(Transducer<Env1, Env> f) =>
         new(Transducer.compose(f, runReaderT));
    
    public ReaderT<Env, M, A> Local(Func<Env, Env> f) =>
        With(lift(f));
    
    public static ReaderT<Env, M, A> Pure(A value) =>
        Lift(M.Pure(value));
    
    public static ReaderT<Env, M, A> Lift(KStar<M, A> monad) => 
        new (lift<Env, KStar<M, A>>(_ => monad));

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
         new(lift<Env, Transducer<Env, KStar<M, B>>>(
                     env =>
                         runReaderT.Map(ma => M.Map(ma, f.Map(mb => mb.Morphism.Invoke(env)).Flatten()))
                                   .Map(M.Flatten))
                .Flatten());
    
    public ReaderT<Env, M, B> Bind<B>(Transducer<A, KArrow<MReaderT<Env, M>, Env, M, B>> f) =>
        new(lift<Env, Transducer<Env, KStar<M, B>>>(
                    env =>
                        runReaderT.Map(ma => M.Map(ma, f.Map(mb => mb.Morphism.Invoke(env)).Flatten()))
                                  .Map(M.Flatten))
               .Flatten());
        
    
    public ReaderT<Env, M, B> Bind<B>(Func<A, ReaderT<Env, M, B>> f) =>
        Bind(lift(f));

    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, KStar<M, B>> bind, Func<A, B, C> project) =>
        Bind(x => ReaderT<Env, M, B>.Lift(bind(x)).Map(y => project(x, y)));
    
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
}
