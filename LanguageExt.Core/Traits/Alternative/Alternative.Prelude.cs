using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> oneOf<F, A>(params K<F, A>[] ms)
        where F : Alternative<F> =>
        Alternative.oneOf(ms);

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> oneOf<F, A>(Seq<K<F, A>> ms)
        where F : Alternative<F> =>
        Alternative.oneOf(ms);
}
