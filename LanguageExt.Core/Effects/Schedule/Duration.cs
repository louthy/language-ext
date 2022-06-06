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

    public Duration(double milliseconds)
    {
        if (milliseconds < 0) throw new ArgumentException("milliseconds must be a positive number.");
        Milliseconds = milliseconds;
    }

    public static Duration Zero =>
        new(0);

    /// <summary>
    /// Random duration between the provided min and max durations. 
    /// </summary>
    /// <remarks>
    /// This can be used to seed a schedule in parallel.
    /// Providing another method of decorrelation.
    ///
    /// For example, this is a linear schedule that,
    ///
    /// - starts with a seed duration between 10 and 50 milliseconds
    /// - includes a 10% jitter, added and removed in sequence from each duration
    /// - recurring 5 times
    ///
    ///     Schedule.Linear(Duration.Random(10 * ms, 50 * ms)) | Schedule.Decorrelate() | Schedule.Recurs(5)
    ///
    /// Three runs result in,
    ///
    ///     (25 * ms, 23 * ms, 50 * ms, 47 * ms, 72 * ms)
    ///     (13 * ms, 11 * ms, 25 * ms, 23 * ms, 40 * ms)
    ///     (28 * ms, 25 * ms, 56 * ms, 53 * ms, 87 * ms)
    ///
    /// </remarks>
    /// <param name="min">min duration</param>
    /// <param name="max">max duration</param>
    /// <param name="seed">optional seed</param>
    /// <returns>random duration between min and max duration</returns>
    [Pure]
    public static Duration Random(Duration min, Duration max, Option<int> seed = default)
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
