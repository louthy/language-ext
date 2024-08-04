using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Construct identity monad
    /// </summary>
    public static Identity<A> Id<A>(A value) =>
        Identity<A>.Pure(value);

    /// <summary>
    /// Create a new Pure monad.  This monad doesn't do much, but when combined with
    /// other monads, it allows for easier construction of pure lifted values.
    ///
    /// There are various bind operators that make it work with these types:
    ///
    ///     * Option
    ///     * Eff
    ///     * Either
    ///     * Fin
    ///     * IO
    ///     * Validation
    ///     
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Pure monad</returns>
    public static Pure<A> Pure<A>(A value) =>
        new(value);

    /// <summary>
    /// Create a new Fail monad: the monad that always fails.  This monad doesn't do much,
    /// but when combined with other monads, it allows for easier construction of lifted 
    /// failure values.
    ///
    /// There are various bind operators that make it work with these types:
    ///
    ///     * Option
    ///     * Eff
    ///     * Either
    ///     * Fin
    ///     * IO
    ///     * Validation
    ///     
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Pure monad</returns>
    public static Fail<E> Fail<E>(E error) =>
        new(error);

    /// <summary>
    /// Construct an applicative structure from a pure value  
    /// </summary>
    /// <param name="value">Pure value to lift into the applicative structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Applicative structure</returns>
    public static K<F, A> pure<F, A>(A value)
        where F : Applicative<F> =>
        F.Pure(value);

    /// <summary>
    /// Construct a structure `F` in a failed state
    /// </summary>
    /// <param name="error">Failure value</param>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="F">Structure to construct</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`F` structure in a failed state</returns>
    public static K<F, A> fail<E, F, A>(E error)
        where F : Fallible<E, F> =>
        F.Fail<A>(error);

    /// <summary>
    /// Construct a structure `F` in a failed state
    /// </summary>
    /// <param name="error">Failure value</param>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="F">Structure to construct</typeparam>
    /// <returns>`F` structure in a failed state</returns>
    public static K<F, Unit> fail<E, F>(E error)
        where F : Fallible<E, F> =>
        F.Fail<Unit>(error);

    /// <summary>
    /// Construct a structure `F` in a failed state
    /// </summary>
    /// <param name="error">Failure value</param>
    /// <typeparam name="F">Structure to construct</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`F` structure in a failed state</returns>
    public static K<F, A> error<F, A>(Error error)
        where F : Fallible<F> =>
        F.Fail<A>(error);

    /// <summary>
    /// Construct a structure `F` in a failed state
    /// </summary>
    /// <param name="error">Failure value</param>
    /// <typeparam name="F">Structure to construct</typeparam>
    /// <returns>`F` structure in a failed state</returns>
    public static K<F, Unit> error<F>(Error error)
        where F : Fallible<F> =>
        F.Fail<Unit>(error);
}
