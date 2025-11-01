using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Co-product trait (abstract version of `Either`)
/// </summary>
/// <typeparam name="F">Self</typeparam>
public static class CoproductKExtensions
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
    public static K<F, A, C> Match<F, A, B, C>(this K<F, A, B> fab, Func<A, C> Left, Func<B, C> Right) 
        where F : CoproductK<F> =>
        F.Match(Left, Right, fab);

    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Two left and right sequences</returns>
    public static K<F, A, (Seq<A> Left, Seq<B> Right)> Partition<FF, F, A, B>(this K<FF, K<F, A, B>> fabs)
        where F : CoproductK<F>, Bimonad<F>
        where FF : Foldable<FF> =>
        CoproductK.partition(fabs);    

    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Two left and right sequences</returns>
    public static K<F, A, (Seq<A> Left, Seq<B> Right)> PartitionSequence<F, A, B>(this IEnumerable<K<F, A, B>> fabs)
        where F : CoproductK<F>, Bimonad<F> =>
        CoproductK.partitionSequence(fabs);    
}
