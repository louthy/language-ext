using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Either constructor
    /// Constructs an Either in a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="value">Right value</param>
    /// <returns>A new Either instance</returns>
    [Pure]
    public static Either<L, R> Right<L, R>(R value) =>
        new Either<L, R>.Right(value);

    /// <summary>
    /// Constructs an EitherRight which can be implicitly cast to an 
    /// Either〈_, R〉
    /// </summary>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="value">Right value</param>
    /// <returns>A new EitherRight instance</returns>
    [Pure]
    public static Pure<R> Right<R>(R value) =>
        new (value);
    
    /// <summary>
    /// Either constructor
    /// Constructs an Either in a Left state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="value">Left value</param>
    /// <returns>A new Either instance</returns>
    [Pure]
    public static Either<L, R> Left<L, R>(L value) =>
        new Either<L, R>.Left(value);

    /// <summary>
    /// Constructs an EitherLeft which can be implicitly cast to an 
    /// Either〈L, _〉
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <param name="value">Right value</param>
    /// <returns>A new EitherLeft instance</returns>
    [Pure]
    public static Fail<L> Left<L>(L value) =>
        new (value);
}
