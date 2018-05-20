using System;

namespace LanguageExt.UnitsOfMeasure
{
    /// <summary>
    /// Numeric force value
    /// Handles unit conversions automatically
    /// Internally all forces are stored as newtons
    /// All standard arithmetic operators work on the Force
    /// type.  So keep all Forces wrapped until you need the
    /// value, then extract using various unit-of-measure
    /// accessors (Lbf, etc.) or divide by 1.Newton()
    /// </summary>
    public struct Force :
        IComparable<Force>,
        IEquatable<Force>
    {
        readonly double Value;

        internal Force(double force) 
            => Value = force;

        public override string ToString() =>
            Value + " N";

        public bool Equals(Force other) =>
            Value.Equals(other.Value);

        public bool Equals(Force other, double epsilon) =>
            Math.Abs(other.Value - Value) < epsilon;

        public override bool Equals(object obj) =>
            obj is Force f
                ? Equals(f)
                : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(Force other) =>
            Value.CompareTo(other.Value);

        public Force Append(Force rhs) =>
            new Force(Value + rhs.Value);

        public Force Subtract(Force rhs) =>
            new Force(Value - rhs.Value);

        public Force Multiply(double rhs) =>
            new Force(Value * rhs);

        public Force Divide(double rhs) =>
            new Force(Value / rhs);

        public static Force operator *(Force lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static Force operator *(double lhs, Force rhs) =>
            rhs.Multiply(lhs);

        public static Force operator +(Force lhs, Force rhs) =>
            lhs.Append(rhs);

        public static Force operator -(Force lhs, Force rhs) =>
            lhs.Subtract(rhs);

        public static Force operator /(Force lhs, double rhs) =>
            lhs.Divide(rhs);

        public static double operator /(Force lhs, Force rhs) =>
            lhs.Value / rhs.Value;

        public static Pressure operator /(Force lhs, Area rhs) =>
            new Pressure(lhs.Value / rhs.SqMetres);

        public static Mass operator /(Force lhs, Accel rhs) =>
            new Mass(lhs.Value / rhs.MetresPerSecond2);

        public static Accel operator /(Force lhs, Mass rhs) =>
            new Accel(lhs.Value / rhs.Kilograms);

        public static bool operator ==(Force lhs, Force rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Force lhs, Force rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(Force lhs, Force rhs) =>
            lhs.Value > rhs.Value;

        public static bool operator <(Force lhs, Force rhs) =>
            lhs.Value < rhs.Value;

        public static bool operator >=(Force lhs, Force rhs) =>
            lhs.Value >= rhs.Value;

        public static bool operator <=(Force lhs, Force rhs) =>
            lhs.Value <= rhs.Value;

        public Force Pow(double power) =>
            new Force(Math.Pow(Value, power));

        public Force Round() =>
            new Force(Math.Round(Value));

        public Force Sqrt() =>
            new Force(Math.Sqrt(Value));

        public Force Abs() =>
            new Force(Math.Abs(Value));

        public Force Min(Force rhs) =>
            new Force(Math.Min(Value, rhs.Value));

        public Force Max(Force rhs) =>
            new Force(Math.Max(Value, rhs.Value));

        public double Newtons => Value;

        public double Lbf => Value * 0.224809;
    }

    public static class UnitsForceExtensions
    {
        public static Force Newtons(this int self) =>
            new Force(self);

        public static Force Newtons(this float self) =>
            new Force(self);

        public static Force Newtons(this double self) =>
            new Force(self);

        public static Force Lbf(this int self) =>
            new Force(4.448222 * self);

        public static Force Lbf(this float self) =>
            new Force(4.448222 * self);

        public static Force Lbf(this double self) =>
            new Force(4.448222 * self);
    }
}
