using System;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public static partial class Prelude
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
    public static Task<B> action<A, B>(Task<A> fa, Task<B> fb) =>
        default(ApplTask<A, B>).Action(fa, fb);
    
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
    public static Task<B> apply<A, B>(Task<Func<A, B>> ff, Task<A> fx) =>
        default(ApplTask<A, B>).Apply(ff, fx);
    
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
    public static Task<B> apply<A, B>(Func<A, B> ff, Task<A> fx) =>
        default(ApplTask<A, B>).Apply(ff.AsTask(), fx);

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
    public static Task<Func<B, C>> apply<A, B, C>(Task<Func<A, B, C>> ff, Task<A> fx) =>
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
    public static Task<Func<B, C>> apply<A, B, C>(Func<A, B, C> ff, Task<A> fx) =>
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
    public static Task<Func<B, Func<C, D>>> apply<A, B, C, D>(Task<Func<A, B, C, D>> ff, Task<A> fx) =>
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
    public static Task<Func<B, Func<C, D>>> apply<A, B, C, D>(Func<A, B, C, D> ff, Task<A> fx) =>
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
    public static Task<Func<B, Func<C, Func<D, E>>>> apply<A, B, C, D, E>(
        Task<Func<A, B, C, D, E>> ff, 
        Task<A> fx) =>
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
    public static Task<Func<B, Func<C, Func<D, E>>>> apply<A, B, C, D, E>(
        Func<A, B, C, D, E> ff, 
        Task<A> fx) =>
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
    public static Task<Func<B, Func<C, Func<D, Func<E, F>>>>> apply<A, B, C, D, E, F>(
        Task<Func<A, B, C, D, E, F>> ff, 
        Task<A> fx) =>
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
    public static Task<Func<B, Func<C, Func<D, Func<E, F>>>>> apply<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F> ff, 
        Task<A> fx) =>
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
    public static Task<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> apply<A, B, C, D, E, F, G>(
        Task<Func<A, B, C, D, E, F, G>> ff, 
        Task<A> fx) =>
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
    public static Task<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> apply<A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G> ff, 
        Task<A> fx) =>
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
    public static Task<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> apply<A, B, C, D, E, F, G, H>(
        Task<Func<A, B, C, D, E, F, G, H>> ff, 
        Task<A> fx) =>
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
    public static Task<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> apply<A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H> ff, 
        Task<A> fx) =>
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
    public static Task<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> apply<A, B, C, D, E, F, G, H, I>(
        Task<Func<A, B, C, D, E, F, G, H, I>> ff, 
        Task<A> fx) =>
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
    public static Task<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> apply<A, B, C, D, E, F, G, H, I>(
        Func<A, B, C, D, E, F, G, H, I> ff, 
        Task<A> fx) =>
        curry(ff).Apply(fx);    
}

