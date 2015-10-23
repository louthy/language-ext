using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
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
    [Serializable]
    public struct Velocity :
        IAppendable<Velocity>,
        ISubtractable<Velocity>,
        IComparable<Velocity>,
        IEquatable<Velocity>
    {
        readonly double Value;

        internal Velocity(double length)
        {
            Value = length;
        }

        public override string ToString() =>
            Value + "m/s";

        public bool Equals(Velocity other) =>
            Value.Equals(other.Value);

        public bool Equals(Velocity other, double epsilon) =>
            Math.Abs(other.Value - Value) < epsilon;

        public override bool Equals(object obj) =>
            obj == null
                ? false
                : obj is Length
                    ? Equals((Length)obj)
                    : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(Velocity other) =>
            Value.CompareTo(other.Value);

        public Velocity Append(Velocity rhs) =>
            new Velocity(Value + rhs.Value);

        public Velocity Subtract(Velocity rhs) =>
            new Velocity(Value - rhs.Value);

        public Velocity Product(double rhs) =>
            new Velocity(Value * rhs);

        public Velocity Divide(double rhs) =>
            new Velocity(Value / rhs);

        public static Velocity operator *(Velocity lhs, double rhs) =>
            lhs.Product(rhs);

        public static Velocity operator *(double lhs, Velocity rhs) =>
            rhs.Product(lhs);

        public static Length operator *(Velocity lhs, Time rhs) =>
            new Length(lhs.Value * rhs.Seconds);

        public static Velocity operator +(Velocity lhs, Velocity rhs) =>
            lhs.Append(rhs);

        public static Velocity operator -(Velocity lhs, Velocity rhs) =>
            lhs.Subtract(rhs);

        public static Velocity operator /(Velocity lhs, double rhs) =>
            lhs.Divide(rhs);

        public static double operator /(Velocity lhs, Velocity rhs) =>
            lhs.Value / rhs.Value;

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

        public Velocity Pow(double power) =>
            new Velocity(Math.Pow(Value,power));

        public Velocity Round() =>
            new Velocity(Math.Round(Value));

        public Velocity Sqrt() =>
            new Velocity(Math.Sqrt(Value));

        public Velocity Abs() =>
            new Velocity(Math.Abs(Value));

        public Velocity Min(Velocity rhs) =>
            new Velocity(Math.Min(Value, rhs.Value));

        public Velocity Max(Velocity rhs) =>
            new Velocity(Math.Max(Value, rhs.Value));

        public double MetresPerSecond     => Value;
        public double KilometresPerSecond => Value / 1000.0;
        public double KilometresPerHour   => Value / 1000.0 / 3600.0;
        public double MilesPerSecond      => Value / 1609.344000006437376000025749504;
        public double MilesPerHour        => Value / 1609.344000006437376000025749504 / 3600.0;
    }

    namespace UnitsOfMeasure
    {
        public static class __UnitsVelocityExt
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
                new Velocity(self * 1000.0 * 3600.0);

            public static Velocity KilometresPerHour(this float self) =>
                new Velocity(self * 1000.0 * 3600.0);

            public static Velocity KilometresPerHour(this double self) =>
                new Velocity(self * 1000.0 * 3600.0);

            public static Velocity MilesPerSecond(this int self) =>
                new Velocity(self * 1609.344000006437376000025749504);

            public static Velocity MilesPerSecond(this float self) =>
                new Velocity(self * 1609.344000006437376000025749504);

            public static Velocity MilesPerSecond(this double self) =>
                new Velocity(self * 1609.344000006437376000025749504);

            public static Velocity MilesPerHour(this int self) =>
                new Velocity(self * 1609.344000006437376000025749504 * 3600.0);

            public static Velocity MilesPerHour(this float self) =>
                new Velocity(self * 1609.344000006437376000025749504 * 3600.0);

            public static Velocity MilesPerHour(this double self) =>
                new Velocity(self * 1609.344000006437376000025749504 * 3600.0);
        }
    }
}
