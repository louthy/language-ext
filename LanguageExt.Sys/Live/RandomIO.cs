#nullable enable

using System;
using System.Threading;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live;

/// <summary>
/// Live random access
/// </summary>
public struct RandomIO : LanguageExt.Sys.Traits.RandomIO
{
    static readonly ThreadLocal<Random> SharedRandom = 
        new(() => new Random());

    static readonly Func<int, ThreadLocal<Random>> SeededSharedRandom =
        memoUnsafe((int seed) => new ThreadLocal<Random>(() => new Random(seed)));

    readonly int? _seed;

    RandomIO(int? seed) =>
        _seed = seed;

    /// <summary>
    /// Creates a new seeded instance of random IO
    /// </summary>
    /// <param name="seed">seed</param>
    /// <returns>random IO</returns>
    public static RandomIO New(int? seed = default) => new(seed);

    static Random Instance(int? seed) =>
        seed.HasValue
            ? SeededSharedRandom(seed.Value).Value
            : SharedRandom.Value;
    
    /// <summary>
    /// Returns a non-negative int
    /// </summary>
    /// <param name="min">minimum int to return</param>
    /// <param name="max">maximum int to return</param>
    /// <returns>int</returns>
    public int NextInt(int? min = default, int? max = default)
    {
        var rng = Instance(_seed);
        return (min, max) switch
        {
            ({ } m, { } mx) when m <= mx => rng.Next(m, mx),
            ({ } m, { } mx) when m >= mx => rng.Next(mx, m),
            (_, { } mx) => rng.Next(mx),
            _ => rng.Next()
        };
    }

    /// <summary>
    /// Returns an array of bytes with random numbers
    /// </summary>
    /// <param name="length">number of bytes to fill</param>
    /// <returns>bytes</returns>
    public byte[] NextByteArray(long length)
    {
        var rng = Instance(_seed);
        var array = new byte[length];
        rng.NextBytes(array);
        return array;
    }

    /// <summary>
    /// Returns a non-negative double
    /// </summary>
    /// <returns>double</returns>
    public double NextDouble()
    {
        var rng = Instance(_seed);
        return rng.NextDouble();
    }

    /// <summary>
    /// Returns a non-negative long
    /// </summary>
    /// <returns>long</returns>
    public long NextLong()
    {
        var rng = Instance(_seed);
        var buf = new byte[8];
        rng.NextBytes(buf);
        return Math.Abs(BitConverter.ToInt64(buf, 0));
    }

    /// <summary>
    /// Returns a non-negative float
    /// </summary>
    /// <returns>float</returns>
    public float NextFloat()
    {
        var rng = Instance(_seed);
        var buffer = new byte[4];
        rng.NextBytes(buffer);
        return Math.Abs(BitConverter.ToSingle(buffer, 0));
    }

    /// <summary>
    /// Returns a non-negative guid
    /// </summary>
    /// <returns>guid</returns>
    public Guid NextGuid() =>
        Guid.NewGuid();
}
