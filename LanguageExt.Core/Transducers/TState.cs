#nullable enable
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LanguageExt;

public class TState : IDisposable
{
    public readonly CancellationToken Token;
    public readonly SynchronizationContext SynchronizationContext;
    ConcurrentDictionary<object, IDisposable>? Disps;

    public TState(SynchronizationContext syncContext, CancellationToken token) =>
        (Disps, Token, SynchronizationContext) = (null, token, syncContext);

    public TState(ConcurrentDictionary<object, IDisposable>? disps, SynchronizationContext syncContext, CancellationToken token) =>
        (Disps, Token, SynchronizationContext) = (disps, token, syncContext);

    public Unit Using<A>(A value, Func<A, Unit> dispose)
    {
        object? key = value;
        if (key is null) throw new InvalidCastException("can't cast the use value to object without it being null");
        var disps = Disps ?? new ConcurrentDictionary<object, IDisposable>();
        disps.TryAdd(key, new Cleaner<A>(value, dispose));
        return default;
    }

    public Unit Using<A>(A value) where A : IDisposable =>
        Using(value, x => { x.Dispose(); return default; });

    public Unit Release<A>(A value)
    {
        object? key = value;
        if (key is null) throw new InvalidCastException("can't cast the use value to object without it being null");
        if (Disps is not null && Disps.TryRemove(key, out var disp))
        {
            disp.Dispose();
        }
        return default;
    }
    
    public void Dispose()
    {
        var disps = Interlocked.Exchange(ref Disps, null);
        if(disps is not null)
        {
            foreach (var d in disps)
            {
                d.Value.Dispose();
            }
        }
    }

    record Cleaner<A>(A Value, Func<A, Unit> Free) : IDisposable
    {
        volatile int hasRun;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref hasRun, 1) == 0)
            {
                Free(Value);
            }
        }
    }
}
