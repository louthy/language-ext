using System;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Numeric time-span value
/// Handles unit conversions automatically
/// All standard arithmetic operators work on the Time
/// type.  So keep all Times wrapped until you need the
/// value, then extract using various unit-of-measure
/// accessors (Milliseconds, Seconds, etc.) or divide by 1.Second()
/// Implicitly convertible to TimeSpan
/// </summary>
public readonly struct Time :
    IComparable<Time>,
    IEquatable<Time>,
    IComparable
{
    readonly double Value;

    internal Time(double value) =>
        Value = value;

    public override string ToString() =>
        Value + " s";

    public bool Equals(Time other) =>
        Value.Equals(other.Value);

    public bool Equals(Time other, double epsilon) =>
        Math.Abs(other.Value - Value) < epsilon;

    public override bool Equals(object? obj) =>
        obj is Time time && Equals(time);

    public override int GetHashCode() =>
        Value.GetHashCode();

    public int CompareTo(object? obj) =>
        obj switch
        {
            null       => 1,
            Time other => CompareTo(other),
            _          => throw new ArgumentException($"must be of type {nameof(Time)}")
        };

    public int CompareTo(Time other) =>
        Value.CompareTo(other.Value);

    public Time Add(Time rhs) =>
        new (Value + rhs.Value);

    public Time Subtract(Time rhs) =>
        new (Value - rhs.Value);

    public Time Multiply(double rhs) =>
        new (Value * rhs);

    public Time Divide(double rhs) =>
        new (Value / rhs);

    public static Time operator *(Time lhs, double rhs) =>
        lhs.Multiply(rhs);

    public static Time operator *(double lhs, Time rhs) =>
        rhs.Multiply(lhs);

    public static TimeSq operator *(Time lhs, Time rhs) =>
        new (lhs.Value * rhs.Value);

    public static TimeSq operator ^(Time lhs, int power) =>
        power == 2
            ? new TimeSq(lhs.Value * lhs.Value)
            : raise<TimeSq>(new NotSupportedException("Time can only be raised to the power of 2"));

    public static Time operator /(Time lhs, double rhs) =>
        lhs.Divide(rhs);

    public static Time operator +(Time lhs, Time rhs) =>
        lhs.Add(rhs);

    public static DateTime operator +(DateTime lhs, Time rhs) =>
        lhs.AddSeconds(rhs.Seconds);

    public static Time operator -(Time lhs, Time rhs) =>
        lhs.Subtract(rhs);

    public static DateTime operator -(DateTime lhs, Time rhs) =>
        lhs.AddSeconds(-rhs.Seconds);

    public static double operator /(Time lhs, Time rhs) =>
        lhs.Value / rhs.Value;

    public static bool operator ==(Time lhs, Time rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(Time lhs, Time rhs) =>
        !lhs.Equals(rhs);

    public static bool operator >(Time lhs, Time rhs) =>
        lhs.Value > rhs.Value;

    public static bool operator <(Time lhs, Time rhs) =>
        lhs.Value < rhs.Value;

    public static bool operator >=(Time lhs, Time rhs) =>
        lhs.Value >= rhs.Value;

    public static bool operator <=(Time lhs, Time rhs) =>
        lhs.Value <= rhs.Value;

    public Time Pow(double power) =>
        new (Math.Pow(Value, power));

    public Time Round() =>
        new (Math.Round(Value));

    public Time Sqrt() =>
        new (Math.Sqrt(Value));

    public Time Abs() =>
        new (Math.Abs(Value));

    public Time Min(Time rhs) =>
        new (Math.Min(Value, rhs.Value));

    public Time Max(Time rhs) =>
        new (Math.Max(Value, rhs.Value));

    public TimeSpan ToTimeSpan() =>
        TimeSpan.FromSeconds(Value);

    public static implicit operator TimeSpan(Time value) =>
        value.ToTimeSpan();

    public static implicit operator Time(TimeSpan value) =>
        new (value.TotalSeconds);

    public double Seconds       => Value;
    public double Milliseconds  => Value * 1000.0;
    public double Minutes       => Value / 60.0;
    public double Hours         => Value / 3600.0;
    public double Days          => Value / 86400.0;
}

public static class UnitsTimeExtensions
{
    public static Time Milliseconds(this int self) =>
        new (self / 1000.0);

    public static Time Milliseconds(this float self) =>
        new (self / 1000.0);

    public static Time Milliseconds(this double self) =>
        new (self / 1000.0);

    public static Time Seconds(this int self) =>
        new (self);

    public static Time Seconds(this float self) =>
        new (self);

    public static Time Seconds(this double self) =>
        new (self);

    public static Time Minutes(this int self) =>
        new (self * 60.0);

    public static Time Minutes(this float self) =>
        new (self * 60.0);

    public static Time Minutes(this double self) =>
        new (self * 60.0);

    public static Time Hours(this int self) =>
        new (self * 3600.0);

    public static Time Hours(this float self) =>
        new (self * 3600.0);

    public static Time Hours(this double self) =>
        new (self * 3600.0);

    public static Time Days(this int self) =>
        new (self * 86400.0);

    public static Time Days(this float self) =>
        new (self * 86400.0);

    public static Time Days(this double self) =>
        new (self * 86400.0);
}
