namespace LanguageExt.Traits;

public interface Transducable<F> 
    where F : Transducable<F>
{
    /// <summary>
    /// Transform an `F` structure with a transducer
    /// </summary>
    /// <param name="ma">Structure to transform</param>
    /// <param name="tb">Transducer to transform wih</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Transformed structure</returns>
    public static abstract K<F, B> Transform<A, B>(K<F, A> ma, K<Transducer<A>, B> tb);

    /// <summary>
    /// Run a reducer on the values in the `F` structure
    /// </summary>
    /// <param name="ma">Structure to reduce</param>
    /// <param name="initial">Initial state</param>
    /// <param name="reducer">Reducer function</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="S">State value type to reduce to</typeparam>
    /// <returns>Reduced value</returns>
    public static abstract S Reduce<S, A>(K<F, A> ma, S initial, Reducer<A, S> reducer);
}
