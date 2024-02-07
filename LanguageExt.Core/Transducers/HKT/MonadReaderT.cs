using System;
using static LanguageExt.Prelude;
using static LanguageExt.Transducer;

namespace LanguageExt.HKT;

public interface MonadReaderT<MRdr, Env, M> 
    : Functor<MRdr, Env, M> 
    where M : Monad<M>
    where MRdr : MonadReaderT<MRdr, Env, M>
{
    public static virtual KArrow<MRdr, Env, M, A> Pure<A>(A value) =>
        MRdr.Lift(M.Pure(value));

    public static virtual KArrow<MRdr, Env, M, A> Lift<A>(KStar<M, A> ma) => 
        MRdr.Lift(constant<Env, KStar<M, A>>(ma));

    public static virtual KStar<M, A> Run<A>(KArrow<MRdr, Env, M, A> ma, Env env) =>
        ma.Morphism.Invoke(env);

    public static virtual KArrow<MRdr, Env, M, Env> Ask =>
        MRdr.Lift(identity<Env>().Map(M.Pure));

    public static virtual Transducer<Env1, KStar<M, A>> With<Env1, A>(
        Transducer<Env1, Env> f, 
        KArrow<MRdr, Env, M, A> ma) =>
        lift((Env1 env) => compose(f.Invoke(env), ma.Morphism).Invoke());

    public static virtual KArrow<MRdr, Env, M, A> Local<A>(
        Transducer<Env, Env> f,
        KArrow<MRdr, Env, M, A> ma) =>
        MRdr.Lift(MRdr.With(f, ma));

    public static virtual KArrow<MRdr, Env, M, A> Local<A>(
        Func<Env, Env> f,
        KArrow<MRdr, Env, M, A> ma) =>
        MRdr.Lift(MRdr.With(lift(f), ma));

    public static virtual KArrow<MRdr, Env, M, A> Flatten<A>(
        KArrow<MRdr, Env, M, KArrow<MRdr, Env, M, A>> mma) =>
        MRdr.Lift<A>(env => mma.Invoke(env).Invoke().Invoke(env));

    public static virtual KArrow<MRdr, Env, M, B> Bind<A, B>(
        KArrow<MRdr, Env, M, A> ma, 
        Transducer<A, KArrow<MRdr, Env, M, B>> f) =>
        MRdr.Flatten(MRdr.Map(ma, f));

    public static virtual KArrow<MRdr, Env, M, B> Bind<A, B>(
        KArrow<MRdr, Env, M, A> ma, 
        Func<A, KArrow<MRdr, Env, M, B>> f) =>
        MRdr.Bind(ma, lift(f));
}
