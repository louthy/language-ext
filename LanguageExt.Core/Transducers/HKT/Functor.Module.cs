using System;
using LanguageExt.Pipes;

namespace LanguageExt.HKT;

public static class Functor
{
    public static Functor<F, B> map<F, A, B>(Functor<F, A> ma, Transducer<A, B> f)
        where F : Functor<F> =>
        F.Map(ma, f);

    public static Functor<F, B> map<F, A, B>(Functor<F, A> ma, Func<A, B> f) 
        where F : Functor<F> =>
        F.Map(ma, f);
    
    public static FunctorT<F, G, B> map<F, G, A, B>(FunctorT<F, G, A> ma, Transducer<A, B> f) 
        where F : FunctorT<F, G>
        where G : Functor<G> =>
        F.Map(ma, f);
    
    public static FunctorT<F, G, B> map<F, G, A, B>(FunctorT<F, G, A> ma, Func<A, B> f) 
        where F : FunctorT<F, G>
        where G : Functor<G> =>
        F.Map(ma, f);
    
    public static Monad<F, A> AsMonad<F, A>(this Functor<F, A> fa)
        where F : Functor<F>, Monad<F> => 
        (Monad<F, A>)fa;
    
    public static MonadT<F, G, A> AsMonad<F, G, A>(this FunctorT<F, G, A> fa)
        where F : FunctorT<F, G>, MonadT<F, G> 
        where G : Functor<G>, Monad<G> => 
        (MonadT<F, G, A>)fa;

}
