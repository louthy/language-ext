#nullable enable
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LanguageExt;

public class TState(
    ConcurrentDictionary<object, IDisposable>? disps,
    SynchronizationContext? syncContext,
    CancellationToken token)
    : IDisposable
{
    public readonly CancellationToken Token = token;
    public readonly SynchronizationContext? SynchronizationContext = syncContext;

    public TState(SynchronizationContext? syncContext, CancellationToken token) : this(null, syncContext, token)
    {
    }

    public Unit Using<A>(A value, Func<A, Unit> dispose)
    {
        object? key = value;
        if (key is null) throw new InvalidCastException("can't cast the use value to object without it being null");
        var disps1 = disps ?? new ConcurrentDictionary<object, IDisposable>();
        disps1.TryAdd(key, new Cleaner<A>(value, dispose));
        return default;
    }

    public Unit Using<A>(A value) where A : IDisposable =>
        Using(value, x => { x.Dispose(); return default; });

    public Unit Release<A>(A value)
    {
        object? key = value;
        if (key is null) throw new InvalidCastException("can't cast the use value to object without it being null");
        if (disps is not null && disps.TryRemove(key, out var disp))
        {
            disp.Dispose();
        }
        return default;
    }
    
    public void Dispose()
    {
        var disps1 = Interlocked.Exchange(ref disps, null);
        if(disps1 is not null)
        {
            foreach (var d in disps1)
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
