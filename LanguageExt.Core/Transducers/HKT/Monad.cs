using System;
using static LanguageExt.Prelude;
using static LanguageExt.Transducer;

namespace LanguageExt.HKT;

public interface Functor<F> : KLift<F> 
    where F : Functor<F>
{
    public static virtual KStar<F, B> Map<A, B>(KStar<F, A> ma, Transducer<A, B> f) =>
        compose(ma, f);

    public static virtual KStar<F, B> Map<A, B>(KStar<F, A> ma, Func<A, B> f) =>
        F.Map(ma, lift(f));
}

public interface Functor<F, Env> : KLift<F, Env> 
    where F : Functor<F, Env>
{
    public static virtual KArr<F, Env, B> Map<A, B>(KArr<F, Env, A> ma, Transducer<A, B> f) =>
        compose(ma, f);

    public static virtual KArr<F, Env, B> Map<A, B>(KArr<F, Env, A> ma, Func<A, B> f) =>
        F.Map(ma, lift(f));
}

public interface Functor<F, Env, G> : KLift<F, Env, G> 
    where F : Functor<F, Env, G>
    where G : Functor<G>
{
    public static virtual KArr<F, Env, G, B> Map<A, B>(KArr<F, Env, G, A> ma, Transducer<A, B> f) =>
        F.Lift(env => G.Map(ma.Morphism.Partial(env), f));
    
    public static virtual KArr<F, Env, G, B> Map<A, B>(KArr<F, Env, G, A> ma, Func<A, B> f) =>
        F.Map(ma, lift(f));
}

public static class Functor
{
    public static KStar<F, B> map<F, A, B>(KStar<F, A> ma, Transducer<A, B> f)
        where F : Functor<F> =>
        F.Map(ma, f);

    public static KStar<F, B> map<F, A, B>(KStar<F, A> ma, Func<A, B> f) 
        where F : Functor<F> =>
        F.Map(ma, f);
    
    public static KArr<F, Env, B> map<F, Env, A, B>(KArr<F, Env, A> ma, Transducer<A, B> f)
        where F : Functor<F, Env> =>
        F.Map(ma, f);

    public static KArr<F, Env, B> map<F, Env, A, B>(KArr<F, Env, A> ma, Func<A, B> f)
        where F : Functor<F, Env> =>
        F.Map(ma, f);
    
    public static KArr<F, Env, G, B> map<F, Env, G, A, B>(KArr<F, Env, G, A> ma, Transducer<A, B> f) 
        where F : Functor<F, Env, G>
        where G : Functor<G> =>
        F.Map(ma, f);
    
    public static KArr<F, Env, G, B> map<F, Env, G, A, B>(KArr<F, Env, G, A> ma, Func<A, B> f) 
        where F : Functor<F, Env, G>
        where G : Functor<G> =>
        F.Map(ma, f);
}

public interface Monad<M> : Functor<M> 
    where M : Monad<M>
{
    public static virtual KStar<M, A> Pure<A>(A value) => 
        M.Lift(constant<Unit, A>(value));

    public static virtual KStar<M, A> Flatten<A>(KStar<M, KStar<M, A>> mma) => 
        mma.Partial();

    public static virtual KStar<M, B> Bind<A, B>(KStar<M, A> ma, Transducer<A, KStar<M, B>> f) =>
        M.Flatten(M.Map(ma, f));

    public static virtual KStar<M, B> Bind<A, B>(KStar<M, A> ma, Func<A, KStar<M, B>> f) =>
        M.Bind(ma, lift(f));
}

public static class Monad
{
    public static KStar<M, A> pure<M, A>(A value) 
        where M : Monad<M> =>
        M.Pure(value);

    public static KStar<M, A> flatten<M, A>(KStar<M, KStar<M, A>> mma)
        where M : Monad<M> =>
        M.Flatten(mma);

    public static KStar<M, B> bind<M, A, B>(KStar<M, A> ma, Transducer<A, KStar<M, B>> f)
        where M  : Monad<M> =>
        M.Bind(ma, f);

    public static KStar<M, B> bind<M, A, B>(KStar<M, A> ma, Func<A, KStar<M, B>> f)
        where M : Monad<M> =>
        M.Bind(ma, f);
    
    public static MB bind<M, MB, A, B>(KStar<M, A> ma, Transducer<A, MB> f)
        where MB : KStar<M, B>
        where M  : Monad<M> =>
        (MB)M.Bind(ma, f.Map(mb => mb.AsKind()));

    public static MB bind<M, MB, A, B>(KStar<M, A> ma, Func<A, MB> f)
        where MB : KStar<M, B>
        where M : Monad<M> =>
        bind<M, MB, A, B>(ma, lift(f));
}

public interface MonadReaderT<MRdr, Env, M> 
    : Functor<MRdr, Env, M> 
    where M : Monad<M>
    where MRdr : MonadReaderT<MRdr, Env, M>
{
    public static virtual KArr<MRdr, Env, M, A> Pure<A>(A value) =>
        MRdr.Lift(M.Pure(value));

    public static virtual KArr<MRdr, Env, M, A> Lift<A>(KStar<M, A> ma) => 
        MRdr.Lift(constant<Env, KStar<M, A>>(ma));

    public static virtual KArr<MRdr, Env, M, Env> Ask =>
        MRdr.Lift(identity<Env>().Map(M.Pure));

    public static virtual KArr<MRdr, Env, M, A> Flatten<A>(KArr<MRdr, Env, M, KArr<MRdr, Env, M, A>> mma) => 
        MRdr.Lift<A>(env => mma.Partial(env).Morphism.Partial().Partial(env));

    public static virtual KArr<MRdr, Env, M, B> Bind<A, B>(
        KArr<MRdr, Env, M, A> ma, 
        Transducer<A, KArr<MRdr, Env, M, B>> f) =>
        MRdr.Flatten(MRdr.Map(ma, f));

    public static virtual KArr<MRdr, Env, M, B> Bind<A, B>(
        KArr<MRdr, Env, M, A> ma, 
        Func<A, KArr<MRdr, Env, M, B>> f) =>
        MRdr.Bind(ma, lift(f));
}

