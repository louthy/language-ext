using System;

namespace LanguageExt.UnitsOfMeasure
{
    /// <summary>
    /// Numeric pressure value
    /// Handles unit conversions automatically
    /// Internally all pressures are stored as pascal
    /// All standard arithmetic operators work on the Pressure
    /// type.  So keep all pressures wrapped until you need the
    /// value, then extract using various unit-of-measure
    /// accessors (Bar, etc.) or divide by 1.Pascal()
    /// </summary>
    public struct Pressure :
        IComparable<Pressure>,
        IEquatable<Pressure>
    {
        readonly double Value;

        internal Pressure(double pressure) => 
            Value = pressure;

        public override string ToString() =>
            Value + " Pa";

        public bool Equals(Pressure other) =>
            Value.Equals(other.Value);

        public bool Equals(Pressure other, double epsilon) =>
            Math.Abs(other.Value - Value) < epsilon;

        public override bool Equals(object obj) => 
            obj is Pressure p 
                ? Equals(p) 
                : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(Pressure other) =>
            Value.CompareTo(other.Value);

        public Pressure Add(Pressure rhs) =>
            new Pressure(Value + rhs.Value);

        public Pressure Subtract(Pressure rhs) =>
            new Pressure(Value - rhs.Value);

        public Pressure Multiply(double rhs) =>
            new Pressure(Value * rhs);

        public Pressure Divide(double rhs) =>
            new Pressure(Value / rhs);

        public static Pressure operator *(Pressure lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static Pressure operator *(double lhs, Pressure rhs) =>
            rhs.Multiply(lhs);

        public static Force operator *(Pressure lhs, Area rhs) =>
            new Force(lhs.Value * rhs.SqMetres);

        public static Force operator *(Area lhs, Pressure rhs) =>
            new Force(rhs.Value * lhs.SqMetres);

        public static Pressure operator +(Pressure lhs, Pressure rhs) =>
            lhs.Add(rhs);

        public static Pressure operator -(Pressure lhs, Pressure rhs) =>
            lhs.Subtract(rhs);

        public static Pressure operator /(Pressure lhs, double rhs) =>
            lhs.Divide(rhs);

        public static double operator /(Pressure lhs, Pressure rhs) =>
            lhs.Value / rhs.Value;

        public static bool operator ==(Pressure lhs, Pressure rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Pressure lhs, Pressure rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(Pressure lhs, Pressure rhs) =>
            lhs.Value > rhs.Value;

        public static bool operator <(Pressure lhs, Pressure rhs) =>
            lhs.Value < rhs.Value;

        public static bool operator >=(Pressure lhs, Pressure rhs) =>
            lhs.Value >= rhs.Value;

        public static bool operator <=(Pressure lhs, Pressure rhs) =>
            lhs.Value <= rhs.Value;

        public Pressure Pow(double power) =>
            new Pressure(Math.Pow(Value, power));

        public Pressure Round() =>
            new Pressure(Math.Round(Value));

        public Pressure Sqrt() =>
            new Pressure(Math.Sqrt(Value));

        public Pressure Abs() =>
            new Pressure(Math.Abs(Value));

        public Pressure Min(Pressure rhs) =>
            new Pressure(Math.Min(Value, rhs.Value));

        public Pressure Max(Pressure rhs) =>
            new Pressure(Math.Max(Value, rhs.Value));

        public double Pascal => Value;

        public double Hectopascal => Value * 0.01;

        public double NewtonsPerMetre2 => Value;

        public double Bar => Value * 0.00001;

        public double Millibar => Value * 0.01;

        public double Centibar => Value * 0.001;

        public double Psi => Value * 0.000145038;

        public double Atmospheres => Value * 9.86923e-6;
    }

    public static class UnitsPressureExtensions
    {
        public static Pressure Pascal(this int self) =>
            new Pressure(self);

        public static Pressure Pascal(this float self) =>
            new Pressure(self);

        public static Pressure Pascal(this double self) =>
            new Pressure(self);

        public static Pressure Hectopascals(this int self) =>
            new Pressure(100.0 * self);

        public static Pressure Hectopascals(this float self) =>
            new Pressure(100.0 * self);

        public static Pressure Hectopascals(this double self) =>
            new Pressure(100.0 * self);

        public static Pressure NewtonsPerMetre2(this int self) =>
            new Pressure(self);

        public static Pressure NewtonsPerMetre2(this float self) =>
            new Pressure(self);

        public static Pressure NewtonsPerMetre2(this double self) =>
            new Pressure(self);

        public static Pressure Bar(this int self) =>
            new Pressure(100000.0 * self);

        public static Pressure Bar(this float self) =>
            new Pressure(100000.0 * self);

        public static Pressure Bar(this double self) =>
            new Pressure(100000.0 * self);

        public static Pressure Millibars(this int self) =>
            new Pressure(100.0 * self);

        public static Pressure Millibars(this float self) =>
            new Pressure(100.0 * self);

        public static Pressure Millibars(this double self) =>
            new Pressure(100.0 * self);

        public static Pressure Centibars(this int self) =>
            new Pressure(1000.0 * self);

        public static Pressure Centibars(this float self) =>
            new Pressure(1000.0 * self);

        public static Pressure Centibars(this double self) =>
            new Pressure(1000.0 * self);

        public static Pressure Psi(this int self) =>
            new Pressure(6894.757 * self);

        public static Pressure Psi(this float self) =>
            new Pressure(6894.757 * self);

        public static Pressure Psi(this double self) =>
            new Pressure(6894.757 * self);

        public static Pressure Atmospheres(this int self) =>
            new Pressure(101325.0 * self);

        public static Pressure Atmospheres(this float self) =>
            new Pressure(101325.0 * self);

        public static Pressure Atmospheres(this double self) =>
            new Pressure(101325.0 * self);
    }
}
