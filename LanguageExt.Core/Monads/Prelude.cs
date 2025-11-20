using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Construct identity monad
    /// </summary>
    public static Identity<A> Id<A>(A value) =>
        Identity.Pure(value);

    /// <summary>
    /// Construct identity monad
    /// </summary>
    public static IdentityT<M, A> Id<M, A>(A value) 
        where M : Monad<M>, Choice<M> =>
        IdentityT.Pure<M, A>(value);

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
}
