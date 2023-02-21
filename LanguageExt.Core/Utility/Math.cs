#nullable enable

using System.Runtime.InteropServices;
using static System.BitConverter;

namespace LanguageExt;

/// <summary>
/// Provides implementations of math functions missing from the standard library 
/// </summary>
public static class MathExt
{
    /// <summary>
    /// Gets the floating-point number that is next after <paramref name="fromNumber"/> in the direction of <paramref name="towardNumber"/>
    /// </summary>
    /// <remarks>See https://github.com/MachineCognitis/C.math.NET/blob/master/C.math/math.cs#L1695</remarks>
    /// <param name="fromNumber">A floating-point number</param>
    /// <param name="towardNumber">A floating-point number</param>
    /// <returns>The floating-point number that is next after <paramref name="fromNumber"/> in the direction of <paramref name="towardNumber"/></returns>
    public static double NextAfter(double fromNumber, double towardNumber)
    {
        // If either fromNumber or towardNumber is NaN, return NaN.
        if (double.IsNaN(towardNumber) || double.IsNaN(fromNumber))
            return double.NaN;

        // If no direction.
        if (fromNumber == towardNumber)
            return towardNumber;

        // If fromNumber is zero, return smallest subnormal.
        if (fromNumber == 0)
            return towardNumber > 0 ? double.Epsilon : -double.Epsilon;

        // All other cases are handled by incrementing or decrementing the bits value.
        // Transitions to infinity, to subnormal, and to zero are all taken care of this way.
        var bits = DoubleToInt64Bits(fromNumber);
        // A xor here avoids nesting conditionals. We have to increment if fromValue lies between 0 and toValue.
        if ((fromNumber > 0) ^ (fromNumber > towardNumber))
            bits += 1;
        else
            bits -= 1;

        return Int64BitsToDouble(bits);
    }

    /// <summary>
    /// For more information see https://stackoverflow.com/a/59273138
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    struct FloatToInt 
    {
        [FieldOffset(0)]private float f;
        [FieldOffset(0)]private int i;
        public static int Convert(float value) =>
            new FloatToInt { f = value }.i;
        public static float Convert(int value) =>
            new FloatToInt { i = value }.f;
    }
    
    /// <summary>
    /// Gets the floating-point number that is next after <paramref name="fromNumber"/> in the direction of <paramref name="towardNumber"/>
    /// </summary>
    /// <remarks>See https://github.com/MachineCognitis/C.math.NET/blob/master/C.math/math.cs#L1771</remarks>
    /// <param name="fromNumber">A floating-point number.</param>
    /// <param name="towardNumber">A floating-point number.</param>
    /// <returns>The floating-point number that is next after <paramref name="fromNumber"/> in the direction of <paramref name="towardNumber"/></returns>
    public static float NextAfter(float fromNumber, float towardNumber)
    {
        // If either fromNumber or towardNumber is NaN, return NaN.
        if (float.IsNaN(towardNumber) || float.IsNaN(fromNumber))
            return float.NaN;

        // If no direction or if fromNumber is infinity or is not a number, return fromNumber.
        if (fromNumber == towardNumber)
            return towardNumber;

        // If fromNumber is zero, return smallest subnormal.
        if (fromNumber == 0)
            return towardNumber > 0 ? float.Epsilon : -float.Epsilon;

        // All other cases are handled by incrementing or decrementing the bits value.
        // Transitions to infinity, to subnormal, and to zero are all taken care of this way.
        var bits = FloatToInt.Convert(fromNumber);
        // A xor here avoids nesting conditionals. We have to increment if fromValue lies between 0 and toValue.
        if ((fromNumber > 0) ^ (fromNumber > towardNumber))
            bits += 1;
        else
            bits -= 1;

        return FloatToInt.Convert(bits);
    }
}
