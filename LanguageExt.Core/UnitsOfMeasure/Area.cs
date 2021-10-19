using System;
using static LanguageExt.Prelude;

namespace LanguageExt.UnitsOfMeasure
{
    /// <summary>
    /// Numeric area value
    /// Handles unit conversions automatically
    /// Internally all areas are stored as metres^2
    /// All standard arithmetic operators work on the Area
    /// type.  So keep all Areas wrapped until you need the
    /// value, then extract using various unit-of-measure
    /// accessors (SqMetres, SqCentimetres, etc.) or divide by 1.SqMetre()
    /// </summary>
    public readonly struct Area :
        IComparable<Area>,
        IEquatable<Area>,
        IComparable
    {
        readonly double Value;

        internal Area(double value) =>
            Value = value;

        public override string ToString() =>
            Value + " m²";

        public bool Equals(Area other) =>
            Value.Equals(other.Value);

        public bool Equals(Area other, double epsilon) =>
            Math.Abs(other.Value - Value) < epsilon;

        public override bool Equals(object obj) =>
            obj == null
                ? false
                : obj is Area
                    ? Equals((Area)obj)
                    : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(object obj) => 
            obj is null ? 1
            : obj is Area other ? CompareTo(other)
            : throw new ArgumentException($"must be of type {nameof(Area)}");

        public int CompareTo(Area other) =>
            Value.CompareTo(other.Value);

        public Area Add(Area rhs) =>
            new Area(Value + rhs.Value);

        public Area Subtract(Area rhs) =>
            new Area(Value - rhs.Value);

        public Area Multiply(double rhs) =>
            new Area(Value * rhs);

        public Area Divide(double rhs) =>
            new Area(Value / rhs);

        public static Area operator *(Area lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static Area operator *(double lhs, Area rhs) =>
            rhs.Multiply(lhs);

        public static Area operator /(Area lhs, double rhs) =>
            lhs.Divide(rhs);

        public static Area operator +(Area lhs, Area rhs) =>
            lhs.Add(rhs);

        public static Area operator -(Area lhs, Area rhs) =>
            lhs.Subtract(rhs);

        public static Length operator /(Area lhs, Length rhs) =>
            new Length(lhs.Value / rhs.Metres);

        public static double operator /(Area lhs, Area rhs) =>
            lhs.Value / rhs.Value;

        public static bool operator ==(Area lhs, Area rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Area lhs, Area rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(Area lhs, Area rhs) =>
            lhs.Value > rhs.Value;

        public static bool operator <(Area lhs, Area rhs) =>
            lhs.Value < rhs.Value;

        public static bool operator >=(Area lhs, Area rhs) =>
            lhs.Value >= rhs.Value;

        public static bool operator <=(Area lhs, Area rhs) =>
            lhs.Value <= rhs.Value;

        public Area Pow(double power) =>
            new Area(Math.Pow(Value,power));

        public Area Round() =>
            new Area(Math.Round(Value));

        public Area Sqrt() =>
            new Area(Math.Sqrt(Value));

        public Length Abs() =>
            new Length(Math.Abs(Value));

        public Area Min(Area rhs) =>
            new Area(Math.Min(Value, rhs.Value));

        public Area Max(Area rhs) =>
            new Area(Math.Max(Value, rhs.Value));

        public double SqKilometres  => Value * 0.000001;
        public double SqMetres      => Value;
        public double SqCentimetres => Value * 10000.0;
        public double SqMillimetres => Value * 1000000.0;
    }

    public static class UnitsAreaExtensions
    {
        public static Area SqKilometres(this int self) =>
            new Area(self / 0.000001);

        public static Area SqKilometres(this float self) =>
            new Area(self / 0.000001);

        public static Area SqKilometres(this double self) =>
            new Area(self / 0.000001);

        public static Area SqMetres(this int self) =>
            new Area(self);

        public static Area SqMetres(this float self) =>
            new Area(self);

        public static Area SqMetres(this double self) =>
            new Area(self);

        public static Area SqCentimetres(this int self) =>
            new Area(self / 10000.0);

        public static Area SqCentimetres(this float self) =>
            new Area(self / 10000.0);

        public static Area SqCentimetres(this double self) =>
            new Area(self / 10000.0);

        public static Area SqMillimetres(this int self) =>
            new Area(self / 1000000.0);

        public static Area SqMillimetres(this float self) =>
            new Area(self / 1000000.0);

        public static Area SqMillimetres(this double self) =>
            new Area(self / 1000000.0);
    }
}
