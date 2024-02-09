using System;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.HKT;
using static LanguageExt.Prelude;

namespace LanguageExt;

//public delegate Fin<A> Reader<in Env, A>(Env env);

public readonly record struct Ask<Env, A>(Func<Env, A> F)
{
    public ReaderT<Env, M, A> ToReaderT<M>() where M : Monad<M> =>
        ReaderT<Env, M, A>.Asks(F);
    
    public Reader<Env, A> ToReader() =>
        Reader<Env, A>.Asks(F).As();

    public ReaderT<Env, M, C> SelectMany<M, B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> =>
        ToReaderT<M>().SelectMany(bind, project);

    public Reader<Env, C> SelectMany<B, C>(Func<A, Reader<Env, B>> bind, Func<A, B, C> project) =>
        ToReader().SelectMany(bind, project).As();
}

public static partial class ReaderT
{
    public static ReaderT<Env, M, A> As<Env, M, A>(this MonadT<MReaderT<Env, M>, M, A> ma)
        where M : Monad<M> =>
        (ReaderT<Env, M, A>)ma;
    
    public static ReaderT<Env, M, A> As<Env, M, A>(this FunctorT<MReaderT<Env, M>, M, A> ma)
        where M : Monad<M> =>
        (ReaderT<Env, M, A>)ma;
}

public class MReaderT<Env, M> : MonadReaderT<MReaderT<Env, M>, Env, M>
    where M : Monad<M>
{
    public static MonadT<MReaderT<Env, M>, M, A> Pure<A>(A value) =>
        ReaderT<Env, M, A>.Pure(value);

    public static MonadT<MReaderT<Env, M>, M, A> Lift<A>(Monad<M, A> ma) => 
        ReaderT<Env, M, A>.Lift(ma);

    public static MonadT<MReaderT<Env, M>, M, A> Asks<A>(Transducer<Env, A> ma) => 
        ReaderT<Env, M, A>.Lift(ma);

    public static FunctorT<MReaderT<Env, M>, M, B> Map<A, B>(
        FunctorT<MReaderT<Env, M>, M, A> mma,
        Transducer<A, B> f) =>
        new ReaderT<Env, M, B>(mma.As().runReaderT.Map(ma => M.Map(ma, f).AsMonad()));

    public static MonadT<MReaderT<Env, M>, M, B> Bind<A, B>(
        MonadT<MReaderT<Env, M>, M, A> mma,
        Transducer<A, MonadT<MReaderT<Env, M>, M, B>> f) =>
        new ReaderT<Env, M, B>(
            lift<Env, Transducer<Env, Monad<M, B>>>(
                env =>
                    mma.As().runReaderT
                       .Map(ma =>
                                M.Map(ma,
                                      f.Map(mb => mb.As()
                                                    .runReaderT
                                                    .Invoke(env)).Flatten()).AsMonad()).Map(M.Flatten)).Flatten());

    public static MonadT<MRdr1, M, A> With<MRdr1, Env1, A>(
        Transducer<Env1, Env> f, 
        MonadT<MReaderT<Env, M>, M, A> ma) 
        where MRdr1 : MonadReaderT<MRdr1, Env1, M> =>
        ma.As().With(f);
}

