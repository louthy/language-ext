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
    public static virtual KArrow<F, Env, B> Map<A, B>(KArrow<F, Env, A> ma, Transducer<A, B> f) =>
        compose(ma, f);

    public static virtual KArrow<F, Env, B> Map<A, B>(KArrow<F, Env, A> ma, Func<A, B> f) =>
        F.Map(ma, lift(f));
}

public interface Functor<F, Env, G> : KLift<F, Env, G> 
    where F : Functor<F, Env, G>
    where G : Functor<G>
{
    public static virtual KArrow<F, Env, G, B> Map<A, B>(KArrow<F, Env, G, A> ma, Transducer<A, B> f) =>
        F.Lift(env => G.Map(ma.Morphism.Invoke(env), f));
    
    public static virtual KArrow<F, Env, G, B> Map<A, B>(KArrow<F, Env, G, A> ma, Func<A, B> f) =>
        F.Map(ma, lift(f));
}
