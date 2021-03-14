using System;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt.UnitsOfMeasure
{
    public readonly struct Temperature :
        IComparable<Temperature>,
        IEquatable<Temperature>,
        IComparable
    {
        internal enum UnitType
        {
            K, C, F
        }

        readonly UnitType Type;
        readonly double Value;

        public static Temperature AbsoluteZero = default(Temperature);
        public static Temperature ZeroCelsius = new Temperature(UnitType.C, 0.0);
        public static Temperature ZeroFahrenheit = new Temperature(UnitType.F, 0.0);

        internal Temperature(UnitType type, double value)
        {
            Type = type;
            Value = value;

            if (this < AbsoluteZero) throw new ArgumentOutOfRangeException(nameof(value), $"{value} [{type}]", "Less than absolute zero");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double CtoK(double x) => x + 273.15;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double KtoC(double x) => x - 273.15;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double FtoK(double x) => (x + 459.67) * 5.0 / 9.0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double KtoF(double x) => (x * 1.8) - 459.67;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double CtoF(double x) => (x * 1.8) + 32.0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double FtoC(double x) => (x - 32.0) * 5.0 / 9.0;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public override bool Equals(object obj) =>
            notnull(obj) && obj is Temperature t && Equals(t);

        public bool Equals(Temperature rhs) =>
            Value.Equals(rhs.Value);

        public override string ToString() =>
            Type == UnitType.K ? $"{Value} K"
          : Type == UnitType.C ? $"{Value} °C"
          : Type == UnitType.F ? $"{Value} °F"
          : throw new NotSupportedException(Type.ToString());

        public Temperature Kelvin =>
            Type == UnitType.K ? this
          : Type == UnitType.C ? new Temperature(UnitType.K, CtoK(Value))
          : Type == UnitType.F ? new Temperature(UnitType.K, FtoK(Value))
          : throw new NotSupportedException(Type.ToString());

        public double KValue =>
            Type == UnitType.K ? Value
          : Type == UnitType.C ? CtoK(Value)
          : Type == UnitType.F ? FtoK(Value)
          : throw new NotSupportedException(Type.ToString());

        public Temperature Celsius =>
            Type == UnitType.K ? new Temperature(UnitType.C, KtoC(Value))
          : Type == UnitType.C ? this
          : Type == UnitType.F ? new Temperature(UnitType.C, FtoC(Value))
          : throw new NotSupportedException(Type.ToString());

        public Temperature Fahrenheit =>
            Type == UnitType.K ? new Temperature(UnitType.F, KtoF(Value))
          : Type == UnitType.C ? new Temperature(UnitType.F, CtoF(Value))
          : Type == UnitType.F ? this
          : throw new NotSupportedException(Type.ToString());

        public bool Equals(Temperature rhs, double epsilon) =>
            Type == UnitType.K ?
                rhs.Type == UnitType.K ? Math.Abs(rhs.Value - Value) < epsilon
              : rhs.Type == UnitType.C ? Math.Abs(CtoK(rhs.Value) - Value) < epsilon
              : rhs.Type == UnitType.F ? Math.Abs(FtoK(rhs.Value) - Value) < epsilon
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.C ?
                rhs.Type == UnitType.K ? Math.Abs(KtoC(rhs.Value) - Value) < epsilon
              : rhs.Type == UnitType.C ? Math.Abs(rhs.Value - Value) < epsilon
              : rhs.Type == UnitType.F ? Math.Abs(FtoC(rhs.Value) - Value) < epsilon
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.F ?
                rhs.Type == UnitType.K ? Math.Abs(KtoF(rhs.Value) - Value) < epsilon
              : rhs.Type == UnitType.C ? Math.Abs(CtoF(rhs.Value) - Value) < epsilon
              : rhs.Type == UnitType.F ? Math.Abs(rhs.Value - Value) < epsilon
              : throw new NotSupportedException(Type.ToString())
          : throw new NotSupportedException(Type.ToString());

        public int CompareTo(object obj) =>
            obj is null ? 1
            : obj is Temperature other ? CompareTo(other)
            : throw new ArgumentException($"must be of type {nameof(Temperature)}");

        public int CompareTo(Temperature rhs) =>
            Type == UnitType.K ?
                rhs.Type == UnitType.K ? Value.CompareTo(rhs.Value)
              : rhs.Type == UnitType.C ? Value.CompareTo(CtoK(rhs.Value))
              : rhs.Type == UnitType.F ? Value.CompareTo(FtoK(rhs.Value))
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.C ?
                rhs.Type == UnitType.K ? Value.CompareTo(KtoC(rhs.Value))
              : rhs.Type == UnitType.C ? Value.CompareTo(rhs.Value)
              : rhs.Type == UnitType.F ? Value.CompareTo(FtoC(rhs.Value))
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.F ?
                rhs.Type == UnitType.K ? Value.CompareTo(KtoF(rhs.Value))
              : rhs.Type == UnitType.C ? Value.CompareTo(CtoF(rhs.Value))
              : rhs.Type == UnitType.F ? Value.CompareTo(rhs.Value)
              : throw new NotSupportedException(Type.ToString())
          : throw new NotSupportedException(Type.ToString());

        public Temperature Add(Temperature rhs) =>
            Type == UnitType.K ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.K, Value + rhs.Value)
              : rhs.Type == UnitType.C ? new Temperature(UnitType.K, Value + CtoK(rhs.Value))
              : rhs.Type == UnitType.F ? new Temperature(UnitType.K, Value + FtoK(rhs.Value))
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.C ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.C, Value + KtoC(rhs.Value))
              : rhs.Type == UnitType.C ? new Temperature(UnitType.C, Value + rhs.Value)
              : rhs.Type == UnitType.F ? new Temperature(UnitType.C, Value + FtoC(rhs.Value))
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.F ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.F, Value + KtoF(rhs.Value))
              : rhs.Type == UnitType.C ? new Temperature(UnitType.F, Value + CtoF(rhs.Value))
              : rhs.Type == UnitType.F ? new Temperature(UnitType.F, Value + rhs.Value)
              : throw new NotSupportedException(Type.ToString())
          : throw new NotSupportedException(Type.ToString()); 

        public Temperature Subtract(Temperature rhs) =>
            Type == UnitType.K ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.K, Value - rhs.Value)
              : rhs.Type == UnitType.C ? new Temperature(UnitType.K, Value - CtoK(rhs.Value))
              : rhs.Type == UnitType.F ? new Temperature(UnitType.K, Value - FtoK(rhs.Value))
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.C ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.C, Value - KtoC(rhs.Value))
              : rhs.Type == UnitType.C ? new Temperature(UnitType.C, Value - rhs.Value)
              : rhs.Type == UnitType.F ? new Temperature(UnitType.C, Value - FtoC(rhs.Value))
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.F ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.F, Value - KtoF(rhs.Value))
              : rhs.Type == UnitType.C ? new Temperature(UnitType.F, Value - CtoF(rhs.Value))
              : rhs.Type == UnitType.F ? new Temperature(UnitType.F, Value - rhs.Value)
              : throw new NotSupportedException(Type.ToString())
          : throw new NotSupportedException(Type.ToString());

        public Temperature Multiply(double rhs) =>
            new Temperature(Type, Value * rhs);

        public Temperature Divide(double rhs) =>
            new Temperature(Type, Value / rhs);

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

        public static bool operator ==(Temperature lhs, Temperature rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Temperature lhs, Temperature rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(Temperature lhs, Temperature rhs) =>
            lhs.CompareTo(rhs) > 0;

        public static bool operator <(Temperature lhs, Temperature rhs) =>
            lhs.CompareTo(rhs) < 0;

        public static bool operator >=(Temperature lhs, Temperature rhs) =>
            lhs.CompareTo(rhs) >= 0;

        public static bool operator <=(Temperature lhs, Temperature rhs) =>
            lhs.CompareTo(rhs) <= 0;

        public Temperature Round() =>
            new Temperature(Type, Math.Round(Value));

        public Temperature Abs() =>
            new Temperature(Type, Math.Abs(Value));

        public Temperature Min(Temperature rhs) =>
            Type == UnitType.K ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.K, Math.Min(Value, rhs.Value))
              : rhs.Type == UnitType.C ? new Temperature(UnitType.K, Math.Min(Value, CtoK(rhs.Value)))
              : rhs.Type == UnitType.F ? new Temperature(UnitType.K, Math.Min(Value, FtoK(rhs.Value)))
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.C ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.C, Math.Min(Value, KtoC(rhs.Value)))
              : rhs.Type == UnitType.C ? new Temperature(UnitType.C, Math.Min(Value, rhs.Value))
              : rhs.Type == UnitType.F ? new Temperature(UnitType.C, Math.Min(Value, FtoC(rhs.Value)))
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.F ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.F, Math.Min(Value, KtoF(rhs.Value)))
              : rhs.Type == UnitType.C ? new Temperature(UnitType.F, Math.Min(Value, CtoF(rhs.Value)))
              : rhs.Type == UnitType.F ? new Temperature(UnitType.F, Math.Min(Value, rhs.Value))
              : throw new NotSupportedException(Type.ToString())
          : throw new NotSupportedException(Type.ToString());

        public Temperature Max(Temperature rhs) =>
            Type == UnitType.K ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.K, Math.Max(Value, rhs.Value))
              : rhs.Type == UnitType.C ? new Temperature(UnitType.K, Math.Max(Value, CtoK(rhs.Value)))
              : rhs.Type == UnitType.F ? new Temperature(UnitType.K, Math.Max(Value, FtoK(rhs.Value)))
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.C ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.C, Math.Max(Value, KtoC(rhs.Value)))
              : rhs.Type == UnitType.C ? new Temperature(UnitType.C, Math.Max(Value, rhs.Value))
              : rhs.Type == UnitType.F ? new Temperature(UnitType.C, Math.Max(Value, FtoC(rhs.Value)))
              : throw new NotSupportedException(Type.ToString())
          : Type == UnitType.F ?
                rhs.Type == UnitType.K ? new Temperature(UnitType.F, Math.Max(Value, KtoF(rhs.Value)))
              : rhs.Type == UnitType.C ? new Temperature(UnitType.F, Math.Max(Value, CtoF(rhs.Value)))
              : rhs.Type == UnitType.F ? new Temperature(UnitType.F, Math.Max(Value, rhs.Value))
              : throw new NotSupportedException(Type.ToString())
          : throw new NotSupportedException(Type.ToString());
    }

    public static class UnitsTemperatureExtensions
    {
        public static Temperature Celsius(this int self) =>
            new Temperature(Temperature.UnitType.C, self);

        public static Temperature Celsius(this float self) =>
            new Temperature(Temperature.UnitType.C, self);

        public static Temperature Celsius(this double self) =>
            new Temperature(Temperature.UnitType.C, self);

        public static Temperature Fahrenheit(this int self) =>
            new Temperature(Temperature.UnitType.F, self);

        public static Temperature Fahrenheit(this float self) =>
            new Temperature(Temperature.UnitType.F, self);

        public static Temperature Fahrenheit(this double self) =>
            new Temperature(Temperature.UnitType.F, self);

        public static Temperature Kelvin(this int self) =>
            new Temperature(Temperature.UnitType.K, self);

        public static Temperature Kelvin(this float self) =>
            new Temperature(Temperature.UnitType.K, self);

        public static Temperature Kelvin(this double self) =>
            new Temperature(Temperature.UnitType.K, self);
    }
}
