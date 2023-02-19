#nullable enable

using System;
using LanguageExt.Attributes;

namespace LanguageExt.Sys.Traits;

public interface RandomIO
{
    /// <summary>
    /// Returns a non-negative int
    /// </summary>
    /// <param name="min">minimum int to return</param>
    /// <param name="max">maximum int to return</param>
    /// <returns>int</returns>
    int GenerateInt(int? min = default, int? max = default);

    /// <summary>
    /// Fills the elements of a specified array of bytes with random numbers
    /// </summary>
    /// <param name="length">number of bytes to fill</param>
    /// <returns>bytes</returns>
    byte[] GenerateByteArray(long length);

    /// <summary>
    /// Returns a non-negative double
    /// </summary>
    /// <returns>double</returns>
    double GenerateDouble();

    /// <summary>
    /// Returns a non-negative long
    /// </summary>
    /// <returns>long</returns>
    long GenerateLong();

    /// <summary>
    /// Returns a non-negative float
    /// </summary>
    /// <returns>float</returns>
    float GenerateFloat();

    /// <summary>
    /// Returns a non-negative guid
    /// </summary>
    /// <returns>guid</returns>
    Guid GenerateGuid();
}

/// <summary>
/// Type-class giving a struct the trait of supporting Random IO
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
[Typeclass("*")]
public interface HasRandom<RT> where RT : struct
{
    /// <summary>
    /// Access the random synchronous effect environment
    /// </summary>
    /// <returns>Random synchronous effect environment</returns>
    Eff<RT, RandomIO> RandomEff { get; }
}
