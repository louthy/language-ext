using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

public static class IteratorExtensions
{
    /// <summary>
    /// Downcast operator
    /// </summary>
    public static Iterator<A> As<A>(this K<Iterator, A> ma) =>
        (Iterator<A>)ma;
    
    /// <summary>
    /// Get an iterator for any `IEnumerable` 
    /// </summary>
    public static Iterator<A> GetIterator<A>(this IEnumerable<A> enumerable) =>
        Iterator.from(enumerable);

}