public static class MonadReaderT
{
    public static KArr<MRdr, Env, M, A> pure<MRdr, Env, M, A>(A value)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Pure(value);

    public static KArr<MRdr, Env, M, A> lift<MRdr, Env, M, A>(KStar<M, A> ma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Lift(ma);

    public static KArr<MRdr, Env, M, Env> ask<MRdr, Env, M>()
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Ask;

    public static KArr<MRdr, Env, M, A> flatten<MRdr, Env, M, A>(KArr<MRdr, Env, M, KArr<MRdr, Env, M, A>> mma)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Flatten(mma);

    public static KArr<MRdr, Env, M, B> map<MRdr, Env, M, A, B>(KArr<MRdr, Env, M, A> ma, Transducer<A, B> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        Functor.map(ma, f);

    public static KArr<MRdr, Env, M, B> map<MRdr, Env, M, A, B>(KArr<MRdr, Env, M, A> ma, Func<A, B> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        Functor.map(ma, f);

    public static KArr<MRdr, Env, M, B> bind<MRdr, Env, M, A, B>(
        KArr<MRdr, Env, M, A> ma,
        Transducer<A, KArr<MRdr, Env, M, B>> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Bind(ma, f);

    public static KArr<MRdr, Env, M, B> bind<MRdr, Env, M, A, B>(
        KArr<MRdr, Env, M, A> ma,
        Func<A, KArr<MRdr, Env, M, B>> f)
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        MRdr.Bind(ma, f);

    public static MB bind<MRdr, MB, Env, M, A, B>(
        KArr<MRdr, Env, M, A> ma,
        Transducer<A, MB> f)
        where MB : KArr<MRdr, Env, M, B>
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        (MB)MRdr.Bind(ma, f.Map(x => x.AsKind()));

    public static MB bind<MRdr, MB, Env, M, A, B>(
        KArr<MRdr, Env, M, A> ma,
        Func<A, MB> f)
        where MB : KArr<MRdr, Env, M, B>
        where M : Monad<M>
        where MRdr : MonadReaderT<MRdr, Env, M> =>
        bind<MRdr, MB, Env, M, A, B>(ma, Prelude.lift(f));
}

/*
public interface MonadFail<E>
{
    public static abstract Transducer<A, Sum<E, A>> Pure<A>();
    public static abstract Transducer<E, Sum<E, A>> Fail<A>();
    public static abstract Transducer<A, Transducer<Unit, Sum<E, A>>> Flatten<A>();
}

public interface MonadReader<Env>
{
    public static abstract Transducer<A, A> Pure<A>();
    public static abstract Transducer<A, Transducer<Env, A>> Bind<A>();
}

public interface MonadReaderFail<Env, E>
{
    public static abstract Transducer<A, A> Pure<A>();
    public static abstract Transducer<E, Sum<E, A>> Fail<A>();
    public static abstract Transducer<A, Transducer<Env, B>> Bind<A, B>();
}*/


/*
/// <summary>
/// Monad bind trait
/// </summary>
public interface Monad<M> : Applicative<M>
    where M : Monad<M>
{
    /// <summary>
    /// Monad bind
    /// </summary>
    //public static abstract KArr<M, Unit, B> Bind<A, B>(KArr<M, Unit, A> mx, Transducer<A, KArr<M, Unit, B>> f);
    public static abstract Transducer<Unit, M> Bind<A, B>();
}

/// <summary>
/// Monad bind trait with fixed input type
/// </summary>
public interface MonadReader<M, Env> : Applicative<M, Env>
    where M : MonadReader<M, Env>
{
    /// <summary>
    /// Monad bind
    /// </summary>
    public static abstract KArr<M, Env, B> Bind<A, B>(KArr<M, Env, A> mx, Transducer<A, KArr<M, Env, B>> f);
}

public static class MonadExtensions
{
    /// <summary>
    /// Monad bind
    /// </summary>
    public static KArr<M, Unit, B> Bind<M, A, B>(this Monad<M> self, KArr<M, Unit, A> mx, Func<A, KArr<M, Unit, B>> f)
        where M : Monad<M> =>
        M.Bind(mx, Prelude.lift(f));

    /// <summary>
    /// Monad bind
    /// </summary>
    public static KArr<M, Unit, A> Flatten<M, A>(this KArr<M, Unit, KArr<M, Unit, A>> mmx)
        where M : Monad<M> =>
        M.Bind(mmx, Transducer.identity<KArr<M, Unit, A>>());

    /// <summary>
    /// Monad bind
    /// </summary>
    public static KArr<M, Env, B> Bind<Env,M, A, B>(this KArr<M, Env, A> mx, Func<A, KArr<M, Env, B>> f)
        where M : MonadReader<M, Env> =>
        M.Bind(mx, Prelude.lift(f));

    /// <summary>
    /// Monad bind
    /// </summary>
    public static KArr<M, Env, A> Flatten<Env,M, A>(this KArr<M, Env, KArr<M, Env, A>> mmx)
        where M : MonadReader<M, Env> =>
        M.Bind(mmx, Transducer.identity<KArr<M, Env, A>>());
}
*/
