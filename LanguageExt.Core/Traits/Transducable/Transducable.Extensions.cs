namespace LanguageExt.Traits;

public static class TransducableExtensions
{
    /// <summary>
    /// Transform an `F` structure with a transducer
    /// </summary>
    /// <param name="ma">Structure to transform</param>
    /// <param name="tb">Transducer to transform wih</param>
    /// <typeparam name="F">Structure to transform</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Transformed structure</returns>
    public static K<F, B> Transform<F, A, B>(this K<F, A> ma, K<Transducer<A>, B> tb) 
        where F : Transducable<F> =>
        F.Transform(ma, tb);

    /// <summary>
    /// Run a reducer on the values in the `F` structure
    /// </summary>
    /// <param name="ma">Structure to reduce</param>
    /// <param name="initial">Initial state</param>
    /// <param name="reducer">Reducer function</param>
    /// <typeparam name="F">Structure to reduce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="S">State value type to reduce to</typeparam>
    /// <returns>Reduced value</returns>
    public static S Reduce<F, A, S>(this K<F, A> ma, S initial, ReducerAsync<A, S> reducer)
        where F : Transducable<F> =>
        F.Reduce(ma, initial, reducer);
}
