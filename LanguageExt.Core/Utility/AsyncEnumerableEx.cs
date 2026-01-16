using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.LinqExtensionInternal;

public static partial class AsyncEnumerableEx
{
    /// <summary>
    /// Merges elements from all the specified async-enumerable sequences into a single async-enumerable sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
    /// <param name="sources">Async-enumerable sequences.</param>
    /// <returns>The async-enumerable sequence that merges the elements of the async-enumerable sequences.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="sources"/> is null.</exception>
    public static IAsyncEnumerable<TSource> Flatten<TSource>(params IAsyncEnumerable<TSource>[] sources)
    {
        return Core(sources);

        static async IAsyncEnumerable<TSource> Core(IAsyncEnumerable<TSource>[] sources,
                                                    [System.Runtime.CompilerServices.EnumeratorCancellation]
                                                    CancellationToken cancellationToken = default)
        {
            var count = sources.Length;

            var enumerators   = new IAsyncEnumerator<TSource>[count];
            var moveNextTasks = new ValueTask<bool>[count];

            try
            {
                for (var i = 0; i < count; i++)
                {
                    var enumerator = sources[i].GetAsyncEnumerator(cancellationToken);
                    enumerators[i] = enumerator;
                    moveNextTasks[i] = enumerator.MoveNextAsync();
                }

                var whenAny = TaskExt.WhenAny(moveNextTasks);

                int active = count;

                while (active > 0)
                {
                    int index = await whenAny;

                    var enumerator   = enumerators[index];
                    var moveNextTask = moveNextTasks[index];

                    if (!await moveNextTask.ConfigureAwait(false))
                    {
                        moveNextTasks[index] = new ValueTask<bool>();
                        enumerators[index] = null!;
                        await enumerator.DisposeAsync().ConfigureAwait(false);

                        active--;
                    }
                    else
                    {
                        TSource item = enumerator.Current;

                        whenAny.Replace(index, enumerator.MoveNextAsync());

                        yield return item;
                    }
                }
            }
            finally
            {

                var errors = default(List<Exception>);

                for (var i = count - 1; i >= 0; i--)
                {
                    var moveNextTask = moveNextTasks[i];
                    var enumerator   = enumerators[i];

                    try
                    {
                        try
                        {
                            _ = await moveNextTask.ConfigureAwait(false);
                        }
                        finally
                        {
                            if (enumerator != null!)
                            {
                                await enumerator.DisposeAsync().ConfigureAwait(false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (errors == null)
                        {
                            errors = new List<Exception>();
                        }

                        errors.Add(ex);
                    }
                }

                if (errors != null)
                {
                    throw new AggregateException(errors);
                }
            }
        }

    }

    /// <summary>
    /// Merges elements from all inner async-enumerable sequences into a single async-enumerable sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequences.</typeparam>
    /// <param name="sources">Async-enumerable sequence of inner async-enumerable sequences.</param>
    /// <returns>The async-enumerable sequence that merges the elements of the inner sequences.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="sources"/> is null.</exception>
    public static IAsyncEnumerable<TSource> Flatten<TSource>(
        this IAsyncEnumerable<IAsyncEnumerable<TSource>> sources) =>
        sources.SelectMany(source => source);    
}
