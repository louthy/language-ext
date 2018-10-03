using System;

namespace LanguageExt.Core.DataTypes.UnitsOfMeasure
{
  public struct Mass : IComparable<Mass>, IEquatable<Mass>
  {
    readonly double Value;
    internal Mass(double value) =>
        Value = value;

    public override string ToString() =>
        Kilograms + " kg";

    #region CompareTo

    public int CompareTo(Mass other) => Kilograms.CompareTo(other.Kilograms);

    #endregion

    #region Equals

    public bool Equals(Mass other) => Kilograms.Equals(other.Kilograms);

    public bool Equals(Mass other, double epsilon) => Math.Abs(other.Kilograms - Kilograms) < epsilon;

    public override bool Equals(object obj) =>
        obj == null
            ? false
            : obj is Mass
                ? Equals((Mass)obj)
                : false;

    #endregion

    public override int GetHashCode() => Kilograms.GetHashCode();

    #region Basic maths functions

    public Mass Add(Mass rhs) =>
        new Mass(Kilograms + rhs.Kilograms);

    public Mass Subtract(Mass rhs) =>
        new Mass(Kilograms - rhs.Kilograms);

    public Mass Multiply(double rhs) =>
        new Mass(Kilograms * rhs);

    public Mass Divide(double rhs) =>
        new Mass(Kilograms / rhs);

    #endregion

    #region Operators

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

    #endregion

    #region Conversion units

    public double Grams => Value / 1000;
    public double Kilograms => Value;
    public double Tonnes => Value * 1000;

    public double Ounces => Pounds * 16;
    public double Pounds => Value * 2.2046226;
    public double Stones => Pounds / 14;
    public double TonsUK => Value / 0.00098421;
    public double TonsUS => Value / 0.0011023;

    #endregion
  }

  public static class UnitsMassExtensions
  {
    public static Mass Grams(this int self)
        => new Mass(self / 1000.0);
    public static Mass Grams(this double self)
        => new Mass(self / 1000);
    public static Mass Grams(this float self)
        => new Mass(self / 1000);
    public static Mass Kilograms(this int self)
        => new Mass(self);
    public static Mass Kilograms(this double self)
        => new Mass(self);
    public static Mass Kilograms(this float self)
        => new Mass(self);
    public static Mass Tonnes(this int self)
        => new Mass(self * 1000);
    public static Mass Tonnes(this double self)
        => new Mass(self * 1000);
    public static Mass Tonnes(this float self)
        => new Mass(self * 1000);
    public static Mass Ounces(this int self)
        => new Mass(self / 35.273961949);
    public static Mass Ounces(this double self)
        => new Mass(self / 35.273961949);
    public static Mass Ounces(this float self)
        => new Mass(self / 35.273961949);
    public static Mass Pounds(this int self)
        => new Mass(self / 2.2046226219);
    public static Mass Pounds(this double self)
        => new Mass(self / 2.2046226219);
    public static Mass Pounds(this float self)
        => new Mass(self / 2.2046226219);
    public static Mass Stones(this int self)
        => new Mass(self / 0.157473044418);
    public static Mass Stones(this double self)
        => new Mass(self / 0.157473044418);
    public static Mass Stones(this float self)
        => new Mass(self / 0.157473044418);
    public static Mass TonsUK(this int self)
        => new Mass(self / 0.0009842065277);
    public static Mass TonsUK(this double self)
        => new Mass(self / 0.0009842065277);
    public static Mass TonsUK(this float self)
        => new Mass(self / 0.0009842065277);
    public static Mass TonsUS(this int self)
        => new Mass(self / 0.00110231131093);
    public static Mass TonsUS(this double self)
        => new Mass(self / 0.00110231131093);
    public static Mass TonsUS(this float self)
        => new Mass(self / 0.00110231131093);
  }
}
