using System;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Gets all maintained values for a domain set.
    /// </summary>
    public static Seq<A> get<A>()
        where A : Maintainer<A> =>
        A.All;

    /// <summary>
    /// Finds the first maintained value that satisfies the predicate.
    /// </summary>
    public static Option<A> findM<A>(Func<A, bool> fa)
        where A : Maintainer<A> =>
        get<A>().Find(fa);

    /// <summary>
    /// Finds the first maintained value that satisfies the predicate.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no maintained value satisfies the predicate.
    /// </exception>
    public static A find<A>(Func<A, bool> fa)
        where A : Maintainer<A> =>
        findM(fa).Case switch
        {
            A a => a,
            _ => throw new InvalidOperationException("Option was None")
        };
}