public record ReaderT<Env, M, A>(Transducer<Env, Monad<M, A>> runReaderT) :
    MonadT<MReaderT<Env, M>, M, A> 
    where M : Monad<M>
{
    public static ReaderT<Env, M, A> Pure(A value) =>
        Lift(M.Pure(value));

    public static ReaderT<Env, M, A> Asks(Func<Env, A> f) =>
        Asks(lift(f));
    
    public static ReaderT<Env, M, A> Asks(Transducer<Env, A> f) =>
        new (f.Map(M.Pure));
    
    public static ReaderT<Env, M, A> Lift(Monad<M, A> monad) => 
        new (lift<Env, Monad<M, A>>(_ => monad));

    public static ReaderT<Env, M, A> Lift(Transducer<Unit, A> t) =>
        new(Transducer.compose(Transducer.constant<Env, Unit>(default), t.Map(M.Pure)));
    
    public static ReaderT<Env, M, A> Lift(Func<Unit, A> t) =>
        Lift(lift(t));
    
    public static ReaderT<Env, M, A> Lift(Transducer<Env, A> t) =>
        new (t.Map(M.Pure));
    
    public static ReaderT<Env, M, A> Lift(Func<Env, A> f) =>
        Lift(lift(f));

    public ReaderT<Env1, M, A> With<Env1>(Transducer<Env1, Env> f) =>
        new(Transducer.compose(f, runReaderT));
    
    public ReaderT<Env, M, A> Local(Func<Env, Env> f) =>
        With(lift(f));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    public ReaderT<Env, M, B> Map<B>(Transducer<A, B> f) =>
        Functor.map(this, f).As();

    public ReaderT<Env, M, B> Map<B>(Func<A, B> f) =>
        Functor.map(this, f).As();
    
    public ReaderT<Env, M, B> Select<B>(Func<A, B> f) =>
        Map(f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //
    
    public ReaderT<Env, M, B> Bind<B>(Transducer<A, MonadT<MReaderT<Env, M>, M, B>> f) =>
        MonadReaderT.bind<MReaderT<Env, M>, Env, M, A, B>(this, f).As();

    public ReaderT<Env, M, B> Bind<B>(Func<A, MonadT<MReaderT<Env, M>, M, B>> f) =>
        MonadReaderT.bind<MReaderT<Env, M>, Env, M, A, B>(this, f).As();
    
    public ReaderT<Env, M, B> Bind<B>(Transducer<A, ReaderT<Env, M, B>> f) =>
        Bind(f.Map(x => x.AsMonad()));

    public ReaderT<Env, M, B> Bind<B>(Func<A, ReaderT<Env, M, B>> f) =>
        Bind(lift(f));

    public ReaderT<Env, M, B> Bind<B>(Transducer<A, Ask<Env, B>> f) =>
        Bind(f.Map(ask => ask.ToReaderT<M>()));
    
    public ReaderT<Env, M, B> Bind<B>(Func<A, Ask<Env, B>> f) =>
        Bind(lift(f));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany
    //
    
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, MonadT<MReaderT<Env, M>, M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).As().Map(y => project(x, y)));

    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Monad<M, B>> bind, Func<A, B, C> project) =>
        Bind(x => ReaderT<Env, M, B>.Lift(bind(x)).Map(y => project(x, y)));

    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));
    
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Ask<Env, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).ToReaderT<M>().Map(y => project(x, y)));

    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Transducer<Env, B>> bind, Func<A, B, C> project) =>
        Bind(x => ReaderT<Env, M, B>.Lift(bind(x)).Map(y => project(x, y)));

    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Transducer<Unit, B>> bind, Func<A, B, C> project) =>
        Bind(x => ReaderT<Env, M, B>.Lift(bind(x)).Map(y => project(x, y)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //

    public static implicit operator ReaderT<Env, M, A>(Transducer<Unit, A> t) =>
        new(Transducer.compose(Transducer.constant<Env, Unit>(default), t.Map(M.Pure)));
    
    public static implicit operator ReaderT<Env, M, A>(Transducer<Env, A> t) =>
        new (t.Map(M.Pure));
    
    public static implicit operator ReaderT<Env, M, A>(Transducer<Env, Monad<M, A>> runReaderT) =>
        new (runReaderT);
    
    public static implicit operator ReaderT<Env, M, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    public static implicit operator ReaderT<Env, M, A>(Fail<Error> ma) =>
        Lift(Transducer.fail<Env, A>(ma.Value));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the reader
    //

    public TResult<S> Run<S>(
        Env env,
        S initialState,
        Reducer<Monad<M, A>, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        runReaderT.Morphism.Run(env, initialState, reducer, token, syncContext);

    public Fin<Monad<M, A>> Run(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        Run(env, default, Reducer<Monad<M, A>>.last, token, syncContext)
           .Bind(ma => ma is null ? TResult.None<Monad<M, A>>() : TResult.Continue(ma))
           .ToFin(); 

    public Fin<Seq<Monad<M, A>>> RunMany(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        Run(env, default, Reducer<Monad<M, A>>.seq, token, syncContext).ToFin(); 
}
