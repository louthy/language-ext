using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt;

/// <summary>
/// A duration that is always positive.
/// </summary>
public readonly struct PositiveDuration :
    IEquatable<PositiveDuration>,
    IComparable<PositiveDuration>
{
    private readonly double _milliseconds;

    public PositiveDuration(double milliseconds) => _milliseconds = Math.Abs(milliseconds);

    public static PositiveDuration Zero => new(0);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PositiveDuration(double milliseconds) =>
        new(milliseconds);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PositiveDuration(TimeSpan timeSpan) =>
        new(timeSpan.TotalMilliseconds);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PositiveDuration(Time time) =>
        new(time.Milliseconds);

    [Pure]
    public static implicit operator double(PositiveDuration duration) =>
        duration._milliseconds;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator TimeSpan(PositiveDuration duration) =>
        TimeSpan.FromMilliseconds(duration._milliseconds);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Time(PositiveDuration duration) =>
        duration._milliseconds.Milliseconds();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(PositiveDuration a, PositiveDuration b)
        => a._milliseconds.Equals(b._milliseconds);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(PositiveDuration a, PositiveDuration b) => !(a == b);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(PositiveDuration other) => _milliseconds.Equals(other);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object obj) => obj is PositiveDuration other && Equals(other);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _milliseconds.GetHashCode();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(PositiveDuration other) => _milliseconds.CompareTo(other);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(PositiveDuration a, PositiveDuration b) => a.CompareTo(b) == 1;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(PositiveDuration a, PositiveDuration b) => a.CompareTo(b) >= 0;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(PositiveDuration a, PositiveDuration b) => a.CompareTo(b) == -1;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(PositiveDuration a, PositiveDuration b) => a.CompareTo(b) <= 0;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => $"{nameof(PositiveDuration)}({(TimeSpan)this})";
}
