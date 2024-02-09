using System;

namespace LanguageExt.HKT;

/// <summary>
/// Functor module
/// </summary>
public static class Functor
{
    public static Functor<F, B> map<F, A, B>(Functor<F, A> ma, Transducer<A, B> f)
        where F : Functor<F> =>
        F.Map(ma, f);

    public static Functor<F, B> map<F, A, B>(Functor<F, A> ma, Func<A, B> f) 
        where F : Functor<F> =>
        F.Map(ma, f);
    
    public static Monad<F, A> AsMonad<F, A>(this Functor<F, A> fa)
        where F : Functor<F>, Monad<F> => 
        (Monad<F, A>)fa;
}
