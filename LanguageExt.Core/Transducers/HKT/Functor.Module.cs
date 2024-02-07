using System;

namespace LanguageExt.HKT;

public static class Functor
{
    public static KStar<F, B> map<F, A, B>(KStar<F, A> ma, Transducer<A, B> f)
        where F : Functor<F> =>
        F.Map(ma, f);

    public static KStar<F, B> map<F, A, B>(KStar<F, A> ma, Func<A, B> f) 
        where F : Functor<F> =>
        F.Map(ma, f);
    
    public static KArrow<F, Env, B> map<F, Env, A, B>(KArrow<F, Env, A> ma, Transducer<A, B> f)
        where F : Functor<F, Env> =>
        F.Map(ma, f);

    public static KArrow<F, Env, B> map<F, Env, A, B>(KArrow<F, Env, A> ma, Func<A, B> f)
        where F : Functor<F, Env> =>
        F.Map(ma, f);
    
    public static KArrow<F, Env, G, B> map<F, Env, G, A, B>(KArrow<F, Env, G, A> ma, Transducer<A, B> f) 
        where F : Functor<F, Env, G>
        where G : Functor<G> =>
        F.Map(ma, f);
    
    public static KArrow<F, Env, G, B> map<F, Env, G, A, B>(KArrow<F, Env, G, A> ma, Func<A, B> f) 
        where F : Functor<F, Env, G>
        where G : Functor<G> =>
        F.Map(ma, f);
}
