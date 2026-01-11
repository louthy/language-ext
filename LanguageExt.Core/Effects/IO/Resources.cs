using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Holds the acquired resources for the `ResourceT` monad transformer
/// </summary>
public class Resources : IDisposable
{
    ConcurrentDictionary<object, TrackedResource> resources = new (ReferenceEqualityComparer.Instance);
    readonly Resources? parent;
    readonly object sync = new();

    public Resources(Resources? parent) =>
        this.parent = parent;

    public static IO<Resources> NewIO(Resources? parent) => 
        IO.lift(_ => new Resources(parent));
    
    public void Dispose()
    {
        var s = new CancellationTokenSource();
        var e = EnvIO.New(this, CancellationToken.None, s, SynchronizationContext.Current);
        DisposeU(e);
    }
    
    public Unit DisposeU(EnvIO envIO)
    {
        if(resources.IsEmpty) return unit;
        using var source = new CancellationTokenSource();
        var disposeEnv = new EnvIO(envIO.Resources, CancellationToken.None, source, envIO.SyncContext, null, 0);
        foreach (var item in resources)
        {
            item.Value.Release().Run(disposeEnv);
        }
        resources.Clear();
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
        resources.TryAdd(obj, new TrackedResourceDisposable<A>(value));
        return default;
    }

    public Unit AcquireAsync<A>(A value) where A : IAsyncDisposable
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        resources.TryAdd(obj, new TrackedResourceAsyncDisposable<A>(value));
        return default;
    }

    public Unit Acquire<A>(A value, Func<A, IO<Unit>> release) 
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        resources.TryAdd(obj, new TrackedResourceWithFree<A>(value, release));
        return default;
    }

    public IO<Unit> Release<A>(A value) =>
        IO.liftVAsync(async e =>
                      {
                          var obj = (object?)value;
                          if (obj is null) throw new InvalidCastException();

                          if (resources.TryRemove(obj, out var f))
                          {
                              return await f.Release().RunAsync(e);
                          }
                          else
                          {
                              if (parent is not null)
                              {
                                  return await parent.Release(value).RunAsync(e);
                              }
                          }
                          return default;
                      });
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
