using System;

namespace LanguageExt;

/// <summary>
/// Numeric time-span squared value
/// </summary>
public readonly struct TimeSq :
    IComparable<TimeSq>,
    IEquatable<TimeSq>,
    IComparable
{
    readonly double Value;

    internal TimeSq(double value) =>
        Value = value;

    public override string ToString() =>
        Value + " s²";

    public bool Equals(TimeSq other) =>
        Value.Equals(other.Value);

    public bool Equals(TimeSq other, double epsilon) =>
        Math.Abs(other.Value - Value) < epsilon;

    public override bool Equals(object? obj) =>
        obj is TimeSq sq && Equals(sq);

    public override int GetHashCode() =>
        Value.GetHashCode();

    public int CompareTo(object? obj) =>
        obj switch
        {
            null         => 1,
            TimeSq other => CompareTo(other),
            _            => throw new ArgumentException($"must be of type {nameof(TimeSq)}")
        };

    public int CompareTo(TimeSq other) =>
        Value.CompareTo(other.Value);

    public TimeSq Add(TimeSq rhs) =>
        new (Value + rhs.Value);

    public TimeSq Subtract(TimeSq rhs) =>
        new (Value - rhs.Value);

    public TimeSq Multiply(double rhs) =>
        new (Value * rhs);

    public TimeSq Divide(double rhs) =>
        new (Value / rhs);

    public static TimeSq operator *(TimeSq lhs, double rhs) =>
        lhs.Multiply(rhs);

    public static TimeSq operator *(double lhs, TimeSq rhs) =>
        rhs.Multiply(lhs);

    public static TimeSq operator /(TimeSq lhs, double rhs) =>
        lhs.Divide(rhs);

    public static TimeSq operator +(TimeSq lhs, TimeSq rhs) =>
        lhs.Add(rhs);

    public static TimeSq operator -(TimeSq lhs, TimeSq rhs) =>
        lhs.Subtract(rhs);

    public static double operator /(TimeSq lhs, TimeSq rhs) =>
        lhs.Value / rhs.Value;

    public static bool operator ==(TimeSq lhs, TimeSq rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(TimeSq lhs, TimeSq rhs) =>
        !lhs.Equals(rhs);

    public static bool operator >(TimeSq lhs, TimeSq rhs) =>
        lhs.Value > rhs.Value;

    public static bool operator <(TimeSq lhs, TimeSq rhs) =>
        lhs.Value < rhs.Value;

    public static bool operator >=(TimeSq lhs, TimeSq rhs) =>
        lhs.Value >= rhs.Value;

    public static bool operator <=(TimeSq lhs, TimeSq rhs) =>
        lhs.Value <= rhs.Value;

    public TimeSq Round() =>
        new (Math.Round(Value));

    public Time Sqrt() =>
        new (Math.Sqrt(Value));

    public TimeSq Abs() =>
        new (Math.Abs(Value));

    public TimeSq Min(TimeSq rhs) =>
        new (Math.Min(Value, rhs.Value));

    public TimeSq Max(TimeSq rhs) =>
        new (Math.Max(Value, rhs.Value));

    public double Seconds2 => Value;
}
