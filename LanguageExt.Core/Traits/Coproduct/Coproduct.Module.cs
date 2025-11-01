using System.Collections.Generic;

namespace LanguageExt.Traits;

public static class Coproduct
{
    /// <summary>
    /// Construct a coproduct structure in a 'Left' state
    /// </summary>
    /// <param name="value">Left value</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Constructed coproduct structure</returns>
    public static K<F, A, B> left<F, A, B>(A value)
        where F : Coproduct<F> =>
        F.Left<A, B>(value);
    
    /// <summary>
    /// Construct a coproduct structure in a 'Left' state
    /// </summary>
    /// <param name="value">Left value</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Constructed coproduct structure</returns>
    public static K<F, A, B> right<F, A, B>(B value) 
        where F : Coproduct<F> =>
        F.Right<A, B>(value);

    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Two left and right sequences</returns>
    public static (Seq<A> Lefts, Seq<B> Rights) partition<FF, F, A, B>(K<FF, K<F, A, B>> fabs)
        where F : Coproduct<F>
        where FF : Foldable<FF> =>
        F.Partition(fabs);

    /// <summary>
    /// Partition the foldable of coproducts into two left and right sequences.
    /// </summary>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Two left and right sequences</returns>
    public static (Seq<A> Lefts, Seq<B> Rights) partitionSequence<F, A, B>(IEnumerable<K<F, A, B>> fabs)
        where F : Coproduct<F> =>
        F.Partition(fabs.AsIterable());     
}
