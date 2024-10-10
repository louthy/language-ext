using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class FreeExtensions
{
    public static Free<Fnctr, A> As<Fnctr, A>(this K<Free<Fnctr>, A> ma)
        where Fnctr : Functor<Fnctr> =>
        (Free<Fnctr, A>)ma;
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="mf">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, B> Apply<Fnctr, A, B>(this K<Free<Fnctr>, Func<A, B>> mf, K<Free<Fnctr>, A> ma) 
        where Fnctr : Functor<Fnctr> => 
        (mf, ma) switch
        {
            (Pure<Fnctr, Func<A, B>> (var f), Pure<Fnctr, A> (var a)) => new Pure<Fnctr, B>(f(a)),
            (Pure<Fnctr, Func<A, B>> (var f), Bind<Fnctr, A> (var a)) => new Bind<Fnctr, B>(a.Map(x => x.Map(f).As())),
            (Bind<Fnctr, Func<A, B>> (var f), Bind<Fnctr, A> a)       => new Bind<Fnctr, B>(f.Map(x => x.Apply(a).As())),
            _                                                         => throw new InvalidOperationException()
        };

    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="mf">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, C>> Apply<Fnctr, A, B, C>(
        this K<Free<Fnctr>, Func<A, B, C>> mf, K<Free<Fnctr>, A> ma)
        where Fnctr : Functor<Fnctr> =>
        mf.Map(curry).Apply(ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="mf">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, D>>> Apply<Fnctr, A, B, C, D>(
        this K<Free<Fnctr>, Func<A, B, C, D>> mf, K<Free<Fnctr>, A> ma) 
        where Fnctr : Functor<Fnctr> => 
        mf.Map(curry).Apply(ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="mf">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, E>>>> Apply<Fnctr, A, B, C, D, E>(
        this K<Free<Fnctr>, Func<A, B, C, D, E>> mf, K<Free<Fnctr>, A> ma) 
        where Fnctr : Functor<Fnctr> => 
        mf.Map(curry).Apply(ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="mf">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, F>>>>> Apply<Fnctr, A, B, C, D, E, F>(
        this K<Free<Fnctr>, Func<A, B, C, D, E, F>> mf, K<Free<Fnctr>, A> ma)  
        where Fnctr : Functor<Fnctr> => 
        mf.Map(curry).Apply(ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="mf">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Apply<Fnctr, A, B, C, D, E, F, G>(
        this K<Free<Fnctr>, Func<A, B, C, D, E, F, G>> mf, K<Free<Fnctr>, A> ma)  
        where Fnctr : Functor<Fnctr> => 
        mf.Map(curry).Apply(ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="mf">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Apply<Fnctr, A, B, C, D, E, F, G, H>(
        this K<Free<Fnctr>, Func<A, B, C, D, E, F, G, H>> mf, K<Free<Fnctr>, A> ma)  
        where Fnctr : Functor<Fnctr> => 
        mf.Map(curry).Apply(ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="mf">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Apply<Fnctr, A, B, C, D, E, F, G, H, I>(
        this K<Free<Fnctr>, Func<A, B, C, D, E, F, G, H, I>> mf, K<Free<Fnctr>, A> ma)  
        where Fnctr : Functor<Fnctr> => 
        mf.Map(curry).Apply(ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="mf">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Apply<Fnctr, A, B, C, D, E, F, G, H, I, J>(
        this K<Free<Fnctr>, Func<A, B, C, D, E, F, G, H, I, J>> mf, K<Free<Fnctr>, A> ma)  
        where Fnctr : Functor<Fnctr> => 
        mf.Map(curry).Apply(ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="mf">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Apply<Fnctr, A, B, C, D, E, F, G, H, I, J, K>(
        this K<Free<Fnctr>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, K<Free<Fnctr>, A> ma) 
        where Fnctr : Functor<Fnctr> => 
        mf.Map(curry).Apply(ma);
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, B> Map<Fnctr, A, B>(this Func<A, B> f, K<Free<Fnctr>, A> ma) 
        where Fnctr : Functor<Fnctr> => 
        ma.Map(f).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, C>> Map<Fnctr, A, B, C>(
        this Func<A, B, C> f, K<Free<Fnctr>, A> ma) 
        where Fnctr : Functor<Fnctr> => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, D>>> Map<Fnctr, A, B, C, D>(
        this Func<A, B, C, D> f, K<Free<Fnctr>, A> ma) 
        where Fnctr : Functor<Fnctr> => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, E>>>> Map<Fnctr, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, K<Free<Fnctr>, A> ma) 
        where Fnctr : Functor<Fnctr> => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<Fnctr, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, K<Free<Fnctr>, A> ma)  
        where Fnctr : Functor<Fnctr> => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<Fnctr, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, K<Free<Fnctr>, A> ma)  
        where Fnctr : Functor<Fnctr> => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, K<Free<Fnctr>, A> ma)  
        where Fnctr : Functor<Fnctr> => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, K<Free<Fnctr>, A> ma)  
        where Fnctr : Functor<Fnctr> => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, K<Free<Fnctr>, A> ma)  
        where Fnctr : Functor<Fnctr> => 
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Free<Fnctr, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<Fnctr, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, K<Free<Fnctr>, A> ma) 
        where Fnctr : Functor<Fnctr> => 
        ma.Map(x => curry(f)(x)).As();     
}
