namespace LanguageExt.Traits;

/// <summary>
/// Natural transformation 
/// </summary>
/// <typeparam name="F">From functor</typeparam>
/// <typeparam name="G">To functor</typeparam>
public interface Natural<out F, in G>
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
    public static abstract K<G, A> Transform<A>(K<F, A> fa);
}
