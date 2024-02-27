
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Represents a successful operation
    /// </summary>
    /// <typeparam name="F">Error type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <param name="value">Value</param>
    /// <returns>Validation applicative</returns>
    public static Validation<F, A> Success<F, A>(A value) 
        where F : Monoid<F> =>
        Validation<F, A>.Success(value);

    /// <summary>
    /// Represents a failed operation
    /// </summary>
    /// <typeparam name="F">Error type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <param name="value">Error value</param>
    /// <returns>Validation applicative</returns>
    public static Validation<F, A> Fail<F, A>(F value) 
        where F : Monoid<F> =>
        Validation<F, A>.Fail(value);

    /// <summary>
    /// Represents a failed operation
    /// </summary>
    /// <typeparam name="F">Error type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <param name="value">Error value</param>
    /// <returns>Validation applicative</returns>
    public static Validation<F, A> Fail<F, A>(Seq<F> values)
        where F : Monoid<F> =>
        Validation<F, A>.Fail(Monoid.concat(values));
}
