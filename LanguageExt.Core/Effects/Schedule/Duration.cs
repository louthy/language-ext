#nullable enable

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt;

/// <summary>
/// Period of time in milliseconds.
/// Can be used to convert between other duration like types, such as TimeSpan and Time.
/// </summary>
public readonly struct Duration :
    IEquatable<Duration>,
    IComparable<Duration>
{
    readonly double Milliseconds;

    /// <summary>
    /// Duration constructor
    /// </summary>
    /// <param name="milliseconds">Magnitude of the duration.  Must be zero or a positive value</param>
    /// <exception cref="ArgumentException">Throws if `milliseconds` is less than `0`</exception>
    public Duration(double milliseconds)
    {
        if (milliseconds < 0) throw new ArgumentException($"{nameof(milliseconds)} must be a positive number.");
        Milliseconds = milliseconds;
    }

    /// <summary>
    /// Zero magnitude duration (instant)
    /// </summary>
    public static Duration Zero =
        new(0);

    /// <summary>
    /// Random duration between the provided min and max durations. 
    /// </summary>
    /// <remarks>
    /// This can be used to seed a schedule in parallel.
    /// Providing another method of de-correlation.
    ///
    /// For example, this is a linear schedule that,
    ///
    /// - starts with a seed duration between 10 and 50 milliseconds
    /// - includes a 10% jitter, added and removed in sequence from each duration
    /// - recurring 5 times
    ///
    ///     Schedule.linear(Duration.Random(10*ms, 50*ms)) | Schedule.decorrelate() | Schedule.recurs(5)
    ///
    /// Three runs result in,
    ///
    ///     (25ms, 23ms, 50ms, 47ms, 72ms)
    ///     (13ms, 11ms, 25ms, 23ms, 40ms)
    ///     (28ms, 25ms, 56ms, 53ms, 87ms)
    ///
    /// </remarks>
    /// <param name="min">min duration</param>
    /// <param name="max">max duration</param>
    /// <param name="seed">optional seed</param>
    /// <returns>random duration between min and max duration</returns>
    [Pure]
    public static Duration random(Duration min, Duration max, Option<int> seed = default)
        => new(SingletonRandom.Uniform(min, max, seed));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Duration(double milliseconds) =>
        new(milliseconds);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Duration(TimeSpan timeSpan) =>
        new(timeSpan.TotalMilliseconds);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Duration(Time time) =>
        new(time.Milliseconds);

    [Pure]
    public static implicit operator double(Duration duration) =>
        duration.Milliseconds;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator TimeSpan(Duration duration) =>
        TimeSpan.FromMilliseconds(duration.Milliseconds);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Time(Duration duration) =>
        duration.Milliseconds.Milliseconds();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Duration a, Duration b) =>
        a.Milliseconds.Equals(b.Milliseconds);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Duration a, Duration b) =>
        !(a == b);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Duration other) =>
        Milliseconds.Equals(other);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object obj) =>
        obj is Duration other && Equals(other);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() =>
        Milliseconds.GetHashCode();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Duration other) =>
        Milliseconds.CompareTo(other);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Duration a, Duration b) =>
        a.CompareTo(b) > 0;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Duration a, Duration b) =>
        a.CompareTo(b) >= 0;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Duration a, Duration b) =>
        a.CompareTo(b) < 0;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Duration a, Duration b) =>
        a.CompareTo(b) <= 0;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() =>
        $"{nameof(Duration)}({(TimeSpan)this})";
}
