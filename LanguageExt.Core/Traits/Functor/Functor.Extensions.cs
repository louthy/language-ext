using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Functor module
/// </summary>
public static class FunctorExtensions
{
    public static K<F, B> Map<F, A, B>(this K<F, A> ma, Func<A, B> f) 
        where F : Functor<F> =>
        F.Map(f, ma);
    
    public static K<F, B> Map<F, A, B>(this Func<A, B> f, K<F, A> ma) 
        where F : Functor<F> =>
        F.Map(f, ma);
    
    public static K<F, Func<B, C>> Map<F, A, B, C>(this Func<A, B, C> f, K<F, A> ma) 
        where F : Functor<F> =>
        F.Map(x => curry(f)(x), ma);
    
    public static K<F, Func<B, Func<C, D>>> Map<F, A, B, C, D>(this Func<A, B, C, D> f, K<F, A> ma) 
        where F : Functor<F> =>
        F.Map(x => curry(f)(x), ma);
    
    public static K<F, Func<B, Func<C, Func<D, E>>>> Map<F, A, B, C, D, E>(this Func<A, B, C, D, E> f, K<F, A> ma) 
        where F : Functor<F> =>
        F.Map(x => curry(f)(x), ma);
    
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<Fnctr, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<Fnctr, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
    
    public static K<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, K<Fnctr, A> ma) 
        where Fnctr : Functor<Fnctr> =>
        Fnctr.Map(x => curry(f)(x), ma);
}
