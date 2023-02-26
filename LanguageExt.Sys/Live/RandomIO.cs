#nullable enable

using System;

namespace LanguageExt.Sys.Live;

/// <summary>
/// Live random access
/// </summary>
public struct RandomIO : LanguageExt.Sys.Traits.RandomIO
{
    private readonly Random _rng;

    public RandomIO(Random rng) =>
        _rng = rng;

    /// <summary>
    /// Returns a non-negative int
    /// </summary>
    /// <param name="min">minimum int to return</param>
    /// <param name="max">maximum int to return</param>
    /// <returns>int</returns>
    public int NextInt(int? min = default, int? max = default) =>
        (min, max) switch
        {
            ({ } m, { } mx) when m <= mx => _rng.Next(m, mx),
            ({ } m, { } mx) when m >= mx => _rng.Next(mx,m),
            (_, { } mx) => _rng.Next(mx),
            _ => _rng.Next()
        };

    /// <summary>
    /// Returns an array of bytes with random numbers
    /// </summary>
    /// <param name="length">number of bytes to fill</param>
    /// <returns>bytes</returns>
    public byte[] NextByteArray(long length)
    {
        var array = new byte[length];
        _rng.NextBytes(array);
        return array;
    }

    /// <summary>
    /// Returns a non-negative double
    /// </summary>
    /// <returns>double</returns>
    public double NextDouble() =>
        _rng.NextDouble();

    /// <summary>
    /// Returns a non-negative long
    /// </summary>
    /// <returns>long</returns>
    public long NextLong()
    {
        var buf = new byte[8];
        _rng.NextBytes(buf);
        return Math.Abs(BitConverter.ToInt64(buf, 0));
    }

    /// <summary>
    /// Returns a non-negative float
    /// </summary>
    /// <returns>float</returns>
    public float NextFloat()
    {
        var buffer = new byte[4];
        _rng.NextBytes(buffer);
        return Math.Abs(BitConverter.ToSingle(buffer, 0));
    }

    /// <summary>
    /// Returns a non-negative guid
    /// </summary>
    /// <returns>guid</returns>
    public Guid NextGuid() =>
        Guid.NewGuid();
}
