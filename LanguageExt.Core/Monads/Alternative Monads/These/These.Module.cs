using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class These
{
    /// <summary>
    /// Coalesce with the provided operation
    /// </summary>
    /// <param name="f">Coalesce operation</param>
    /// <returns>Coalesced value</returns>
    public static A merge<A>(Func<A, A, A> f, These<A, A> these) =>
        these.Merge(f);

    /// <summary>
    /// Select each constructor and partition them into separate lists.
    /// </summary>
    /// <param name="theses">Selection</param>
    /// <typeparam name="F">Foldable structure</typeparam>
    /// <returns>Partitioned sequences</returns>
    public static (Seq<A> This, Seq<B> That, Seq<(A, B)> Both) partition<F, A, B>(
        K<F, These<A, B>> theses)
        where F : Foldable<F> =>
        theses.Partition();
    
    /// <summary>
    /// Select each constructor and partition them into separate lists.
    /// </summary>
    /// <param name="theses">Selection</param>
    /// <typeparam name="F">Foldable structure</typeparam>
    /// <returns>Partitioned sequences</returns>
    public static (Seq<A> This, Seq<B> That) partition2<F, A, B>(
        K<F, These<A, B>> theses)
        where F : Foldable<F> =>
        theses.Partition2();
}
