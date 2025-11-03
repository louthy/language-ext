using System;
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
    [Obsolete("FinSucc has been deprecated in favour of `Fin.Succ` or `Prelude.Pure`")]
    public static Fin<A> FinSucc<A>(A value) =>
        new Fin<A>.Succ(value);

    /// <summary>
    /// Fin constructor
    /// Constructs a Fin in a failure state
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="value">Failure value</param>
    /// <returns>A new Fin instance</returns>
    [Pure]
    [Obsolete("FinFail has been deprecated in favour of `Fin.Fail` or `Prelude.Fail`")]
    public static Fin<A> FinFail<A>(Error value) =>
        new Fin<A>.Fail(value);
}
