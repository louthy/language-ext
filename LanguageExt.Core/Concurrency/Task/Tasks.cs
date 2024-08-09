using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt;

internal static class Tasks
{
    [Pure]
    public static async Task<bool> ForAll<A>(IEnumerable<Task<A>> fs, Func<A, bool> pred, CancellationToken token = default)
    {
        var ra = await fs.WindowMap(pred, default).ConfigureAwait(false);
        return ra.AsIterable().ForAll(identity);
    }

    [Pure]
    public static async Task<bool> ForAll<A>(IEnumerable<Task<A>> fs, Func<A, bool> pred, int windowSize, CancellationToken token = default)
    {
        var ra = await fs.WindowMap(windowSize, pred, default).ConfigureAwait(false);
        return ra.AsIterable().ForAll(identity);
    }

    [Pure]
    public static async Task<bool> Exists<A>(IEnumerable<Task<A>> fs, Func<A, bool> pred, CancellationToken token = default)
    {
        var ra = await fs.WindowMap(pred, default).ConfigureAwait(false);
        return ra.AsIterable().Exists(identity);
    }

    [Pure]
    public static async Task<bool> Exists<A>(IEnumerable<Task<A>> fs, Func<A, bool> pred, int windowSize, CancellationToken token = default)
    {
        var ra = await fs.WindowMap(windowSize, pred, default).ConfigureAwait(false);
        return ra.AsIterable().Exists(identity);
    }    
}
