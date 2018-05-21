using System;
using static LanguageExt.Prelude;

namespace LanguageExt.UnitsOfMeasure
{
    /// <summary>
    /// Numeric temperature scale value
    /// Internally all temperatures are stored as Celsius
    /// All standard arithmetic operators work on the ScaleTemp
    /// type.  So keep all ScaleTemps wrapped until you need the
    /// value, then extract using various unit-of-measure
    /// accessors (Kelvin, etc.) or divide by 1.Celsius()
    /// </summary>
    public struct ScaleTemp :
        IComparable<ScaleTemp>,
        IEquatable<ScaleTemp>
    {
        readonly double Value;

        internal ScaleTemp(double scaleTemp)
        {
            if (scaleTemp < UnitsAbsTempExtensions.AbsZeroInDegC)
                throw new ArgumentOutOfRangeException(nameof(scaleTemp));

            Value = scaleTemp;
        }

        public override string ToString() =>
            Value + " °C";

        public bool Equals(ScaleTemp other) =>
            Value.Equals(other.Value);

        public bool Equals(ScaleTemp other, double epsilon) =>
            Math.Abs(other.Value - Value) < epsilon;

        public bool Equals(AbsTemp other) =>
            Value.Equals(other.Celsius);

        public override bool Equals(object obj) =>
            obj is ScaleTemp st
            ? Equals(st)
            : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(ScaleTemp other) =>
            Value.CompareTo(other.Value);

        public ScaleTemp Append(ScaleTemp rhs) =>
            new ScaleTemp(Value + rhs.Value);

        public ScaleTemp Subtract(ScaleTemp rhs) =>
            new ScaleTemp(Value - rhs.Value);

        public ScaleTemp Multiply(double rhs) =>
            new ScaleTemp(Value * rhs);

        public ScaleTemp Divide(double rhs) =>
            new ScaleTemp(Value / rhs);

        public static ScaleTemp operator *(ScaleTemp lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static ScaleTemp operator *(double lhs, ScaleTemp rhs) =>
            rhs.Multiply(lhs);

        public static ScaleTemp operator +(ScaleTemp lhs, ScaleTemp rhs) =>
            lhs.Append(rhs);

        public static ScaleTemp operator -(ScaleTemp lhs, ScaleTemp rhs) =>
            lhs.Subtract(rhs);

        public static ScaleTemp operator /(ScaleTemp lhs, double rhs) =>
            lhs.Divide(rhs);

        public static double operator /(ScaleTemp lhs, ScaleTemp rhs) =>
            lhs.Value / rhs.Value;

        public static bool operator ==(ScaleTemp lhs, ScaleTemp rhs) =>
            lhs.Equals(rhs);

        public static bool operator ==(ScaleTemp lhs, AbsTemp rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(ScaleTemp lhs, ScaleTemp rhs) =>
            !lhs.Equals(rhs);

        public static bool operator !=(ScaleTemp lhs, AbsTemp rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(ScaleTemp lhs, ScaleTemp rhs) =>
            lhs.Value > rhs.Value;

        public static bool operator >(ScaleTemp lhs, AbsTemp rhs) =>
            lhs.Celsius > rhs.Celsius;

        public static bool operator <(ScaleTemp lhs, ScaleTemp rhs) =>
            lhs.Value < rhs.Value;

        public static bool operator <(ScaleTemp lhs, AbsTemp rhs) =>
            lhs.Celsius < rhs.Celsius;

        public static bool operator >=(ScaleTemp lhs, ScaleTemp rhs) =>
            lhs.Value >= rhs.Value;

        public static bool operator >=(ScaleTemp lhs, AbsTemp rhs) =>
            lhs.Celsius >= rhs.Celsius;

        public static bool operator <=(ScaleTemp lhs, ScaleTemp rhs) =>
            lhs.Value <= rhs.Value;

        public static bool operator <=(ScaleTemp lhs, AbsTemp rhs) =>
            lhs.Celsius <= rhs.Celsius;

        public ScaleTemp Pow(double power) =>
            new ScaleTemp(Math.Pow(Value, power));

        public ScaleTemp Round() =>
            new ScaleTemp(Math.Round(Value));

        public ScaleTemp Sqrt() =>
            new ScaleTemp(Math.Sqrt(Value));

        public ScaleTemp Abs() =>
            new ScaleTemp(Math.Abs(Value));

        public ScaleTemp Min(ScaleTemp rhs) =>
            new ScaleTemp(Math.Min(Value, rhs.Value));

        public ScaleTemp Max(ScaleTemp rhs) =>
            new ScaleTemp(Math.Max(Value, rhs.Value));

        public double Celsius => Value;

        public double Kelvin => Value.DegCToK();

        public double Fahrenheit => Value.DegCToDegF();
    }

    public static class UnitsScaleTempExtensions
    {
        internal const double MptIceInDegF = 32.0;

        internal static double DegCToDegF(this double degC) =>
            9.0 * degC / 5.0 + MptIceInDegF;

        internal static double DegFToDegC(this double degF) =>
            (degF - MptIceInDegF) * 5.0 / 9.0;

        public static ScaleTemp Celsius(this int self) =>
            new ScaleTemp(self);

        public static ScaleTemp Celsius(this float self) =>
            new ScaleTemp(self);

        public static ScaleTemp Celsius(this double self) =>
            new ScaleTemp(self);

        public static ScaleTemp Fahrenheit(this int self) =>
            new ScaleTemp(DegFToDegC(self));

        public static ScaleTemp Fahrenheit(this float self) =>
            new ScaleTemp(DegFToDegC(self));

        public static ScaleTemp Fahrenheit(this double self) =>
            new ScaleTemp(DegFToDegC(self));
    }
}
