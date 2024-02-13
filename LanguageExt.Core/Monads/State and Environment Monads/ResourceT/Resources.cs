using System;
using LanguageExt.HKT;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Holds the acquired resources for the `ResourceT` monad transformer
/// </summary>
public class Resources : IDisposable
{
    readonly AtomHashMap<object, IDisposable> resources = AtomHashMap<object, IDisposable>();

    public void Dispose()
    {
        foreach (var item in resources)
        {
            item.Value.Dispose();
        }
    }

    public Unit Acquire<A>(A value) where A : IDisposable
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        return resources.TryAdd(obj, value);
    }

    public Unit Acquire<A>(A value, Action<A> release) where A : class
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        return resources.TryAdd(obj, new ResourceFree<A>(value, release));
    }

    public Unit Release<A>(A value)
    {
        var obj = (object?)value;
        if (obj is null) throw new InvalidCastException();
        return resources.Find(obj)
                        .Iter(f =>
                              {
                                  f.Dispose();
                                  resources.Remove(obj);
                              });
    }
}

public record ResourceFree<A>(A Value, Action<A> Release) : IDisposable
{
    public void Dispose() =>
        Release(Value);
}
