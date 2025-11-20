using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Co-product trait (abstract version of `Either`)
/// </summary>
/// <typeparam name="F">Self</typeparam>
public static class CoproductExtensions
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
    public static C Match<F, A, B, C>(this K<F, A, B> fab, Func<A, C> Left, Func<B, C> Right) 
        where F : Coproduct<F> =>
        F.Match(Left, Right, fab);
        
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
    public static C Match<F, A, B, C>(this K<F, A, B> fab, C Left, Func<B, C> Right) 
        where F : Coproduct<F> =>
        F.Match(Left, Right, fab);
    
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
    public static C Match<F, A, B, C>(this K<F, A, B> fab, Func<A, C> Left, C Right) 
        where F : Coproduct<F> =>
        F.Match(Left, Right, fab);
        
    /// <summary>
    /// Pattern-match the left value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Left">Function to map the left value to a result</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Either the right value or the result of mapping the left with the function provided</returns>
    public static B IfLeft<F, A, B>(this K<F, A, B> fab, Func<A, B> Left) 
        where F : Coproduct<F> =>
        F.IfLeft(Left, fab);
        
    /// <summary>
    /// Pattern-match the left value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Left">Default value to use if the state is `Left`</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Either the right value or provided `Left` value</returns>
    public static B IfLeft<F, A, B>(this K<F, A, B> fab, B Left) 
        where F : Coproduct<F> =>
        F.IfLeft(Left, fab);
    
    /// <summary>
    /// Pattern-match either the left or right value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Right">Function to map the right value to a result</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Either the left value or the result of mapping the right value with the function provided</returns>
    public static A IfRight<F, A, B>(this K<F, A, B> fab, Func<B, A> Right) 
        where F : Coproduct<F> =>
        F.IfRight(Right, fab);
    
    /// <summary>
    /// Pattern-match either the left or right value in the coproduct 
    /// </summary>
    /// <param name="fab">Coproduct value</param>
    /// <param name="Right">Default value to use if the state is `Right`</param>
    /// <typeparam name="F">Coproduct trait type</typeparam>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Either the left value or provided `Right` value</returns>
    public static A IfRight<F, A, B>(this K<F, A, B> fab, A Right) 
        where F : Coproduct<F> =>
        F.IfRight(Right, fab);

    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Two left and right sequences</returns>
    public static (Seq<A> Lefts, Seq<B> Rights) Partition<FF, F, A, B>(this K<FF, K<F, A, B>> fabs)
        where F : Coproduct<F>
        where FF : Foldable<FF> =>
        F.Partition(fabs);

    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Two left and right sequences</returns>
    public static (Seq<A> Lefts, Seq<B> Rights) PartitionSequence<F, A, B>(this IEnumerable<K<F, A, B>> fabs)
        where F : Coproduct<F> =>
        F.Partition(fabs.AsIterable());    
}
