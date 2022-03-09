using System;
using static LanguageExt.Prelude;

namespace LanguageExt.UnitsOfMeasure
{
    /// <summary>
    /// Numeric velocity value
    /// Handles unit conversions automatically
    /// Internally all speeds are stored as metres per second
    /// All standard arithmetic operators work on the Velocity
    /// type.  So keep all velocities wrapped until you need the
    /// value, then extract using various unit-of-measure
    /// accessors (MetresPerSecond, etc.) or divide by 1.MetrePerSecond()
    /// </summary>
    public readonly struct Velocity :
        IComparable<Velocity>,
        IEquatable<Velocity>,
        IComparable
    {
        readonly double Value;

        internal Velocity(double value) =>
            Value = value;

        public override string ToString() =>
             $"{Value} m/s";

        public bool Equals(Velocity other) =>
            Value.Equals(other.Value);

        public bool Equals(Velocity other, double epsilon) =>
            Math.Abs(other.Value - Value) < epsilon;

        public override bool Equals(object obj) =>
            obj == null
                ? false
                : obj is Velocity
                    ? Equals((Velocity)obj)
                    : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(object obj) =>
            obj is null ? 1
            : obj is Velocity other ? CompareTo(other)
            : throw new ArgumentException($"must be of type {nameof(Velocity)}");

        public int CompareTo(Velocity other) =>
            Value.CompareTo(other.Value);

        public Velocity Add(Velocity rhs) =>
            new Velocity(Value + rhs.Value);

        public Velocity Subtract(Velocity rhs) =>
            new Velocity(Value - rhs.Value);

        public Velocity Multiply(double rhs) =>
            new Velocity(Value * rhs);

        public Velocity Divide(double rhs) =>
            new Velocity(Value / rhs);

        public static Velocity operator *(Velocity lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static Velocity operator *(double lhs, Velocity rhs) =>
            rhs.Multiply(lhs);

        public static Length operator *(Velocity lhs, Time rhs) =>
            new Length(lhs.Value * rhs.Seconds);

        public static Length operator *(Time lhs, Velocity rhs) =>
            new Length(lhs.Seconds * rhs.Value);

        public static VelocitySq operator *(Velocity lhs, Velocity rhs) =>
            new VelocitySq(lhs.Value * rhs.Value);

        public static VelocitySq operator^(Velocity lhs, int power) =>
            power == 2
                ? new VelocitySq(lhs.Value * lhs.Value)
                : raise<VelocitySq>(new NotSupportedException("Velocity can only be raised to the power of 2"));

        public static Velocity operator +(Velocity lhs, Velocity rhs) =>
            lhs.Add(rhs);

        public static Velocity operator -(Velocity lhs, Velocity rhs) =>
            lhs.Subtract(rhs);

        public static Velocity operator /(Velocity lhs, double rhs) =>
            lhs.Divide(rhs);

        public static double operator /(Velocity lhs, Velocity rhs) =>
            lhs.Value / rhs.Value;

        public static Accel operator /(Velocity lhs, Time rhs) =>
            new Accel(lhs.Value / rhs.Seconds);

        public static Time operator /(Velocity lhs, Accel rhs) =>
            new Time(lhs.Value / rhs.MetresPerSecond2);

        public static bool operator ==(Velocity lhs, Velocity rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Velocity lhs, Velocity rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(Velocity lhs, Velocity rhs) =>
            lhs.Value > rhs.Value;

        public static bool operator <(Velocity lhs, Velocity rhs) =>
            lhs.Value < rhs.Value;

        public static bool operator >=(Velocity lhs, Velocity rhs) =>
            lhs.Value >= rhs.Value;

        public static bool operator <=(Velocity lhs, Velocity rhs) =>
            lhs.Value <= rhs.Value;

        public Velocity Round() =>
            new Velocity(Math.Round(Value));

        public Velocity Abs() =>
            new Velocity(Math.Abs(Value));

        public Velocity Min(Velocity rhs) =>
            new Velocity(Math.Min(Value, rhs.Value));

        public Velocity Max(Velocity rhs) =>
            new Velocity(Math.Max(Value, rhs.Value));

        public double MetresPerSecond     => Value;
        public double KilometresPerSecond => Value / 1000.0;
        public double KilometresPerHour   => Value / 1000.0 * 3600.0;
        public double MilesPerSecond      => Value / 1609.344000006437376000025749504;
        public double MilesPerHour        => Value / 1609.344000006437376000025749504 * 3600.0;
    }

    public static class UnitsVelocityExtensions
    {
        public static Velocity MetresPerSecond(this int self) =>
            new Velocity(self);

        public static Velocity MetresPerSecond(this float self) =>
            new Velocity(self);

        public static Velocity MetresPerSecond(this double self) =>
            new Velocity(self);

        public static Velocity KilometresPerSecond(this int self) =>
            new Velocity(self * 1000.0);

        public static Velocity KilometresPerSecond(this float self) =>
            new Velocity(self * 1000.0);

        public static Velocity KilometresPerSecond(this double self) =>
            new Velocity(self * 1000.0);

        public static Velocity KilometresPerHour(this int self) =>
            new Velocity(self * 1000.0 / 3600.0);

        public static Velocity KilometresPerHour(this float self) =>
            new Velocity(self * 1000.0 / 3600.0);

        public static Velocity KilometresPerHour(this double self) =>
            new Velocity(self * 1000.0 / 3600.0);

        public static Velocity MilesPerSecond(this int self) =>
            new Velocity(self * 1609.344000006437376000025749504);

        public static Velocity MilesPerSecond(this float self) =>
            new Velocity(self * 1609.344000006437376000025749504);

        public static Velocity MilesPerSecond(this double self) =>
            new Velocity(self * 1609.344000006437376000025749504);

        public static Velocity MilesPerHour(this int self) =>
            new Velocity(self * 1609.344000006437376000025749504 / 3600.0);

        public static Velocity MilesPerHour(this float self) =>
            new Velocity(self * 1609.344000006437376000025749504 / 3600.0);

        public static Velocity MilesPerHour(this double self) =>
            new Velocity(self * 1609.344000006437376000025749504 / 3600.0);
    }
}
