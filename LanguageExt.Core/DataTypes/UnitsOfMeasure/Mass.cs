using System;
using static LanguageExt.Prelude;

namespace LanguageExt.UnitsOfMeasure
{
    /// <summary>
    /// Numeric mass value
    /// Handles unit conversions automatically
    /// Internally all masses are stored as kilograms
    /// All standard arithmetic operators work on the Mass
    /// type.  So keep all Mass wrapped until you need the
    /// value, then extract using various unit-of-measure
    /// accessors (Kilograms, Pounds, etc.) or divide by 1.Kilogram()
    /// </summary>
    public struct Mass :
        IComparable<Mass>,
        IEquatable<Mass>
    {
        readonly double Value;

        internal Mass(double mass) 
            => Value = mass;

        public override string ToString() =>
            Value + " m";

        public bool Equals(Mass other) =>
            Value.Equals(other.Value);

        public bool Equals(Mass other, double epsilon) =>
            Math.Abs(other.Value - Value) < epsilon;

        public override bool Equals(object obj) =>
            obj is Mass m
                ? Equals(m)
                : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(Mass other) =>
            Value.CompareTo(other.Value);

        public Mass Append(Mass rhs) =>
            new Mass(Value + rhs.Value);

        public Mass Subtract(Mass rhs) =>
            new Mass(Value - rhs.Value);

        public Mass Multiply(double rhs) =>
            new Mass(Value * rhs);

        public Mass Divide(double rhs) =>
            new Mass(Value / rhs);

        public static Area operator *(Mass lhs, Mass rhs) =>
            new Area(lhs.Value * rhs.Value);

        public static Mass operator *(Mass lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static Mass operator *(double lhs, Mass rhs) =>
            rhs.Multiply(lhs);

        public static Mass operator +(Mass lhs, Mass rhs) =>
            lhs.Append(rhs);

        public static Mass operator -(Mass lhs, Mass rhs) =>
            lhs.Subtract(rhs);

        public static Mass operator /(Mass lhs, double rhs) =>
            lhs.Divide(rhs);

        public static double operator /(Mass lhs, Mass rhs) =>
            lhs.Value / rhs.Value;

        public static Force operator *(Mass lhs, Accel rhs) =>
            new Force(lhs.Value * rhs.MetresPerSecond2);

        public static Force operator *(Accel lhs, Mass rhs) =>
            new Force(lhs.MetresPerSecond2 * rhs.Value);

        public static bool operator ==(Mass lhs, Mass rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Mass lhs, Mass rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(Mass lhs, Mass rhs) =>
            lhs.Value > rhs.Value;

        public static bool operator <(Mass lhs, Mass rhs) =>
            lhs.Value < rhs.Value;

        public static bool operator >=(Mass lhs, Mass rhs) =>
            lhs.Value >= rhs.Value;

        public static bool operator <=(Mass lhs, Mass rhs) =>
            lhs.Value <= rhs.Value;

        public Mass Pow(double power) =>
            new Mass(Math.Pow(Value, power));

        public Mass Round() =>
            new Mass(Math.Round(Value));

        public Mass Sqrt() =>
            new Mass(Math.Sqrt(Value));

        public Mass Abs() =>
            new Mass(Math.Abs(Value));

        public Mass Min(Mass rhs) =>
            new Mass(Math.Min(Value, rhs.Value));

        public Mass Max(Mass rhs) =>
            new Mass(Math.Max(Value, rhs.Value));

        public double Kilograms => Value;
        public double Grams => Value * 1_000.0;
        public double Milligrams => Value * 1_000_000.0;
        public double Micrograms => Value * 1_000_000_000.0;
        public double Tonnes => Value * 0.001;
        public double Kilotonnes => Value * 0.000_001;
        public double Megatonnes => Value * 0.000_000_001;
        public double Ounces => Value * 35.27395746;
        public double Pounds => Value * 2.204622341;
    }

    public static class UnitsMassExtensions
    {
        public static Mass Kilograms(this int self) =>
            new Mass(self);

        public static Mass Kilograms(this float self) =>
            new Mass(self);

        public static Mass Kilograms(this double self) =>
            new Mass(self);

        public static Mass Grams(this int self) =>
            new Mass(0.001 * self);

        public static Mass Grams(this float self) =>
            new Mass(0.001 * self);

        public static Mass Grams(this double self) =>
            new Mass(0.001 * self);

        public static Mass Milligrams(this int self) =>
            new Mass(0.000_001 * self);

        public static Mass Milligrams(this float self) =>
            new Mass(0.000_001 * self);

        public static Mass Milligrams(this double self) =>
            new Mass(0.000_001 * self);

        public static Mass Micrograms(this int self) =>
            new Mass(0.000_000_001 * self);

        public static Mass Micrograms(this float self) =>
            new Mass(0.000_000_001 * self);

        public static Mass Micrograms(this double self) =>
            new Mass(0.000_000_001 * self);

        public static Mass Tonnes(this int self) =>
            new Mass(1_000.0 * self);

        public static Mass Tonnes(this float self) =>
            new Mass(1_000.0 * self);

        public static Mass Tonnes(this double self) =>
            new Mass(1_000.0 * self);

        public static Mass Kilotonnes(this int self) =>
            new Mass(1_000_000.0 * self);

        public static Mass Kilotonnes(this float self) =>
            new Mass(1_000_000.0 * self);

        public static Mass Kilotonnes(this double self) =>
            new Mass(1_000_000.0 * self);

        public static Mass Megatonnes(this int self) =>
            new Mass(1_000_000_000.0 * self);

        public static Mass Megatonnes(this float self) =>
            new Mass(1_000_000_000.0 * self);

        public static Mass Megatonnes(this double self) =>
            new Mass(1_000_000_000.0 * self);

        public static Mass Ounces(this int self) =>
            new Mass(0.02834952673 * self);

        public static Mass Ounces(this float self) =>
            new Mass(0.02834952673 * self);

        public static Mass Ounces(this double self) =>
            new Mass(0.02834952673 * self);

        public static Mass Pounds(this int self) =>
            new Mass(0.4535924278 * self);

        public static Mass Pounds(this float self) =>
            new Mass(0.4535924278 * self);

        public static Mass Pounds(this double self) =>
            new Mass(0.4535924278 * self);
    }
}
