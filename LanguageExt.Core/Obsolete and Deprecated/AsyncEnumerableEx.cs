using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.LinqExtensionInternal;

public static partial class AsyncEnumerableEx
{
    [Obsolete("Use Flatten instead.")]
    public static IAsyncEnumerable<TSource> Merge<TSource>(params IAsyncEnumerable<TSource>[] sources) =>
        Flatten(sources);

    [Obsolete("Use Flatten instead.")]
    public static IAsyncEnumerable<TSource> Merge<TSource>(
        this IAsyncEnumerable<IAsyncEnumerable<TSource>> sources) =>
        sources.SelectMany(source => source);
}
