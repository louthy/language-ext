using System;
using System.Threading;
using static LanguageExt.Prelude;
using static LanguageExt.Transducer;

namespace LanguageExt.HKT;

public interface MonadReaderT<MRdr, Env, M> 
    : Functor<MRdr, Env, M> 
    where M : Monad<M>
    where MRdr : MonadReaderT<MRdr, Env, M>
{
    public static abstract KArrow<MRdr, Env, M, A> Lift<A>(KStar<M, A> ma);

    public static abstract KArrow<MRdr, Env, M, A> Lift<A>(Transducer<Env, A> ma);

    public static abstract Transducer<Env1, KStar<M, A>> With<Env1, A>(
        Transducer<Env1, Env> f, 
        KArrow<MRdr, Env, M, A> ma);

    public static abstract KArrow<MRdr, Env, M, B> Bind<A, B>(
        KArrow<MRdr, Env, M, A> ma, 
        Transducer<A, KArrow<MRdr, Env, M, B>> f);

    public static abstract TResult<S> Run<S, A>(
        KArrow<MRdr, Env, M, A> ma, 
        Env env,
        S initialState,
        Reducer<KStar<M, A>, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null);
    
    public static virtual KArrow<MRdr, Env, M, A> Pure<A>(A value) =>
        MRdr.Lift(M.Pure(value));

    public static virtual KArrow<MRdr, Env, M, Env> Ask =>
        MRdr.Lift(identity<Env>());

    public static virtual KArrow<MRdr, Env, M, A> Local<A>(
        Transducer<Env, Env> f,
        KArrow<MRdr, Env, M, A> ma) =>
        MRdr.With<MRdr, Env, A>(f, ma);

    public static virtual KArrow<MRdr, Env, M, A> Local<A>(
        Func<Env, Env> f,
        KArrow<MRdr, Env, M, A> ma) =>
        MRdr.Local(lift(f), ma);
    
    public static virtual KArrow<MRdr, Env, M, B> Bind<A, B>(
        KArrow<MRdr, Env, M, A> ma, 
        Func<A, KArrow<MRdr, Env, M, B>> f) =>
        MRdr.Bind(ma, lift(f));

    public static virtual KArrow<MRdr, Env, M, A> Flatten<A>(
        KArrow<MRdr, Env, M, KArrow<MRdr, Env, M, A>> mma) =>
        MRdr.Bind(mma, identity);
}
