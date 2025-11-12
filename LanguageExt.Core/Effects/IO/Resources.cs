using System;
using System.Threading;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Holds the acquired resources for the `ResourceT` monad transformer
/// </summary>
public class Resources : IDisposable
{
    readonly AtomHashMap<object, TrackedResource> resources = AtomHashMap<object, TrackedResource>();
    readonly Resources? parent;

    public Resources(Resources? parent) =>
        this.parent = parent;

    public static IO<Resources> NewIO(Resources? parent) => 
        IO.lift(_ => new Resources(parent));
    
    public void Dispose()
    {
        var s = new CancellationTokenSource();
        var e = EnvIO.New(this, default, s, SynchronizationContext.Current);
        DisposeU(e);
    }
    
    public Unit DisposeU(EnvIO envIO)
    {
        foreach (var item in resources)
        {
            item.Value.Release().Run(envIO);
        }
        return default;
    }

    public Unit DisposeU()
    {
        Dispose();
        return default;
    }

    public IO<Unit> DisposeIO() =>
        IO.lift(_ => DisposeU());

    public Unit Acquire<A>(A value) where A : IDisposable
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        return resources.TryAdd(obj, new TrackedResourceDisposable<A>(value));
    }

    public Unit AcquireAsync<A>(A value) where A : IAsyncDisposable
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        return resources.TryAdd(obj, new TrackedResourceAsyncDisposable<A>(value));
    }

    public Unit Acquire<A>(A value, Func<A, IO<Unit>> release) 
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        return resources.TryAdd(obj, new TrackedResourceWithFree<A>(value, release));
    }

    public IO<Unit> Release<A>(A value)
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        return resources.Find(obj)
                        .Match(Some: f =>
                                     {
                                         resources.Remove(obj);
                                         return f.Release();
                                     },
                               None: () => parent is null 
                                               ? unitIO
                                               : parent.Release(value));
    }

    public IO<Unit> ReleaseAll() =>
        IO.lift(envIO =>
                {
                    resources.Swap(
                        r =>
                        {
                            foreach (var kv in r)
                            {
                                kv.Value.Release().Run(envIO);
                            }
                            return [];
                        });
                    return unit;
                });
    
    internal Unit Merge(Resources rhs) =>
        resources.Swap(r => r.AddRange(rhs.resources.AsIterable()));
}

abstract record TrackedResource
{
    public abstract IO<Unit> Release();
}

/// <summary>
/// Holds a resource with its disposal function
/// </summary>
record TrackedResourceWithFree<A>(A Value, Func<A, IO<Unit>> Dispose) : TrackedResource
{
    public override IO<Unit> Release() => 
        Dispose(Value);
}

/// <summary>
/// Holds a resource with its disposal function
/// </summary>
record TrackedResourceDisposable<A>(A Value) : TrackedResource
    where A : IDisposable
{
    public override IO<Unit> Release() =>
        Value switch
        {
            IAsyncDisposable disposable => IO.liftAsync(async () =>
                                                        {
                                                            await disposable.DisposeAsync().ConfigureAwait(false);
                                                            return unit;
                                                        }),

            _ => IO.lift(() =>
                         {
                             Value.Dispose();
                             return unit;
                         })
        };
}

/// <summary>
/// Holds a resource with its disposal function
/// </summary>
record TrackedResourceAsyncDisposable<A>(A Value) : TrackedResource
    where A : IAsyncDisposable
{
    public override IO<Unit> Release() =>
        IO.liftAsync(async () =>
                     {
                         await Value.DisposeAsync().ConfigureAwait(false);
                         return unit;
                     });
}
