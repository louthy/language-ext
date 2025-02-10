namespace LanguageExt.Traits;

public static class Natural
{
    /// <summary>
    /// Natural transformation
    /// </summary>
    /// <remarks>
    /// If functor `map` operations transform the bound-values within the structure, then
    /// natural-transformations transform the structure itself.  
    /// </remarks>
    /// <param name="fa">Functor to transform</param>
    /// <typeparam name="F">Source functor type</typeparam>
    /// <typeparam name="G">Target functor type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Transformed functor</returns>
    public static K<G, A> transform<F, G, A>(K<F, A> fa)
        where G : Natural<F>.Transform<G> =>
        G.To(fa);
}
