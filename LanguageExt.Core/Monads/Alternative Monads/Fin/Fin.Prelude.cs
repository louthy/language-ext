using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Fin constructor
    /// Constructs a Fin in a success state
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="value">Success value</param>
    /// <returns>A new Fin instance</returns>
    [Pure]
    public static Fin<A> FinSucc<A>(A value) =>
        Fin<A>.Succ(value);

    /// <summary>
    /// Fin constructor
    /// Constructs a Fin in a failure state
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="value">Failure value</param>
    /// <returns>A new Fin instance</returns>
    [Pure]
    public static Fin<A> FinFail<A>(Error value) =>
        Fin<A>.Fail(value);
}
