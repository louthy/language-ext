using System;
using static LanguageExt.Prelude;

namespace LanguageExt.UnitsOfMeasure
{
    /// <summary>
    /// Numeric VelocitySquared value
    /// Handles unit conversions automatically
    /// </summary>
    public struct VelocitySq :
        IComparable<VelocitySq>,
        IEquatable<VelocitySq>
    {
        readonly double Value;

        internal VelocitySq(double length)
        {
            Value = length;
        }

        public override string ToString() =>
            Value + " m/s²";

        public bool Equals(VelocitySq other) =>
            Value.Equals(other.Value);

        public bool Equals(VelocitySq other, double epsilon) =>
            Math.Abs(other.Value - Value) < epsilon;

        public override bool Equals(object obj) =>
            obj == null
                ? false
                : obj is Length
                    ? Equals((Length)obj)
                    : false;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(VelocitySq other) =>
            Value.CompareTo(other.Value);

        public VelocitySq Append(VelocitySq rhs) =>
            new VelocitySq(Value + rhs.Value);

        public VelocitySq Subtract(VelocitySq rhs) =>
            new VelocitySq(Value - rhs.Value);

        public VelocitySq Multiply(double rhs) =>
            new VelocitySq(Value * rhs);

        public VelocitySq Divide(double rhs) =>
            new VelocitySq(Value / rhs);

        public static VelocitySq operator *(VelocitySq lhs, double rhs) =>
            lhs.Multiply(rhs);

        public static VelocitySq operator *(double lhs, VelocitySq rhs) =>
            rhs.Multiply(lhs);

        public static VelocitySq operator +(VelocitySq lhs, VelocitySq rhs) =>
            lhs.Append(rhs);

        public static VelocitySq operator -(VelocitySq lhs, VelocitySq rhs) =>
            lhs.Subtract(rhs);

        public static VelocitySq operator /(VelocitySq lhs, double rhs) =>
            lhs.Divide(rhs);

        public static double operator /(VelocitySq lhs, VelocitySq rhs) =>
            lhs.Value / rhs.Value;

        public static bool operator ==(VelocitySq lhs, VelocitySq rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(VelocitySq lhs, VelocitySq rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(VelocitySq lhs, VelocitySq rhs) =>
            lhs.Value > rhs.Value;

        public static bool operator <(VelocitySq lhs, VelocitySq rhs) =>
            lhs.Value < rhs.Value;

        public static bool operator >=(VelocitySq lhs, VelocitySq rhs) =>
            lhs.Value >= rhs.Value;

        public static bool operator <=(VelocitySq lhs, VelocitySq rhs) =>
            lhs.Value <= rhs.Value;

        public Velocity Sqrt() =>
            new Velocity(Math.Sqrt(Value));

        public VelocitySq Round() =>
            new VelocitySq(Math.Round(Value));

        public VelocitySq Abs() =>
            new VelocitySq(Math.Abs(Value));

        public VelocitySq Min(VelocitySq rhs) =>
            new VelocitySq(Math.Min(Value, rhs.Value));

        public VelocitySq Max(VelocitySq rhs) =>
            new VelocitySq(Math.Max(Value, rhs.Value));

        public double MetresPerSecond2 => Value;
    }
}
