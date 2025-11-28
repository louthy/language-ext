using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

public static class IteratorAsyncExtensions
{
    /// <summary>
    /// Downcast operator
    /// </summary>
    public static IteratorAsync<A> As<A>(this K<IteratorAsync, A> ma) =>
        (IteratorAsync<A>)ma;
    
    /// <summary>
    /// Get an iterator for any `IAsyncEnumerable` 
    /// </summary>
    public static IteratorAsync<A> GetIteratorAsync<A>(this IAsyncEnumerable<A> enumerable) =>
        IteratorAsync.from(enumerable);

}

