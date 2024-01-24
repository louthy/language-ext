#nullable enable
using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

/// <summary>
/// State monad type class
/// </summary>
[Trait("M*")]
public interface MonadWriter<MonoidW, W, A> : Trait
    where MonoidW  : Monoid<W>
{
    /// <summary>
    /// Tells the monad what you want it to hear.  The monad carries this 'packet'
    /// upwards, merging it if needed (hence the Monoid requirement).
    /// </summary>
    /// <typeparam name="W">Type of the value tell</typeparam>
    /// <param name="what">The value to tell</param>
    /// <returns>Updated writer monad</returns>
    [Pure]
    public static abstract Writer<MonoidW, W, Unit> Tell(W what);

    /// <summary>
    /// 'listen' is an action that executes the monad and adds
    /// its output to the value of the computation.
    /// </summary>
    [Pure]
    public static abstract Writer<MonoidW, W, (A, B)> Listen<B>(Writer<MonoidW, W, A> ma, Func<W, B> f);
}
