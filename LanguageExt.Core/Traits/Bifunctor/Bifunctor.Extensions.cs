using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Functor module
/// </summary>
public static class BifunctorExtensions
{
    /// <summary>
    /// Functor bimap.  Maps all contained values of `A` to values of `B` and every value of `L` to `M`
    /// </summary>
    /// <param name="First">Mapping function</param>
    /// <param name="Second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, M, B> BiMap<F, L, A, M, B>(this K<F, L, A> fab, Func<L, M> First, Func<A, B> Second) 
        where F : Bifunctor<F> =>
        F.BiMap(First, Second, fab);
    
    /// <summary>
    /// Functor bimap (with function currying)
    /// </summary>
    public static K<F, Func<M, N>, Func<B, C>> BiMap<F, L, M, N, A, B, C>(
        this K<F, L, A> ma, 
        Func<L, M, N> First, 
        Func<A, B, C> Second) 
        where F : Bifunctor<F> =>
        ma.BiMap(curry(First), curry(Second));    
    
    /// <summary>
    /// Functor bimap (with function currying)
    /// </summary>
    public static K<F, Func<M, Func<N, O>>, Func<B, Func<C, D>>> BiMap<F, L, M, N, O, A, B, C, D>(
        this K<F, L, A> ma, 
        Func<L, M, N, O> First, 
        Func<A, B, C, D> Second) 
        where F : Bifunctor<F> =>
        ma.BiMap(curry(First), curry(Second));

    /// <summary>
    /// Functor bimap (with function currying)
    /// </summary>
    public static K<F, Func<M, Func<N, Func<O, P>>>, Func<B, Func<C, Func<D, E>>>> BiMap<F, L, M, N, O, P, A, B, C, D, E>(
        this K<F, L, A> ma,
        Func<L, M, N, O, P> First,
        Func<A, B, C, D, E> Second)
        where F : Bifunctor<F> =>
        ma.BiMap(curry(First), curry(Second));

