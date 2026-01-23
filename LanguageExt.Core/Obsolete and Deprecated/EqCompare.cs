using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

class EqCompare<T> : IEqualityComparer<T>
{
    readonly Func<T, T, bool> compare;
    readonly Option<Func<T, int>> hashCode = None;

    public EqCompare(Func<T, T, bool> compare) =>
        this.compare = compare;

    public EqCompare(Func<T, T, bool> compare, Func<T, int> hashCode)
    {
        this.compare = compare;
        this.hashCode = hashCode;
    }

    [Pure]
    public bool Equals(T? x, T? y) =>
        isnull(x) && isnull(y) || (!isnull(x) && !isnull(y) && compare(x!, y!));

    [Pure]
    public int GetHashCode(T obj) =>
        hashCode.Match(
            f => isnull(obj) ? 0 : f(obj),
            () => 0);
}
