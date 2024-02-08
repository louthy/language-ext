using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

public interface KLift<F, Env, G> 
    where F : KLift<F, Env, G>
    where G : KLift<G>
{
    public static abstract KArrow<F, Env, G, A> Lift<A>(Transducer<Env, KStar<G, A>> f);

    public static virtual KArrow<F, Env, G, A> Lift<A>(Func<Env, KStar<G, A>> f) =>
        F.Lift(lift(f));
}

public interface KLift<F, Env> 
    where F : KLift<F, Env>
{
    public static abstract KArrow<F, Env, A> Lift<A>(Transducer<Env, A> f);

    public static virtual KArrow<F, Env, A> Lift<A>(Func<Env, A> f) =>
        F.Lift(lift(f));
}

public interface KLift<F> 
    where F : KLift<F>
{
    public static abstract KStar<F, A> Lift<A>(Transducer<Unit, A> f);

    public static virtual KStar<F, A> Lift<A>(Func<Unit, A> f) =>
        F.Lift(lift(f));
}
