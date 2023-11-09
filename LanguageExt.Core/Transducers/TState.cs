#nullable enable
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LanguageExt;

public class TState : IDisposable
{
    public readonly CancellationToken Token;
    ConcurrentDictionary<object, IDisposable>? Disps;

    public TState() =>
        Disps = null;

    public TState(ConcurrentDictionary<object, IDisposable>? disps, CancellationToken token) =>
        (Disps, Token) = (disps, token);
    
    public void Dispose()
    {
        var disps = Disps;
        Disps = null;
        if (disps is null) return;
        foreach (var d in disps)
        {
            d.Value.Dispose();
        }
    }
}
