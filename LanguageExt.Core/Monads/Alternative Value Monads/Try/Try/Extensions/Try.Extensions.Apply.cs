#nullable enable
using System;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;

/// <summary>
/// Extension methods for `Try`
/// </summary>
public static partial class TryExtensions
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
    public static Try<B> Action<A, B>(this Try<A> fa, Try<B> fb) =>
        default(ApplTry<A, B>).Action(fa, fb);
    
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
    public static Try<B> Apply<A, B>(this Try<Func<A, B>> ff, Try<A> fx) =>
        default(ApplTry<A, B>).Apply(ff, fx);
    
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
    public static Try<B> Apply<A, B>(this Func<A, B> ff, Try<A> fx) =>
        default(ApplTry<A, B>).Apply(TrySucc(ff), fx);

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
    public static Try<Func<B, C>> Apply<A, B, C>(this Try<Func<A, B, C>> ff, Try<A> fx) =>
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
    public static Try<Func<B, C>> Apply<A, B, C>(this Func<A, B, C> ff, Try<A> fx) =>
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
    public static Try<Func<B, Func<C, D>>> Apply<A, B, C, D>(this Try<Func<A, B, C, D>> ff, Try<A> fx) =>
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
    public static Try<Func<B, Func<C, D>>> Apply<A, B, C, D>(this Func<A, B, C, D> ff, Try<A> fx) =>
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
    public static Try<Func<B, Func<C, Func<D, E>>>> Apply<A, B, C, D, E>(
        this Try<Func<A, B, C, D, E>> ff, 
        Try<A> fx) =>
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
    public static Try<Func<B, Func<C, Func<D, E>>>> Apply<A, B, C, D, E>(
        this Func<A, B, C, D, E> ff, 
        Try<A> fx) =>
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
    public static Try<Func<B, Func<C, Func<D, Func<E, F>>>>> Apply<A, B, C, D, E, F>(
        this Try<Func<A, B, C, D, E, F>> ff, 
        Try<A> fx) =>
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
    public static Try<Func<B, Func<C, Func<D, Func<E, F>>>>> Apply<A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> ff, 
        Try<A> fx) =>
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
    public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Apply<A, B, C, D, E, F, G>(
        this Try<Func<A, B, C, D, E, F, G>> ff, 
        Try<A> fx) =>
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
    public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Apply<A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> ff, 
        Try<A> fx) =>
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
    public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Apply<A, B, C, D, E, F, G, H>(
        this Try<Func<A, B, C, D, E, F, G, H>> ff, 
        Try<A> fx) =>
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
    public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Apply<A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> ff, 
        Try<A> fx) =>
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
    public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Apply<A, B, C, D, E, F, G, H, I>(
        this Try<Func<A, B, C, D, E, F, G, H, I>> ff, 
        Try<A> fx) =>
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
    public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Apply<A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> ff, 
        Try<A> fx) =>
        curry(ff).Apply(fx);    
}

