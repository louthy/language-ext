using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Co-product trait (abstract version of `Either`)
/// </summary>
/// <typeparam name="F">Self</typeparam>
public interface Coproduct<F> : CoproductCons<F>
    where F : Coproduct<F>
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
    public static abstract C Match<A, B, C>(Func<A, C> Left, Func<B, C> Right, K<F, A, B> fab);
    
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
    public static virtual C Match<A, B, C>(C Left, Func<B, C> Right, K<F, A, B> fab) =>
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
    public static virtual C Match<A, B, C>(Func<A, C> Left, C Right, K<F, A, B> fab) =>
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
    public static virtual B IfLeft<A, B>(Func<A, B> Left, K<F, A, B> fab) =>
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
    public static virtual B IfLeft<A, B>(B Left, K<F, A, B> fab) =>
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
    public static virtual A IfRight<A, B>(Func<B, A> Right,  K<F, A, B> fab) =>
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
    public static virtual A IfRight<A, B>(A Right, K<F, A, B> fab) => 
        F.Match(identity, _ => Right, fab);    
    
    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Two left and right sequences</returns>
    public static virtual (Seq<A> Lefts, Seq<B> Rights) Partition<FF, A, B>(K<FF, K<F, A, B>> fabs)
        where FF : Foldable<FF> =>
        fabs.Fold((Lefts: Seq<A>(), Rights: Seq<B>()), 
                  (s, fab) => fab.Match(Left: l => s with { Lefts = s.Lefts.Add(l) },
                                        Right: r => s with { Rights = s.Rights.Add(r) }));
    
    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences, then return the left sequence.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Left sequence</returns>
    public static virtual Seq<A> Lefts<G, A, B>(K<G, K<F, A, B>> fabs)
        where G : Foldable<G> =>
        fabs.Fold(Seq<A>(), (s, fab) => fab.Match(Left: s.Add, Right: s));
    
    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences, then return the right sequence.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Right sequence</returns>
    public static virtual Seq<B> Rights<G, A, B>(K<G, K<F, A, B>> fabs)
        where G : Foldable<G> =>
        fabs.Fold(Seq<B>(), (s, fab) => fab.Match(Left: s, Right: s.Add));
}
