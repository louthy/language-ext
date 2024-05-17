using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Hosts a standard .NET `IComparer` from an `Ord<A>` instance (in the static `Default` property)
/// </summary>
public class OrdComparer<OrdA, A> : IComparer<A> where OrdA : Ord<A>
{
    public static readonly IComparer<A> Default = new OrdComparer<OrdA, A>(); 

    public int Compare(A? x, A? y) =>
        (x, y) switch
        {
            (null, null) => 0,
            (null, _)    => -1,
            (_, null)    => 1,
            var (x1, y1) => OrdA.Compare(x1, y1)
        };
}

internal class OrdComparer<A>(Func<A, A, int> Comparer) : IComparer<A>
{
    public int Compare(A? x, A? y) =>
        (x, y) switch
        {
            (null, null) => 0,
            (null, _)    => -1,
            (_, null)    => 1,
            var (x1, y1) => Comparer(x1, y1)
        };
}
