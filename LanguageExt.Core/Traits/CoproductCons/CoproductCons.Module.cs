namespace LanguageExt.Traits;

public static class CoproductCons
{
    /// <summary>
    /// Construct a coproduct structure in a 'Left' state
    /// </summary>
    /// <param name="value">Left value</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Constructed coproduct structure</returns>
    public static K<F, A, B> left<F, A, B>(A value)
        where F : CoproductCons<F> =>
        F.Left<A, B>(value);
    
    /// <summary>
    /// Construct a coproduct structure in a 'Left' state
    /// </summary>
    /// <param name="value">Left value</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Constructed coproduct structure</returns>
    public static K<F, A, B> right<F, A, B>(B value) 
        where F : CoproductCons<F> =>
        F.Right<A, B>(value);
}
