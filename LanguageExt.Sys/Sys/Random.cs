#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys;

/// <summary>
/// Random IO
/// </summary>
/// <typeparam name="RT">runtime</typeparam>
public static class Random<RT> where RT : struct, HasRandom<RT>
{
    /// <summary>
    /// Returns a non-negative int
    /// </summary>
    /// <param name="min">minimum int to return</param>
    /// <param name="max">maximum int to return</param>
    /// <returns>int</returns>
    [Pure]
    public static Eff<RT, int> generateInt(int? min = default, int? max = default) =>
        default(RT).RandomEff.Map(io => io.GenerateInt(min, max));

    /// <summary>
    /// Fills the elements of a specified array of bytes with random numbers
    /// </summary>
    /// <param name="length">number of bytes to fill</param>
    [Pure]
    public static Eff<RT, byte[]> generateByteArray(long length) =>
        default(RT).RandomEff.Map(io => io.GenerateByteArray(length));

    /// <summary>
    /// Returns a non-negative double
    /// </summary>
    /// <param name="min">minimum double to return</param>
    /// <param name="max">maximum double to return</param>
    /// <returns>double</returns>
    [Pure]
    public static Eff<RT, double> generateDouble(double? min = default, double? max = default)
    {
        var minV = min ?? 0.0;
        var maxV = max ?? 1.0;
        return default(RT).RandomEff.Map(static io => io.GenerateDouble()).Map(d => d % (maxV - minV) + minV);
    }

    /// <summary>
    /// Returns a non-negative long
    /// </summary>
    /// <param name="min">minimum long to return</param>
    /// <param name="max">maximum long to return</param>
    /// <returns>long</returns>
    [Pure]
    public static Eff<RT, long> generateLong(long? min = default, long? max = default)
    {
        var minV = min ?? 0;
        var maxV = max ?? long.MaxValue;
        return default(RT).RandomEff.Map(static io => io.GenerateLong()).Map(l => l % (maxV - minV) + minV);
    }

    /// <summary>
    /// Returns a non-negative float
    /// </summary>
    /// <param name="min">minimum float to return</param>
    /// <param name="max">maximum float to return</param>
    /// <returns>float</returns>
    [Pure]
    public static Eff<RT, float> generateFloat(float? min = default, float? max = default)
    {
        var minV = min ?? 0.0f;
        var maxV = max ?? float.MaxValue;
        return default(RT).RandomEff.Map(static io => io.GenerateFloat()).Map(d => d % (maxV - minV) + minV);
    }

    /// <summary>
    /// Returns a non-negative guid
    /// </summary>
    /// <returns>guid</returns>
    [Pure]
    public static Eff<RT, Guid> generateGuid() =>
        default(RT).RandomEff.Map(static io => io.GenerateGuid());

    /// <summary>
    /// Returns a random character
    /// </summary>
    /// <param name="min">min char</param>
    /// <param name="max">max char</param>
    /// <returns>char</returns>
    public static Eff<RT, char> generateChar(char? min = default, char? max = default) =>
        generateInt(min ?? 32, max ?? 126).Map(static i => (char)i);

    /// <summary>
    /// Random duration
    /// </summary>
    /// <param name="min">min duration</param>
    /// <param name="max">max duration</param>
    /// <returns>random duration</returns>
    [Pure]
    public static Eff<RT, Duration> generateDuration(Duration? min = default, Duration? max = default) =>
        generateDouble(min, max).Map(static d => (Duration)d);

    /// <summary>
    /// Random time span
    /// </summary>
    /// <param name="min">min duration</param>
    /// <param name="max">max duration</param>
    /// <returns>random time span</returns>
    [Pure]
    public static Eff<RT, TimeSpan> generateTimespan(Duration? min = default, Duration? max = default) =>
        generateDuration(min, max).Map(static d => (TimeSpan)d);

    /// <summary>
    /// Random date time with offset
    /// </summary>
    /// <param name="min">min duration</param>
    /// <param name="max">max duration</param>
    /// <returns>random date time offset</returns>
    [Pure]
    public static Eff<RT, DateTimeOffset> generateDateTimeOffset(
        DateTimeOffset? min = default,
        DateTimeOffset? max = default) =>
        generateLong(
                (min ?? DateTimeOffset.MinValue).ToUnixTimeMilliseconds(),
                (max ?? DateTimeOffset.MaxValue).ToUnixTimeMilliseconds())
            .Map(static ticks => new DateTimeOffset(ticks, TimeSpan.Zero));

    /// <summary>
    /// Random date time
    /// </summary>
    /// <param name="min">min duration</param>
    /// <param name="max">max duration</param>
    /// <returns>random date time</returns>
    [Pure]
    public static Eff<RT, DateTime> generateDateTime(DateTime? min = default, DateTime? max = default) =>
        generateDateTimeOffset(min, max).Map(static dto => dto.UtcDateTime);

    /// <summary>
    /// Random length enumerable T
    /// </summary>
    /// <param name="effect">effect T</param>
    /// <param name="min">min length</param>
    /// <param name="max">max length</param>
    /// <typeparam name="T">some T</typeparam>
    /// <returns>enumerable of T</returns>
    public static Eff<RT, IEnumerable<T>> generateRange<T>(
        Eff<RT, T> effect,
        int? min = default,
        int? max = default) =>
        generateInt(Math.Abs(min ?? 0), max ?? 1000)
            .Bind(length => Range(0, length).Select(_ => effect).Sequence());

    /// <summary>
    /// Generates a random string
    /// </summary>
    /// <param name="min">min length</param>
    /// <param name="max">max length</param>
    /// <param name="minChar">min char</param>
    /// <param name="maxChar">max char</param>
    /// <returns>string</returns>
    public static Eff<RT, string> generateString(
        int? min = default,
        int? max = default,
        char? minChar = default,
        char? maxChar = default) =>
        generateRange(generateChar(minChar, maxChar), min, max ?? 100)
            .Map(x => new string(x.ToArray()));

    /// <summary>
    /// Returns a random element with an equal probability
    /// </summary>
    /// <param name="elements">list of elements</param>
    /// <typeparam name="T">some T</typeparam>
    /// <returns>random T</returns>
    public static Eff<RT, T> uniform<T>(Lst<NonEmpty, T> elements) =>
        generateInt(0, elements.Count - 1).Map(i => elements[i]);

    /// <summary>
    /// Returns a random element with a weighted probability
    /// </summary>
    /// <param name="elements">weighted list of elements</param>
    /// <typeparam name="T">some T</typeparam>
    /// <returns>random T</returns>
    public static Eff<RT, T> weighted<T>(Lst<NonEmpty, (int weight, T element)> elements)
    {
        var el = elements.Map(x => x with { weight = Math.Abs(x.weight) });
        return generateInt(0, el.Sum(x => x.weight))
            .Map(rand =>
                {
                    var sum = 0;
                    return el.First(x =>
                            {
                                sum += x.weight;
                                return rand < sum;
                            }).element;
                });
    }

    /// <summary>
    /// Returns a random enum from the enumeration
    /// </summary>
    /// <typeparam name="E">enum</typeparam>
    /// <returns>enum</returns>
    public static Eff<RT, E> generateEnum<E>() where E : struct, Enum
    {
        var values = Enum.GetValues(typeof(E)).OfType<E>().ToArr();
        return uniform<E>(List(values[0], values.Tail().ToArray()));
    }

    /// <summary>
    /// Returns a random boolean
    /// </summary>
    /// <returns>enum</returns>
    public static Eff<RT, bool> generateBool() =>
        uniform<bool>(List(true, false));
}
