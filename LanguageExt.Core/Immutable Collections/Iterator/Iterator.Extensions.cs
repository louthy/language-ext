using System.Collections.Generic;

namespace LanguageExt;

public static class IteratorExtensions
{
    /// <summary>
    /// Get an iterator for any `IEnumerable` 
    /// </summary>
    public static Iterator<A> GetIterator<A>(this IEnumerable<A> enumerable) =>
        Iterator.from(enumerable);

    /// <summary>
    /// Create an `IEnumerable` from an `Iterator`
    /// </summary>
    public static IEnumerable<A> AsEnumerable<A>(this Iterator<A> self)
    {
        while (!self.IsEmpty)
        {
            yield return self.Head;
            self = self.Tail;
        }
    }

    /// <summary>
    /// Create an `Iterable` from an `Iterator`
    /// </summary>
    public static Iterable<A> AsIterable<A>(this Iterator<A> self) =>
        Iterable.createRange(self.AsEnumerable());
}

