﻿#nullable enable

using System;

namespace LanguageExt.Sys.Test;

/// <summary>
/// Test random access, controlled with a provided seed
/// </summary>
public sealed class RandomIO : LanguageExt.Sys.Traits.RandomIO
{
    readonly Random _rng;

    public RandomIO(int seed) => 
        _rng = new Random(seed);

    /// <summary>
    /// Returns a non-negative int
    /// </summary>
    /// <param name="min">minimum int to return</param>
    /// <param name="max">maximum int to return</param>
    /// <returns>int</returns>
    public int GenerateInt(int? min = default, int? max = default) =>
        (min, max) switch
        {
            ({ } m, { } mx) => _rng.Next(m, mx),
            (_, { } mx) => _rng.Next(mx),
            _ => _rng.Next()
        };

    /// <summary>
    /// Fills the elements of a specified array of bytes with random numbers
    /// </summary>
    /// <param name="length">number of bytes to fill</param>
    /// <returns>bytes</returns>
    public byte[] GenerateByteArray(long length)
    {
        var array = new byte[length];
        _rng.NextBytes(array);
        return array;
    }

    /// <summary>
    /// Returns a non-negative double
    /// </summary>
    /// <returns>double</returns>
    public double GenerateDouble() => 
        _rng.NextDouble();

    /// <summary>
    /// Returns a non-negative long
    /// </summary>
    /// <returns>long</returns>
    public long GenerateLong()
    {
        var buf = new byte[8];
        _rng.NextBytes(buf);
        return Math.Abs(BitConverter.ToInt64(buf, 0));
    }

    /// <summary>
    /// Returns a non-negative float
    /// </summary>
    /// <returns>float</returns>
    public float GenerateFloat()
    {
        var buffer = new byte[4];
        _rng.NextBytes(buffer);
        return Math.Abs(BitConverter.ToSingle(buffer, 0));
    }

    /// <summary>
    /// Returns a non-negative guid
    /// </summary>
    /// <returns>guid</returns>
    public Guid GenerateGuid() =>
        new(GenerateByteArray(16));
}
