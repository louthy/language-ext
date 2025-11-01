using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Co-product trait (abstract version of `Either`)
/// </summary>
/// <typeparam name="F">Self</typeparam>
public interface CoproductCons<in F>
    where F : CoproductCons<F>
{
    /// <summary>
    /// Construct a coproduct structure in a 'Left' state
    /// </summary>
    /// <param name="value">Left value</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Constructed coproduct structure</returns>
    public static abstract K<F, A, B> Left<A, B>(A value);
    
    /// <summary>
    /// Construct a coproduct structure in a 'Left' state
    /// </summary>
    /// <param name="value">Left value</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Constructed coproduct structure</returns>
    public static abstract K<F, A, B> Right<A, B>(B value);
}
