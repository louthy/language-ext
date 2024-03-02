using System;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Holds the acquired resources for the `ResourceT` monad transformer
/// </summary>
public class Resources : IDisposable
{
    readonly AtomHashMap<object, TrackedResource> resources = AtomHashMap<object, TrackedResource>();

    public void Dispose()
    {
        foreach (var item in resources)
        {
            item.Value.Release().Run();
        }
    }

    public Unit DisposeU()
    {
        Dispose();
        return default;
    }

    public Unit Acquire<A>(A value) where A : IDisposable
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        return resources.TryAdd(obj, new TrackedResourceDisposable<A>(value));
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
                               None: () => unitIO);
    }
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
        IO<Unit>.Lift(
            () =>
            {
                Value.Dispose();
                return unit;
            });
}
