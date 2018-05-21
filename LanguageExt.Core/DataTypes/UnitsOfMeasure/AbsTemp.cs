using System;

namespace LanguageExt.UnitsOfMeasure
{
    /// <summary>
    /// Numeric thermodynamic temperature value
    /// Internally all temperatures are stored as Kelvin
    /// All standard arithmetic operators work on the AbsTemp
    /// type.  So keep all AbsTemps wrapped until you need the
    /// value, then extract using various unit-of-measure
    /// accessors (Celsius, etc.) or divide by 1.Kelvin()
    /// </summary>
    public struct AbsTemp :
        IComparable<AbsTemp>,
        IEquatable<AbsTemp>
    {
        readonly double Value;

        internal AbsTemp(double absTemp)
        {
            if (absTemp < 0.0) throw new ArgumentOutOfRangeException(nameof(absTemp));
            Value = absTemp;
        }

        public override string ToString() =>
            Value + " K";

        public bool Equals(AbsTemp other) =>
            Value.Equals(other.Value);

        public bool Equals(AbsTemp other, double epsilon) =>
            Math.Abs(other.Value - Value) < epsilon;

        public bool Equals(ScaleTemp other) =>
            Value.Equals(other.Kelvin);

        public override bool Equals(object obj) =>
            obj is AbsTemp t
                ? Equals(t)
                : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(AbsTemp other) =>
            Value.CompareTo(other.Value);

        public AbsTemp Add(AbsTemp rhs) =>
            new AbsTemp(Value + rhs.Value);

        public AbsTemp Subtract(AbsTemp rhs) =>
            new AbsTemp(Value - rhs.Value);

        public AbsTemp Multiply(double rhs) =>
            new AbsTemp(Value * rhs);

        public AbsTemp Divide(double rhs) =>
            new AbsTemp(Value / rhs);

        public static AbsTemp operator *(AbsTemp lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static AbsTemp operator *(double lhs, AbsTemp rhs) =>
            rhs.Multiply(lhs);

        public static AbsTemp operator +(AbsTemp lhs, AbsTemp rhs) =>
            lhs.Add(rhs);

        public static AbsTemp operator -(AbsTemp lhs, AbsTemp rhs) =>
            lhs.Subtract(rhs);

        public static AbsTemp operator /(AbsTemp lhs, double rhs) =>
            lhs.Divide(rhs);

        public static double operator /(AbsTemp lhs, AbsTemp rhs) =>
            lhs.Value / rhs.Value;

        public static bool operator ==(AbsTemp lhs, AbsTemp rhs) =>
            lhs.Equals(rhs);

        public static bool operator ==(AbsTemp lhs, ScaleTemp rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(AbsTemp lhs, AbsTemp rhs) =>
            !lhs.Equals(rhs);

        public static bool operator !=(AbsTemp lhs, ScaleTemp rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(AbsTemp lhs, AbsTemp rhs) =>
            lhs.Value > rhs.Value;

        public static bool operator >(AbsTemp lhs, ScaleTemp rhs) =>
            lhs.Value > rhs.Kelvin;

        public static bool operator <(AbsTemp lhs, AbsTemp rhs) =>
            lhs.Value < rhs.Value;

        public static bool operator <(AbsTemp lhs, ScaleTemp rhs) =>
            lhs.Value < rhs.Kelvin;

        public static bool operator >=(AbsTemp lhs, AbsTemp rhs) =>
            lhs.Value >= rhs.Value;

        public static bool operator >=(AbsTemp lhs, ScaleTemp rhs) =>
            lhs.Value >= rhs.Kelvin;

        public static bool operator <=(AbsTemp lhs, AbsTemp rhs) =>
            lhs.Value <= rhs.Value;

        public static bool operator <=(AbsTemp lhs, ScaleTemp rhs) =>
            lhs.Value <= rhs.Kelvin;

        public AbsTemp Pow(double power) =>
            new AbsTemp(Math.Pow(Value, power));

        public AbsTemp Round() =>
            new AbsTemp(Math.Round(Value));

        public AbsTemp Sqrt() =>
            new AbsTemp(Math.Sqrt(Value));

        public AbsTemp Min(AbsTemp rhs) =>
            new AbsTemp(Math.Min(Value, rhs.Value));

        public AbsTemp Max(AbsTemp rhs) =>
            new AbsTemp(Math.Max(Value, rhs.Value));

        public double Kelvin => Value;

        public double Celsius => Value.KToDegC();

        public double Fahrenheit => Value.KToDegF();
    }

    public static class UnitsAbsTempExtensions
    {
        internal const double AbsZeroInDegC = -273.15;

        internal const double AbsZeroInDegF = -459.67;

        internal static double KToDegC(this double K) => 
            K + AbsZeroInDegC;

        internal static double DegCToK(this double degC) => 
            degC - AbsZeroInDegC;

        internal static double KToDegF(this double K) => 
            9.0 * K / 5.0 + AbsZeroInDegF;

        internal static double DegFToK(this double degF) => 
            (degF - AbsZeroInDegF) * 5.0 / 9.0;

        public static AbsTemp Kelvin(this int self) =>
            new AbsTemp(self);

        public static AbsTemp Kelvin(this float self) =>
            new AbsTemp(self);

        public static AbsTemp Kelvin(this double self) =>
            new AbsTemp(self);
    }
}
