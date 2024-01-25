using System;

namespace LanguageExt;

/// <summary>
/// Numeric length value
/// Handles unit conversions automatically
/// Internally all lengths are stored as metres
/// All standard arithmetic operators work on the Length
/// type.  So keep all Lengths wrapped until you need the
/// value, then extract using various unit-of-measure
/// accessors (Metres, Centimetres, etc.) or divide by 1.Metre()
/// </summary>
public readonly struct Length :
    IComparable<Length>,
    IEquatable<Length>,
    IComparable
{
    readonly double Value;

    internal Length(double value) =>
        Value = value;

    public override string ToString() =>
        Value + " m";

    public bool Equals(Length other) =>
        Value.Equals(other.Value);

    public bool Equals(Length other, double epsilon) =>
        Math.Abs(other.Value - Value) < epsilon;

    public override bool Equals(object? obj) =>
        obj is Length length && Equals(length);

    public override int GetHashCode() =>
        Value.GetHashCode();

    public int CompareTo(object? obj) =>
        obj switch
        {
            null         => 1,
            Length other => CompareTo(other),
            _            => throw new ArgumentException($"must be of type {nameof(Length)}")
        };

    public int CompareTo(Length other) =>
        Value.CompareTo(other.Value);

    public Length Add(Length rhs) =>
        new (Value + rhs.Value);

    public Length Subtract(Length rhs) =>
        new (Value - rhs.Value);

    public Length Multiply(double rhs) =>
        new (Value * rhs);

    public Length Divide(double rhs) =>
        new (Value / rhs);

    public static Area operator *(Length lhs, Length rhs) =>
        new (lhs.Value * rhs.Value);

    public static Length operator *(Length lhs, double rhs) =>
        lhs.Multiply(rhs);

    public static Length operator *(double lhs, Length rhs) =>
        rhs.Multiply(lhs);

    public static Length operator +(Length lhs, Length rhs) =>
        lhs.Add(rhs);

    public static Length operator -(Length lhs, Length rhs) =>
        lhs.Subtract(rhs);

    public static Length operator /(Length lhs, double rhs) =>
        lhs.Divide(rhs);

    public static double operator /(Length lhs, Length rhs) =>
        lhs.Value / rhs.Value;

    public static Accel operator /(Length lhs, TimeSq rhs) =>
        new Accel(lhs.Value / rhs.Seconds2);

    public static Time operator /(Length lhs, Velocity rhs) =>
        new Time(lhs.Metres / rhs.MetresPerSecond);

    public static Velocity operator /(Length lhs, Time rhs) =>
        new Velocity(lhs.Value / rhs.Seconds);

    public static bool operator ==(Length lhs, Length rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(Length lhs, Length rhs) =>
        !lhs.Equals(rhs);

    public static bool operator >(Length lhs, Length rhs) =>
        lhs.Value > rhs.Value;

    public static bool operator <(Length lhs, Length rhs) =>
        lhs.Value < rhs.Value;

    public static bool operator >=(Length lhs, Length rhs) =>
        lhs.Value >= rhs.Value;

    public static bool operator <=(Length lhs, Length rhs) =>
        lhs.Value <= rhs.Value;

    public Length Pow(double power) =>
        new (Math.Pow(Value,power));

    public Length Round() =>
        new (Math.Round(Value));

    public Length Sqrt() =>
        new (Math.Sqrt(Value));

    public Length Abs() =>
        new (Math.Abs(Value));

    public Length Min(Length rhs) =>
        new (Math.Min(Value, rhs.Value));

    public Length Max(Length rhs) =>
        new (Math.Max(Value, rhs.Value));

    public double Miles       => Value * 6.2137119223484848484848484848485e-4;
    public double Yards       => Value * 1.0936132983333333333333333333333;
    public double Feet        => Value * 3.280839895;
    public double Inches      => Value * 39.37007874;
    public double Kilometres  => Value / 1000.0;
    public double Hectometres => Value / 100.0;
    public double Decametres  => Value / 10.0;
    public double Metres      => Value;
    public double Centimetres => Value * 100.0;
    public double Millimetres => Value * 1000.0;
    public double Micrometres => Value * 1000000.0;
    public double Nanometres  => Value * 1000000000.0;
    public double Angstroms   => Value * 10000000000.0;
}

public static class UnitsLengthExtensions
{
    public static Length Miles(this int self) =>
        new (1609.344000006437376000025749504 * self);

    public static Length Miles(this float self) =>
        new (1609.344000006437376000025749504 * self);

    public static Length Miles(this double self) =>
        new (1609.344000006437376000025749504 * self);

    public static Length Yards(this int self) =>
        new (0.9144000000036576000000146304 * self);

    public static Length Yards(this float self) =>
        new (0.9144000000036576000000146304 * self);

    public static Length Yards(this double self) =>
        new (0.9144000000036576000000146304 * self);

    public static Length Feet(this int self) =>
        new (0.3048000000012192000000048768 * self);

    public static Length Feet(this float self) =>
        new (0.3048000000012192000000048768 * self);

    public static Length Feet(this double self) =>
        new (0.3048000000012192000000048768 * self);

    public static Length Inches(this int self) =>
        new (0.0254000000001016000000004064 * self);

    public static Length Inches(this float self) =>
        new (0.0254000000001016000000004064 * self);

    public static Length Inches(this double self) =>
        new (0.0254000000001016000000004064 * self);

    public static Length Kilometres(this int self) =>
        new (1000.0 * self);

    public static Length Kilometres(this float self) =>
        new (1000.0 * self);

    public static Length Kilometres(this double self) =>
        new (1000.0 * self);

    public static Length Metres(this int self) =>
        new (self);

    public static Length Metres(this float self) =>
        new (self);

    public static Length Metres(this double self) =>
        new (self);

    public static Length Centimetres(this int self) =>
        new (self / 100.0);

    public static Length Centimetres(this float self) =>
        new (self / 100.0);

    public static Length Centimetres(this double self) =>
        new (self / 100.0);

    public static Length Millimetres(this int self) =>
        new (self / 1000.0);

    public static Length Millimetres(this float self) =>
        new (self / 1000.0);

    public static Length Millimetres(this double self) =>
        new (self / 1000.0);

    public static Length Micrometres(this int self) =>
        new (self / 1000000.0);

    public static Length Micrometres(this float self) =>
        new (self / 1000000.0);

    public static Length Micrometres(this double self) =>
        new (self / 1000000.0);

    public static Length Nanometres(this int self) =>
        new (self / 1000000000.0);

    public static Length Nanometres(this float self) =>
        new (self / 1000000000.0);

    public static Length Nanometres(this double self) =>
        new (self / 1000000000.0);

    public static Length Angstroms(this int self) =>
        new (self / 10000000000.0);

    public static Length Angstroms(this float self) =>
        new (self / 10000000000.0);

    public static Length Angstroms(this double self) =>
        new (self / 10000000000.0);
}
