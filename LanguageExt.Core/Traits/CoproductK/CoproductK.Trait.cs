using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Co-product trait (abstract version of `Either`)
/// </summary>
/// <typeparam name="F">Self</typeparam>
public interface CoproductK<F> : CoproductCons<F>
    where F : CoproductK<F>
{
    /// <summary>
    /// Pattern-match either the left or right value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Left">Function to map the left value to a result</param>
    /// <param name="Right">Function to map the right value to a result</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <typeparam name="C">Result type</typeparam>
    /// <returns>Result of mapping either the left or right values with the functions provided</returns>
    public static abstract K<F, A, C> Match<A, B, C>(Func<A, C> Left, Func<B, C> Right, K<F, A, B> fab);
    
    /// <summary>
    /// Pattern-match either the left or right value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Left">Function to map the left value to a result</param>
    /// <param name="Right">Function to map the right value to a result</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <typeparam name="C">Result type</typeparam>
    /// <returns>Result of mapping either the left or right values with the functions provided</returns>
    public static virtual K<F, A, C> Match<A, B, C>(C Left, Func<B, C> Right, K<F, A, B> fab) =>
        F.Match(_ => Left, Right, fab);
    
    /// <summary>
    /// Pattern-match either the left or right value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Left">Function to map the left value to a result</param>
    /// <param name="Right">Function to map the right value to a result</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <typeparam name="C">Result type</typeparam>
    /// <returns>Result of mapping either the left or right values with the functions provided</returns>
    public static virtual K<F, A, C> Match<A, B, C>(Func<A, C> Left, C Right, K<F, A, B> fab) =>
        F.Match(Left, _ => Right, fab);
    
    /// <summary>
    /// Pattern-match the left value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Left">Function to map the left value to a result</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Either the right value or the result of mapping the left with the function provided</returns>
    public static virtual K<F, A, B> IfLeft<A, B>(Func<A, B> Left, K<F, A, B> fab) =>
        F.Match(Left, identity, fab);

    /// <summary>
    /// Pattern-match the left value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Left">Default value to use if the state is `Left`</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Either the right value or provided `Left` value</returns>
    public static virtual K<F, A, B> IfLeft<A, B>(B Left, K<F, A, B> fab) =>
        F.Match(_ => Left, identity, fab);
    
    /// <summary>
    /// Pattern-match either the left or right value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Right">Function to map the right value to a result</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Either the left value or the result of mapping the right value with the function provided</returns>
    public static virtual K<F, A, A> IfRight<A, B>(Func<B, A> Right,  K<F, A, B> fab) =>
        F.Match(identity, Right, fab);
    
    /// <summary>
    /// Pattern-match either the left or right value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Right">Default value to use if the state is `Right`</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Either the left value or provided `Right` value</returns>
    public static virtual K<F, A, A> IfRight<A, B>(A Right, K<F, A, B> fab) => 
        F.Match(identity, _ => Right, fab);
}
