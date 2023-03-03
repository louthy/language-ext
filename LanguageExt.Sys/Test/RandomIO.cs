#nullable enable

using System;

namespace LanguageExt.Sys.Test;

/// <summary>
/// Test random access, controlled with a provided seed
/// </summary>
public sealed class RandomIO : LanguageExt.Sys.Traits.RandomIO
{
    public const int Seed = 123456789;
    
    readonly Random _rng;

    public RandomIO(int seed) => 
        _rng = new Random(seed);

    /// <summary>
    /// Creates a new seeded instance of random IO
    /// </summary>
    /// <param name="seed">seed</param>
    /// <returns>random IO</returns>
    public static RandomIO New(int seed = Seed) => new(seed);
    
    /// <summary>
    /// Returns a non-negative int
    /// </summary>
    /// <param name="min">minimum int to return</param>
    /// <param name="max">maximum int to return</param>
    /// <returns>int</returns>
    public int NextInt(int? min = default, int? max = default)
    {
        lock (_rng)
        {
            return (min, max) switch
            {
                ({ } m, { } mx) when m <= mx => _rng.Next(m, mx),
                ({ } m, { } mx) when m >= mx => _rng.Next(mx, m),
                (_, { } mx) => _rng.Next(mx),
                _ => _rng.Next()
            };
        }
    }

    /// <summary>
    /// Returns an array of bytes with random numbers
    /// </summary>
    /// <param name="length">number of bytes to fill</param>
    /// <returns>bytes</returns>
    public byte[] NextByteArray(long length)
    {
        lock (_rng)
        {
            var array = new byte[length];
            _rng.NextBytes(array);
            return array;
        }
    }

    /// <summary>
    /// Returns a non-negative double
    /// </summary>
    /// <returns>double</returns>
    public double NextDouble()
    {
        lock (_rng)
        {
            return _rng.NextDouble();
        }
    }

    /// <summary>
    /// Returns a non-negative long
    /// </summary>
    /// <returns>long</returns>
    public long NextLong()
    {
        lock (_rng)
        {
            var buf = new byte[8];
            _rng.NextBytes(buf);
            return Math.Abs(BitConverter.ToInt64(buf, 0));
        }
    }

    /// <summary>
    /// Returns a non-negative float
    /// </summary>
    /// <returns>float</returns>
    public float NextFloat()
    {
        lock (_rng)
        {
            var buffer = new byte[4];
            _rng.NextBytes(buffer);
            return Math.Abs(BitConverter.ToSingle(buffer, 0));
        }
    }

    /// <summary>
    /// Returns a non-negative guid
    /// </summary>
    /// <returns>guid</returns>
    public Guid NextGuid() =>
        new(NextByteArray(16));
}
