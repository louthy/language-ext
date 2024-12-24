using System.Collections.Generic;
using System.Threading;

namespace LanguageExt;

public static class IteratorExtensions
{
    public static Iterator<A> GetIterator<A>(this IEnumerable<A> enumerable) =>
        Iterator.from(enumerable);
}

