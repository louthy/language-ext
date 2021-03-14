using System;

namespace LanguageExt.UnitsOfMeasure
{
    public readonly struct Mass : 
        IComparable<Mass>, 
        IEquatable<Mass>, 
        IComparable
    {
        readonly double Value;

        internal Mass(double value) =>
            Value = value;

        public override string ToString() =>
            Kilograms + " kg";

        public int CompareTo(object obj) =>
            obj is null ? 1
            : obj is Mass other ? CompareTo(other)
            : throw new ArgumentException($"must be of type {nameof(Mass)}");

        public int CompareTo(Mass other) =>
            Kilograms.CompareTo(other.Kilograms);

        public bool Equals(Mass other) =>
            Kilograms.Equals(other.Kilograms);

        public bool Equals(Mass other, double epsilon) =>
            Math.Abs(other.Kilograms - Kilograms) < epsilon;

        public override bool Equals(object obj) =>
            obj is Mass m && Equals(m);

        public override int GetHashCode() =>
            Kilograms.GetHashCode();

        public Mass Add(Mass rhs) =>
            new Mass(Kilograms + rhs.Kilograms);

        public Mass Subtract(Mass rhs) =>
            new Mass(Kilograms - rhs.Kilograms);

        public Mass Multiply(double rhs) =>
            new Mass(Kilograms * rhs);

        public Mass Divide(double rhs) =>
            new Mass(Kilograms / rhs);

        public static Mass operator *(Mass lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static Mass operator *(double lhs, Mass rhs) =>
            rhs.Multiply(lhs);

        public static Mass operator +(Mass lhs, Mass rhs) =>
            lhs.Add(rhs);

        public static Mass operator -(Mass lhs, Mass rhs) =>
            lhs.Subtract(rhs);

        public static Mass operator /(Mass lhs, double rhs) =>
            lhs.Divide(rhs);

        public static double operator /(Mass lhs, Mass rhs) =>
            lhs.Kilograms / rhs.Kilograms;

        public static bool operator ==(Mass lhs, Mass rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Mass lhs, Mass rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(Mass lhs, Mass rhs) =>
            lhs.Kilograms > rhs.Kilograms;

        public static bool operator <(Mass lhs, Mass rhs) =>
            lhs.Kilograms < rhs.Kilograms;

        public static bool operator >=(Mass lhs, Mass rhs) =>
            lhs.Kilograms >= rhs.Kilograms;

        public static bool operator <=(Mass lhs, Mass rhs) =>
            lhs.Kilograms <= rhs.Kilograms;

        public Mass Round() =>
            new Mass(Math.Round(Kilograms));

        public Mass Sqrt() =>
            new Mass(Math.Sqrt(Kilograms));

        public Mass Abs() =>
            new Mass(Math.Abs(Kilograms));

        public Mass Min(Mass rhs) =>
            new Mass(Math.Min(Kilograms, rhs.Kilograms));

        public Mass Max(Mass rhs) =>
            new Mass(Math.Max(Kilograms, rhs.Kilograms));

        public double Grams => Value * 1000.0;
        public double Kilograms => Value;
        public double Tonnes => Value / 1000.0;

        public double Ounces => Pounds * 16.0;
        public double Pounds => Value * 2.2046226;
        public double Stones => Pounds / 14.0;
        public double ImperialTons => Value / 0.000984207;
        public double ShortTons => Value / 0.00110231;
    }

    public static class UnitsMassExtensions
    {
        public static Mass Grams(this int self) =>
            new Mass(self / 1000.0);

        public static Mass Grams(this double self) =>
            new Mass(self / 1000.0);

        public static Mass Grams(this float self) =>
            new Mass(self / 1000.0);

        public static Mass Kilograms(this int self) =>
            new Mass(self);

        public static Mass Kilograms(this double self) =>
            new Mass(self);

        public static Mass Kilograms(this float self) =>
            new Mass(self);

        public static Mass Tonnes(this int self) =>
            new Mass(self * 1000.0);

        public static Mass Tonnes(this double self) =>
            new Mass(self * 1000.0);

        public static Mass Tonnes(this float self) =>
            new Mass(self * 1000.0);

        public static Mass Ounces(this int self) =>
            new Mass(self / 35.273961949);

        public static Mass Ounces(this double self) =>
            new Mass(self / 35.273961949);

        public static Mass Ounces(this float self) =>
            new Mass(self / 35.273961949);

        public static Mass Pounds(this int self) =>
            new Mass(self / 2.2046226219);

        public static Mass Pounds(this double self) =>
            new Mass(self / 2.2046226219);

        public static Mass Pounds(this float self) =>
            new Mass(self / 2.2046226219);

        public static Mass Stones(this int self) =>
            new Mass(self / 0.157473044418);

        public static Mass Stones(this double self) =>
            new Mass(self / 0.157473044418);

        public static Mass Stones(this float self) =>
            new Mass(self / 0.157473044418);

        public static Mass ImperialTons(this int self) =>
            new Mass(self / 0.0009842065277);

        public static Mass ImperialTons(this double self) =>
            new Mass(self / 0.0009842065277);

        public static Mass ImperialTons(this float self) =>
            new Mass(self / 0.0009842065277);

        public static Mass ShortTon(this int self) =>
            new Mass(self / 0.00110231131093);

        public static Mass ShortTon(this double self) =>
            new Mass(self / 0.00110231131093);

        public static Mass ShortTon(this float self) =>
            new Mass(self / 0.00110231131093);
    }
}
