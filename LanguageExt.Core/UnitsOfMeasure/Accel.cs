using System;
using static LanguageExt.Prelude;

namespace LanguageExt.UnitsOfMeasure
{
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

        public override bool Equals(object obj) =>
            obj == null
                ? false
                : obj is Accel
                    ? Equals((Accel)obj)
                    : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(object obj) => 
            obj is null ? 1
            : obj is Accel other ? CompareTo(other)
            : throw new ArgumentException($"must be of type {nameof(Accel)}");

        public int CompareTo(Accel other) =>
            Value.CompareTo(other.Value);

        public Accel Add(Accel rhs) =>
            new Accel(Value + rhs.Value);

        public Accel Subtract(Accel rhs) =>
            new Accel(Value - rhs.Value);

        public Accel Multiply(double rhs) =>
            new Accel(Value * rhs);

        public Accel Divide(double rhs) =>
            new Accel(Value / rhs);

        public static Accel operator *(Accel lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static Accel operator *(double lhs, Accel rhs) =>
            rhs.Multiply(lhs);

        public static Velocity operator *(Accel lhs, Time rhs) =>
            new Velocity(lhs.Value * rhs.Seconds);

        public static Velocity operator *(Time lhs, Accel rhs) =>
            new Velocity(lhs.Seconds * rhs.Value);

        public static VelocitySq operator *(Accel lhs, Length rhs) =>
            new VelocitySq(lhs.Value * rhs.Metres);

        public static VelocitySq operator *(Length lhs, Accel rhs) =>
            new VelocitySq(rhs.Value * lhs.Metres);

        public static Length operator *(Accel lhs, TimeSq rhs) =>
            new Length(lhs.Value * rhs.Seconds2);

        public static Length operator *(TimeSq lhs, Accel rhs) =>
            new Length(rhs.Value * lhs.Seconds2);

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
            new Accel(Math.Pow(Value,power));

        public Accel Round() =>
            new Accel(Math.Round(Value));

        public Accel Sqrt() =>
            new Accel(Math.Sqrt(Value));

        public Accel Abs() =>
            new Accel(Math.Abs(Value));

        public Accel Min(Accel rhs) =>
            new Accel(Math.Min(Value, rhs.Value));

        public Accel Max(Accel rhs) =>
            new Accel(Math.Max(Value, rhs.Value));

        public double MetresPerSecond2 => Value;
    }

    public static class UnitsAccelExtensions
    {
        public static Accel MetresPerSecond2(this int self) =>
            new Accel(self);

        public static Accel MetresPerSecond2(this float self) =>
            new Accel(self);

        public static Accel MetresPerSecond2(this double self) =>
            new Accel(self);
    }
}
