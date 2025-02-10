namespace LanguageExt.Traits;

/// <summary>
/// Natural transformation source functor
/// </summary>
/// <typeparam name="F">From functor</typeparam>
public interface Natural<F>
{
    /// <summary>
    /// Natural transformation target functor
    /// </summary>
    /// <typeparam name="G">To functor</typeparam>
    public interface Transform<G>
    {
        /// <summary>
        /// Perform a natural transformation from `F A -> G A`
        /// </summary>
        /// <param name="fa">Functor to transform</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Transformed functor</returns>
        public static abstract K<G, A> To<A>(K<F, A> fa);
    }
}
