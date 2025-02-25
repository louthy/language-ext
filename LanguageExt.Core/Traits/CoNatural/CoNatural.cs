namespace LanguageExt.Traits;

/// <summary>
/// Natural transformation 
/// </summary>
/// <remarks>
/// If functor `map` operations transform the bound-values within the structure, then
/// natural-transformations transform the structure itself.
/// </remarks>
/// <remarks>
/// Functors are referenced, because that's the true definition in category-theory, but
/// there is no requirement in language-ext for FA or GA to be functors.  It is just typically
/// true that FA and GA will also be functors.
/// </remarks>
/// <typeparam name="F">From functor</typeparam>
/// <typeparam name="G">To functor</typeparam>
public interface CoNatural<in F, out G>
{
    /// <summary>
    /// Perform a natural transformation from `FA -> GA`
    /// </summary>
    /// <remarks>
    /// If functor `map` operations transform the bound-values within the structure, then
    /// natural-transformations transform the structure itself.
    /// </remarks>
    /// <remarks>
    /// Functors are referenced, because that's the true definition in category-theory, but
    /// there is no requirement in language-ext for FA or GA to be functors.  It is just typically
    /// true that FA and GA will also be functors.
    /// </remarks>
    /// <param name="fa">Functor to transform</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Transformed functor</returns>
    public static abstract K<F, A> CoTransform<A>(K<G, A> fa);
}
