using System;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Holds the acquired resources for the `ResourceT` monad transformer
/// </summary>
public class Resources : IDisposable
{
    readonly AtomHashMap<object, Resource> resources = AtomHashMap<object, Resource>();

    public void Dispose()
    {
        foreach (var item in resources)
        {
            item.Value.Release().Run();
        }
    }

    public Unit Acquire<A>(A value) where A : IDisposable
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        return resources.TryAdd(obj, new ResourceDisposable<A>(value));
    }

    public Unit Acquire<A>(A value, Func<A, IO<Unit>> release) where A : class
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        return resources.TryAdd(obj, new ResourceWithFree<A>(value, release));
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
                               None: () => IO.unitIO);
    }
}

abstract record Resource
{
    public abstract IO<Unit> Release();
}

/// <summary>
/// Holds a resource with its disposal function
/// </summary>
record ResourceWithFree<A>(A Value, Func<A, IO<Unit>> Dispose) : Resource
{
    public override IO<Unit> Release() => 
        Dispose(Value);
}

/// <summary>
/// Holds a resource with its disposal function
/// </summary>
record ResourceDisposable<A>(A Value) : Resource
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