    /// <summary>
    /// Functor bimap (with function currying)
    /// </summary>
    public static K<Fnctr, Func<M, Func<N, Func<O, Func<P, Q>>>>, Func<B, Func<C, Func<D, Func<E, F>>>>> BiMap<Fnctr, L, M, N, O, P, Q, A, B, C, D, E, F>(
        this K<Fnctr, L, A> ma,
        Func<L, M, N, O, P, Q> First,
        Func<A, B, C, D, E, F> Second)
        where Fnctr : Bifunctor<Fnctr> =>
        ma.BiMap(curry(First), curry(Second));
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, M, A> MapFirst<F, L, A, M>(this K<F, L, A> fab, Func<L, M> first) 
        where F : Bifunctor<F> =>
        F.MapFirst(first, fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, Func<M, N>, A> MapFirst<F, L, A, M, N>(this K<F, L, A> fab, Func<L, M, N> first) 
        where F : Bifunctor<F> =>
        F.MapFirst(curry(first), fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, Func<M, Func<N, O>>, A> MapFirst<F, L, A, M, N, O>(this K<F, L, A> fab, Func<L, M, N, O> first) 
        where F : Bifunctor<F> =>
        F.MapFirst(curry(first), fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, Func<M, Func<N, Func<O, P>>>, A> MapFirst<F, L, A, M, N, O, P>(this K<F, L, A> fab, Func<L, M, N, O, P> first) 
        where F : Bifunctor<F> =>
        F.MapFirst(curry(first), fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, Func<M, Func<N, Func<O, Func<P, Q>>>>, A> MapFirst<F, L, A, M, N, O, P, Q>(this K<F, L, A> fab, Func<L, M, N, O, P, Q> first) 
        where F : Bifunctor<F> =>
        F.MapFirst(curry(first), fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, M, A> MapFirst<F, L, A, M>(this Func<L, M> first, K<F, L, A> fab) 
        where F : Bifunctor<F> =>
        F.MapFirst(first, fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, Func<M, N>, A> MapFirst<F, L, A, M, N>(this Func<L, M, N> first, K<F, L, A> fab) 
        where F : Bifunctor<F> =>
        F.MapFirst(curry(first), fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, Func<M, Func<N, O>>, A> MapFirst<F, L, A, M, N, O>(this Func<L, M, N, O> first, K<F, L, A> fab) 
        where F : Bifunctor<F> =>
        F.MapFirst(curry(first), fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, Func<M, Func<N, Func<O, P>>>, A> MapFirst<F, L, A, M, N, O, P>(this Func<L, M, N, O, P> first, K<F, L, A> fab) 
        where F : Bifunctor<F> =>
        F.MapFirst(curry(first), fab);
    
    /// <summary>
    /// Map covariantly over the first argument.
    /// </summary>
    /// <param name="first">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, Func<M, Func<N, Func<O, Func<P, Q>>>>, A> MapFirst<F, L, A, M, N, O, P, Q>(this Func<L, M, N, O, P, Q> first, K<F, L, A> fab) 
        where F : Bifunctor<F> =>
        F.MapFirst(curry(first), fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, L, B> MapSecond<F, L, A, B>(this K<F, L, A> fab, Func<A, B> second) 
        where F : Bifunctor<F> =>
        F.MapSecond(second, fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, L, Func<B, C>> MapSecond<F, L, A, B, C>(this K<F, L, A> fab, Func<A, B, C> second) 
        where F : Bifunctor<F> =>
        F.MapSecond(curry(second), fab);    
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, L, Func<B, Func<C, D>>> MapSecond<F, L, A, B, C, D>(this K<F, L, A> fab, Func<A, B, C, D> second) 
        where F : Bifunctor<F> =>
        F.MapSecond(curry(second), fab);    
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, L, Func<B, Func<C, Func<D, E>>>> MapSecond<F, L, A, B, C, D, E>(this K<F, L, A> fab, Func<A, B, C, D, E> second) 
        where F : Bifunctor<F> =>
        F.MapSecond(curry(second), fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<BF, L, Func<B, Func<C, Func<D, Func<E, F>>>>> MapSecond<BF, L, A, B, C, D, E, F>(this K<BF, L, A> fab, Func<A, B, C, D, E, F> second) 
        where BF : Bifunctor<BF> =>
        BF.MapSecond(curry(second), fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, L, B> MapSecond<F, L, A, B>(this Func<A, B> second, K<F, L, A> fab) 
        where F : Bifunctor<F> =>
        F.MapSecond(second, fab);
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, L, Func<B, C>> MapSecond<F, L, A, B, C>(this Func<A, B, C> second, K<F, L, A> fab) 
        where F : Bifunctor<F> =>
        F.MapSecond(curry(second), fab);    
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, L, Func<B, Func<C, D>>> MapSecond<F, L, A, B, C, D>(this Func<A, B, C, D> second, K<F, L, A> fab) 
        where F : Bifunctor<F> =>
        F.MapSecond(curry(second), fab);    
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<F, L, Func<B, Func<C, Func<D, E>>>> MapSecond<F, L, A, B, C, D, E>(this Func<A, B, C, D, E> second, K<F, L, A> fab) 
        where F : Bifunctor<F> =>
        F.MapSecond(curry(second), fab);     
    
    /// <summary>
    /// Map covariantly over the second argument.
    /// </summary>
    /// <param name="second">Mapping function</param>
    /// <param name="fab">Bifunctor structure</param>
    /// <typeparam name="F">Bifunctor trait</typeparam>
    /// <returns>Mapped bifunctor</returns>
    public static K<BF, L, Func<B, Func<C, Func<D, Func<E, F>>>>> MapSecond<BF, L, A, B, C, D, E, F>(this Func<A, B, C, D, E, F> second, K<BF, L, A> fab) 
        where BF : Bifunctor<BF> =>
        BF.MapSecond(curry(second), fab);
}
