using static LanguageExt.Prelude;
using System.Collections.Generic;

namespace LanguageExt.Traits;

public static class CoproductK
{
    /// <summary>
    /// Construct a coproduct structure in a 'Left' state
    /// </summary>
    /// <param name="value">Left value</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Constructed coproduct structure</returns>
    public static K<F, A, B> left<F, A, B>(A value)
        where F : CoproductK<F> =>
        F.Left<A, B>(value);
    
    /// <summary>
    /// Construct a coproduct structure in a 'Left' state
    /// </summary>
    /// <param name="value">Left value</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Constructed coproduct structure</returns>
    public static K<F, A, B> right<F, A, B>(B value) 
        where F : CoproductK<F> =>
        F.Right<A, B>(value);

    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Two left and right sequences</returns>
    public static K<F, A, (Seq<A> Left, Seq<B> Right)> partition<FF, F, A, B>(K<FF, K<F, A, B>> fabs)
        where F : CoproductK<F>, Bimonad<F>
        where FF : Foldable<FF> =>
        fabs.Fold(F.Right<A, (Seq<A> Left, Seq<B> Right)>((Left: Seq<A>(), Right: Seq<B>())),
                  (fs, fab) =>
                      fs.BindSecond(s => fab.Match(l => s with { Left = s.Left.Add(l) },
                                                   r => s with { Right = s.Right.Add(r) })));

    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Two left and right sequences</returns>
    public static K<F, A, (Seq<A> Left, Seq<B> Right)> partitionSequence<F, A, B>(IEnumerable<K<F, A, B>> fabs)
        where F : CoproductK<F>, Bimonad<F> =>
        partition(fabs.AsIterable());       
}
