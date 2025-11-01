using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Validation
{
    /// <summary>
    /// Empty failure value
    /// </summary>
    /// <typeparam name="F">Failure type</typeparam>
    /// <typeparam name="A">Success</typeparam>
    /// <returns>Validation structure in a failed state</returns>
    [Pure]
    public static Validation<F, A> Empty<F, A>()
        where F : Monoid<F> =>
        new Validation<F, A>.Fail(F.Empty);

    /// <summary>
    /// Represents a successful operation
    /// </summary>
    /// <typeparam name="F">Error type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <param name="value">Value</param>
    /// <returns>Validation applicative</returns>
    [Pure]
    public static Validation<F, A> Success<F, A>(A value)  
        where F : Monoid<F> =>
        new Validation<F, A>.Success(value);

    /// <summary>
    /// Represents a failed operation
    /// </summary>
    /// <typeparam name="F">Error type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <param name="value">Error value</param>
    /// <returns>Validation applicative</returns>
    [Pure]
    public static Validation<F, A> Fail<F, A>(F value) 
        where F : Monoid<F> =>
        new Validation<F, A>.Fail(value);

    /// <summary>
    /// Represents a failed operation
    /// </summary>
    /// <typeparam name="F">Error type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <param name="value">Error value</param>
    /// <returns>Validation applicative</returns>
    public static Validation<F, A> Fail<F, A>(Seq<F> values)
        where F : Monoid<F> =>
        new Validation<F, A>.Fail(Monoid.combine(values));    
    
    /// <summary>
    /// Represents a successful operation
    /// </summary>
    /// <typeparam name="F">Error type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <param name="value">Value</param>
    /// <returns>Validation applicative</returns>
    [Pure]
    internal static Validation<F, A> SuccessI<F, A>(A value) => 
        new Validation<F, A>.Success(value);

    /// <summary>
    /// Represents a failed operation
    /// </summary>
    /// <typeparam name="F">Error type</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    /// <param name="value">Error value</param>
    /// <returns>Validation applicative</returns>
    [Pure]
    internal static Validation<F, A> FailI<F, A>(F value) =>
        new Validation<F, A>.Fail(value);    
}
