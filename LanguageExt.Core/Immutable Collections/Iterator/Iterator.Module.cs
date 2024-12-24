using System.Collections.Generic;
using System.Threading;

namespace LanguageExt;

public static class Iterator
{
    /// <summary>
    /// Create an iterator from an `IEnumerable`
    /// </summary>
    /// <param name="enumerable"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static Iterator<A> from<A>(IEnumerable<A> enumerable) =>
        new Iterator<A>.ConsFirst(enumerable.GetEnumerator());
}
