using System;

namespace LanguageExt.UnitsOfMeasure
{
    /// <summary>
    /// Numeric thermodynamic temperature value
    /// Handles unit conversions automatically
    /// Internally all temperatures are stored as Kelvin
    /// All standard arithmetic operators work on the Temperature
    /// type.  So keep all Temperatures wrapped until you need the
    /// value, then extract using various unit-of-measure
    /// accessors (Celsius, etc.) or divide by 1.Kelvin()
    /// </summary>
    public struct Temperature :
        IComparable<Temperature>,
        IEquatable<Temperature>
    {
        readonly double Value;

        internal Temperature(double temperature) =>
            Value = temperature;

        public override string ToString() =>
            Value + " K";

        public bool Equals(Temperature other) =>
            Value.Equals(other.Value);

        public bool Equals(Temperature other, double epsilon) =>
            Math.Abs(other.Value - Value) < epsilon;

        public override bool Equals(object obj) =>
            obj is Temperature t
                ? Equals(t)
                : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(Temperature other) =>
            Value.CompareTo(other.Value);

        public Temperature Add(Temperature rhs) =>
            new Temperature(Value + rhs.Value);

        public Temperature Subtract(Temperature rhs) =>
            new Temperature(Value - rhs.Value);

        public Temperature Multiply(double rhs) =>
            new Temperature(Value * rhs);

        public Temperature Divide(double rhs) =>
            new Temperature(Value / rhs);

        public static Temperature operator *(Temperature lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static Temperature operator *(double lhs, Temperature rhs) =>
            rhs.Multiply(lhs);

        public static Temperature operator +(Temperature lhs, Temperature rhs) =>
            lhs.Add(rhs);

        public static Temperature operator -(Temperature lhs, Temperature rhs) =>
            lhs.Subtract(rhs);

        public static Temperature operator /(Temperature lhs, double rhs) =>
            lhs.Divide(rhs);

        public static double operator /(Temperature lhs, Temperature rhs) =>
            lhs.Value / rhs.Value;

        public static bool operator ==(Temperature lhs, Temperature rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Temperature lhs, Temperature rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(Temperature lhs, Temperature rhs) =>
            lhs.Value > rhs.Value;

        public static bool operator <(Temperature lhs, Temperature rhs) =>
            lhs.Value < rhs.Value;

        public static bool operator >=(Temperature lhs, Temperature rhs) =>
            lhs.Value >= rhs.Value;

        public static bool operator <=(Temperature lhs, Temperature rhs) =>
            lhs.Value <= rhs.Value;

        public Temperature Pow(double power) =>
            new Temperature(Math.Pow(Value, power));

        public Temperature Round() =>
            new Temperature(Math.Round(Value));

        public Temperature Sqrt() =>
            new Temperature(Math.Sqrt(Value));

        public Temperature Abs() =>
            new Temperature(Math.Abs(Value));

        public Temperature Min(Temperature rhs) =>
            new Temperature(Math.Min(Value, rhs.Value));

        public Temperature Max(Temperature rhs) =>
            new Temperature(Math.Max(Value, rhs.Value));

        public double Kelvin => Value;

        public double Celsius => Value - 273.15;

        public double Fahrenheit => (9 * Celsius / 5) + 32;
    }

    public static class UnitsTemperatureExtensions
    {
        public static Temperature Kelvin(this int self) =>
            new Temperature(self);

        public static Temperature Kelvin(this float self) =>
            new Temperature(self);

        public static Temperature Kelvin(this double self) =>
            new Temperature(self);

        public static Temperature Celsius(this int self) =>
            new Temperature(self + 273.15);

        public static Temperature Celsius(this float self) =>
            new Temperature(self + 273.15);

        public static Temperature Celsius(this double self) =>
            new Temperature(self + 273.15);

        public static Temperature Fahrenheit(this int self) =>
            new Temperature(5 * (self - 32) / 9);

        public static Temperature Fahrenheit(this float self) =>
            new Temperature(5 * (self - 32) / 9);

        public static Temperature Fahrenheit(this double self) =>
            new Temperature(5 * (self - 32) / 9);
    }
}
