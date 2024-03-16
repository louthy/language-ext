using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt;

internal static class ValueTasks
{
    [Pure]
    public static async ValueTask<bool> ForAll<A>(IEnumerable<ValueTask<A>> fs, Func<A, bool> pred, CancellationToken token = default)
    {
        var ra = await fs.WindowMap(pred, default).ConfigureAwait(false);
        return ra.AsEnumerableM().ForAll(identity);
    }

    [Pure]
    public static async ValueTask<bool> ForAll<A>(IEnumerable<ValueTask<A>> fs, Func<A, bool> pred, int windowSize, CancellationToken token = default)
    {
        var ra = await fs.WindowMap(windowSize, pred, default).ConfigureAwait(false);
        return ra.AsEnumerableM().ForAll(identity);
    }

    [Pure]
    public static async ValueTask<bool> Exists<A>(IEnumerable<ValueTask<A>> fs, Func<A, bool> pred, CancellationToken token = default)
    {
        var ra = await fs.WindowMap(pred, default).ConfigureAwait(false);
        return ra.AsEnumerableM().Exists(identity);
    }

    [Pure]
    public static async ValueTask<bool> Exists<A>(IEnumerable<ValueTask<A>> fs, Func<A, bool> pred, int windowSize, CancellationToken token = default)
    {
        var ra = await fs.WindowMap(windowSize, pred, default).ConfigureAwait(false);
        return ra.AsEnumerableM().Exists(identity);
    }    
}
