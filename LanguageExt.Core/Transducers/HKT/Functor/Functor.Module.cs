using System;

namespace LanguageExt.HKT;

/// <summary>
/// Functor module
/// </summary>
public static class Functor
{
    public static Functor<F, B> map<F, A, B>(Transducer<A, B> f, Functor<F, A> ma)
        where F : Functor<F> =>
        F.Map(f, ma);

    public static Functor<F, B> map<F, A, B>(Func<A, B> f, Functor<F, A> ma) 
        where F : Functor<F> =>
        F.Map(f, ma);
    
    public static Applicative<F, A> AsApplicative<F, A>(this Functor<F, A> fa)
        where F : Applicative<F> => 
        (Applicative<F, A>)fa;
    
    public static Alternative<F, A> AsAlternative<F, A>(this Functor<F, A> fa)
        where F : Alternative<F> => 
        (Alternative<F, A>)fa;
    
    public static Monad<F, A> AsMonad<F, A>(this Functor<F, A> fa)
        where F : Functor<F>, Monad<F> => 
        (Monad<F, A>)fa;
}
