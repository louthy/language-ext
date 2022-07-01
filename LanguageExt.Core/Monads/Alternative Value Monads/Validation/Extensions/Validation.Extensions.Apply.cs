#nullable enable
using System;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

/// <summary>
/// Extension methods for `TryOptionAsync`
/// </summary>
public static partial class ValidationExtensions
{
    /// <summary>
    /// Applicative action
    /// </summary>
    /// <remarks>
    /// Applicative action 'runs' the first item then returns the result of the second (if neither fail). 
    /// </remarks>
    /// <param name="fa">Bound first argument</param>
    /// <param name="fb">Bound second argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, B> Action<MonoidFAIL, FAIL, A, B>(this Validation<MonoidFAIL, FAIL, A> fa, Validation<MonoidFAIL, FAIL, B> fb) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        default(ApplValidation<MonoidFAIL, FAIL, A, B>).Action(fa, fb);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, B> Apply<MonoidFAIL, FAIL, A, B>(this Validation<MonoidFAIL, FAIL, Func<A, B>> ff, Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        default(ApplValidation<MonoidFAIL, FAIL, A, B>).Apply(ff, fx);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, B> Apply<MonoidFAIL, FAIL, A, B>(this Func<A, B> ff, Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        default(ApplValidation<MonoidFAIL, FAIL, A, B>).Apply(Success<MonoidFAIL, FAIL, Func<A, B>>(ff), fx);

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, C>> Apply<MonoidFAIL, FAIL, A, B, C>(this Validation<MonoidFAIL, FAIL, Func<A, B, C>> ff, Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        ff.Map(curry).Apply(fx);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, C>> Apply<MonoidFAIL, FAIL, A, B, C>(this Func<A, B, C> ff, Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        curry(ff).Apply(fx);

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, D>>> Apply<MonoidFAIL, FAIL, A, B, C, D>(this Validation<MonoidFAIL, FAIL, Func<A, B, C, D>> ff, Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        ff.Map(curry).Apply(fx);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, D>>> Apply<MonoidFAIL, FAIL, A, B, C, D>(this Func<A, B, C, D> ff, Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        curry(ff).Apply(fx);

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Intermediate bound value type</typeparam>
    /// <typeparam name="E">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, Func<D, E>>>> Apply<MonoidFAIL, FAIL, A, B, C, D, E>(
        this Validation<MonoidFAIL, FAIL, Func<A, B, C, D, E>> ff, 
        Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        ff.Map(curry).Apply(fx);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Intermediate bound value type</typeparam>
    /// <typeparam name="E">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, Func<D, E>>>> Apply<MonoidFAIL, FAIL, A, B, C, D, E>(
        this Func<A, B, C, D, E> ff, 
        Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        curry(ff).Apply(fx);    

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Intermediate bound value type</typeparam>
    /// <typeparam name="E">Intermediate bound value type</typeparam>
    /// <typeparam name="F">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, Func<D, Func<E, F>>>>> Apply<MonoidFAIL, FAIL, A, B, C, D, E, F>(
        this Validation<MonoidFAIL, FAIL, Func<A, B, C, D, E, F>> ff, 
        Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        ff.Map(curry).Apply(fx);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Intermediate bound value type</typeparam>
    /// <typeparam name="E">Intermediate bound value type</typeparam>
    /// <typeparam name="F">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, Func<D, Func<E, F>>>>> Apply<MonoidFAIL, FAIL, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> ff, 
        Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        curry(ff).Apply(fx);

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Intermediate bound value type</typeparam>
    /// <typeparam name="E">Intermediate bound value type</typeparam>
    /// <typeparam name="F">Intermediate bound value type</typeparam>
    /// <typeparam name="G">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Apply<MonoidFAIL, FAIL, A, B, C, D, E, F, G>(
        this Validation<MonoidFAIL, FAIL, Func<A, B, C, D, E, F, G>> ff, 
        Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        ff.Map(curry).Apply(fx);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Intermediate bound value type</typeparam>
    /// <typeparam name="E">Intermediate bound value type</typeparam>
    /// <typeparam name="F">Intermediate bound value type</typeparam>
    /// <typeparam name="G">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Apply<MonoidFAIL, FAIL, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> ff, 
        Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        curry(ff).Apply(fx);    

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Intermediate bound value type</typeparam>
    /// <typeparam name="E">Intermediate bound value type</typeparam>
    /// <typeparam name="F">Intermediate bound value type</typeparam>
    /// <typeparam name="G">Intermediate bound value type</typeparam>
    /// <typeparam name="H">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Apply<MonoidFAIL, FAIL, A, B, C, D, E, F, G, H>(
        this Validation<MonoidFAIL, FAIL, Func<A, B, C, D, E, F, G, H>> ff, 
        Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        ff.Map(curry).Apply(fx);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Intermediate bound value type</typeparam>
    /// <typeparam name="E">Intermediate bound value type</typeparam>
    /// <typeparam name="F">Intermediate bound value type</typeparam>
    /// <typeparam name="G">Intermediate bound value type</typeparam>
    /// <typeparam name="H">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Apply<MonoidFAIL, FAIL, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> ff, 
        Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        curry(ff).Apply(fx);    

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Intermediate bound value type</typeparam>
    /// <typeparam name="E">Intermediate bound value type</typeparam>
    /// <typeparam name="F">Intermediate bound value type</typeparam>
    /// <typeparam name="G">Intermediate bound value type</typeparam>
    /// <typeparam name="H">Intermediate bound value type</typeparam>
    /// <typeparam name="I">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Apply<MonoidFAIL, FAIL, A, B, C, D, E, F, G, H, I>(
        this Validation<MonoidFAIL, FAIL, Func<A, B, C, D, E, F, G, H, I>> ff, 
        Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        ff.Map(curry).Apply(fx);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Applies the bound function to the bound argument, returning a bound result. 
    /// </remarks>
    /// <param name="ff">Bound function</param>
    /// <param name="fx">Bound argument</param>
    /// <typeparam name="A">Input bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Intermediate bound value type</typeparam>
    /// <typeparam name="D">Intermediate bound value type</typeparam>
    /// <typeparam name="E">Intermediate bound value type</typeparam>
    /// <typeparam name="F">Intermediate bound value type</typeparam>
    /// <typeparam name="G">Intermediate bound value type</typeparam>
    /// <typeparam name="H">Intermediate bound value type</typeparam>
    /// <typeparam name="I">Output bound value type</typeparam>
    /// <returns>Bound result of the application of the function to the argument</returns>
    public static Validation<MonoidFAIL, FAIL, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Apply<MonoidFAIL, FAIL, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> ff, 
        Validation<MonoidFAIL, FAIL, A> fx) 
        where MonoidFAIL : struct, Monoid<FAIL>, Eq<FAIL> =>
        curry(ff).Apply(fx);    
}

