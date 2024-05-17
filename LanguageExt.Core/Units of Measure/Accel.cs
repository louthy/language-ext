using System;

namespace LanguageExt;

/// <summary>
/// Numeric acceleration value
/// Handles unit conversions automatically
/// Internally all speeds are stored as metres per-second squared
/// All standard arithmetic operators work on the Accel
/// type.  So keep all accelerations wrapped until you need the
/// value, then extract using various unit-of-measure
/// accessors (MetresPerSecond2, etc.) or divide by 1.MetresPerSecond2()
/// </summary>
public readonly struct Accel :
    IComparable<Accel>,
    IEquatable<Accel>,
    IComparable
{
    readonly double Value;

    internal Accel(double value) =>
        Value = value;

    public override string ToString() =>
        Value + " m/s²";

    public bool Equals(Accel other) =>
        Value.Equals(other.Value);

    public bool Equals(Accel other, double epsilon) =>
        Math.Abs(other.Value - Value) < epsilon;

    public override bool Equals(object? obj) =>
        obj is Accel accel && Equals(accel);

    public override int GetHashCode() =>
        Value.GetHashCode();

    public int CompareTo(object? obj) =>
        obj switch
        {
            null        => 1,
            Accel other => CompareTo(other),
            _           => throw new ArgumentException($"must be of type {nameof(Accel)}")
        };

    public int CompareTo(Accel other) =>
        Value.CompareTo(other.Value);

    public Accel Add(Accel rhs) =>
        new (Value + rhs.Value);

    public Accel Subtract(Accel rhs) =>
        new (Value - rhs.Value);

    public Accel Multiply(double rhs) =>
        new (Value * rhs);

    public Accel Divide(double rhs) =>
        new (Value / rhs);

    public static Accel operator *(Accel lhs, double rhs) =>
        lhs.Multiply(rhs);

    public static Accel operator *(double lhs, Accel rhs) =>
        rhs.Multiply(lhs);

    public static Velocity operator *(Accel lhs, Time rhs) =>
        new (lhs.Value * rhs.Seconds);

    public static Velocity operator *(Time lhs, Accel rhs) =>
        new (lhs.Seconds * rhs.Value);

    public static VelocitySq operator *(Accel lhs, Length rhs) =>
        new (lhs.Value * rhs.Metres);

    public static VelocitySq operator *(Length lhs, Accel rhs) =>
        new (rhs.Value * lhs.Metres);

    public static Length operator *(Accel lhs, TimeSq rhs) =>
        new (lhs.Value * rhs.Seconds2);

    public static Length operator *(TimeSq lhs, Accel rhs) =>
        new (rhs.Value * lhs.Seconds2);

    public static Accel operator +(Accel lhs, Accel rhs) =>
        lhs.Add(rhs);

    public static Accel operator -(Accel lhs, Accel rhs) =>
        lhs.Subtract(rhs);

    public static Accel operator /(Accel lhs, double rhs) =>
        lhs.Divide(rhs);

    public static double operator /(Accel lhs, Accel rhs) =>
        lhs.Value / rhs.Value;

    public static bool operator ==(Accel lhs, Accel rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(Accel lhs, Accel rhs) =>
        !lhs.Equals(rhs);

    public static bool operator >(Accel lhs, Accel rhs) =>
        lhs.Value > rhs.Value;

    public static bool operator <(Accel lhs, Accel rhs) =>
        lhs.Value < rhs.Value;

    public static bool operator >=(Accel lhs, Accel rhs) =>
        lhs.Value >= rhs.Value;

    public static bool operator <=(Accel lhs, Accel rhs) =>
        lhs.Value <= rhs.Value;

    public Accel Pow(double power) =>
        new (Math.Pow(Value,power));

    public Accel Round() =>
        new (Math.Round(Value));

    public Accel Sqrt() =>
        new (Math.Sqrt(Value));

    public Accel Abs() =>
        new (Math.Abs(Value));

    public Accel Min(Accel rhs) =>
        new (Math.Min(Value, rhs.Value));

    public Accel Max(Accel rhs) =>
        new (Math.Max(Value, rhs.Value));

    public double MetresPerSecond2 => Value;
}

public static class UnitsAccelExtensions
{
    public static Accel MetresPerSecond2(this int self) =>
        new (self);

    public static Accel MetresPerSecond2(this float self) =>
        new (self);

    public static Accel MetresPerSecond2(this double self) =>
        new (self);
}
