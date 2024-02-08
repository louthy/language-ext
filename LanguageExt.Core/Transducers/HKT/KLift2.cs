using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

public interface KLift2<F, Env, X, G> 
    where F : KLift2<F, Env, X, G>
    where G : KLift<G>
{
    public static abstract KArrow2<F, Env, X, G, A> Lift<A>(Transducer<Env, Sum<X, KStar<G, A>>> f);

    public static virtual KArrow2<F, Env, X, G, A> Lift<A>(Func<Env, Sum<X, KStar<G, A>>> f) =>
        F.Lift(lift(f));
}

public interface KLift2<F, Env, X> where F : KLift2<F, Env, X>
{
    public static abstract KArrow2<F, Env, X, A> Lift<A>(Transducer<Env, Sum<X, A>> f);

    public static virtual KArrow2<F, Env, X, A> Lift<A>(Func<Env, Sum<X, A>> f) =>
        F.Lift(lift(f));
}

public interface KLift2<F, X> where F : KLift2<F, X>
{
    public static abstract KStar2<F, X, A> Lift<A>(Transducer<Unit, Sum<X, A>> f);

    public static virtual KStar2<F, X, A> Lift<A>(Func<Unit, Sum<X, A>> f) =>
        F.Lift(lift(f));
}
