using System.Diagnostics.Contracts;

namespace LanguageExt;

public partial class Either
{
    /// <summary>
    /// Construct an `Either` value in a `Right` state.
    /// </summary>
    /// <remarks>
    /// `Right` is often synonymous with 'correct', or 'success', although that isn't a requirement for any reason other
    /// than the default monad bind behaviour.  `Left` shortcuts and 'fails', whereas `Right` means we can successfully
    /// continue.
    /// </remarks>
    /// <param name="value">Value to construct the `Right` state with</param>
    /// <typeparam name="L">Left value type</typeparam>
    /// <typeparam name="R">Right value type</typeparam>
    /// <returns>Constructed `Either` value</returns>
    [Pure]
    public static Either<L, R> Right<L, R>(R value) =>
        new Either<L, R>.Right(value);

    /// <summary>
    /// Construct an `Either` value in a `Left` state.
    /// </summary>
    /// <remarks>
    /// `Left` is often synonymous with 'failure', or 'error', although that isn't a requirement for any reason other
    /// than the default monad bind behaviour.  `Left` shortcuts and 'fails', whereas `Right` means we can successfully
    /// continue.
    /// </remarks>
    /// <param name="value">Value to construct the `Right` state with</param>
    /// <typeparam name="L">Left value type</typeparam>
    /// <typeparam name="R">Right value type</typeparam>
    /// <returns>Constructed `Either` value</returns>
    [Pure]
    public static Either<L, R> Left<L, R>(L value) =>
        new Either<L, R>.Left(value);
}
