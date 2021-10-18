using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// BigInteger convenience wrapper
    /// </summary>
    public readonly struct bigint : IComparable, IComparable<bigint>, IEquatable<bigint>
    {
        public readonly BigInteger Value;

        /// <summary>
        /// Initializes a new instance of bigint structure using provided BigInteger value.
        /// </summary>
        /// <param name="value">A big integer value to initialise this structure with</param>
        public bigint(BigInteger value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the bigint structure using
        /// the values in a byte array.
        ///
        /// </summary><remarks>
        /// value:
        /// An array of byte values in little-endian order.
        ///
        /// </remarks>
        public bigint(byte[] value)
        {
            Value = new BigInteger(value);
        }

        /// <summary>
        /// Initializes a new instance of the bigint structure using
        /// a System.Decimal value.
        ///
        /// </summary><remarks>
        /// value:
        /// A decimal number.
        /// </remarks>
        public bigint(decimal value)
        {
            Value = new BigInteger(value);
        }

        /// <summary>
        /// Initializes a new instance of the bigint structure using
        /// a double-precision floating-point value.
        ///
        /// </summary><remarks>
        /// value:
        /// A double-precision floating-point value.
        ///
        /// </remarks>
        public bigint(double value)
        {
            Value = new BigInteger(value);
        }

        /// <summary>
        /// Initializes a new instance of the bigint structure using
        /// a 32-bit signed integer value.
        ///
        /// </summary><remarks>
        /// value:
        /// A 32-bit signed integer.
        /// </remarks>
        public bigint(int value)
        {
            Value = new BigInteger(value);
        }

        /// <summary>
        /// Initializes a new instance of the bigint structure using
        /// a 64-bit signed integer value.
        ///
        /// </summary><remarks>
        /// value:
        /// A 64-bit signed integer.
        /// </remarks>
        public bigint(long value)
        {
            Value = new BigInteger(value);
        }

        /// <summary>
        /// Initializes a new instance of the bigint structure using
        /// a single-precision floating-point value.
        ///
        /// </summary><remarks>
        /// value:
        /// A single-precision floating-point value.
        ///
        /// </remarks>
        public bigint(float value)
        {
            Value = new BigInteger(value);
        }

        /// <summary>
        /// Initializes a new instance of the bigint structure using
        /// an unsigned 32-bit integer value.
        ///
        /// </summary><remarks>
        /// value:
        /// An unsigned 32-bit integer value.
        /// </remarks>
        public bigint(uint value)
        {
            Value = new BigInteger(value);
        }

        /// <summary>
        /// Initializes a new instance of the bigint structure with an
        /// unsigned 64-bit integer value.
        ///
        /// </summary><remarks>
        /// value:
        /// An unsigned 64-bit integer.
        /// </remarks>
        public bigint(ulong value)
        {
            Value = new BigInteger(value);
        }

        /// <summary>
        /// Gets a value that represents the number one (1).
        /// </summary>
        /// <returns>
        /// An object whose value is one (1).
        /// </returns>
        public static readonly bigint One = new bigint(1);

        /// <summary>
        /// Gets a value that represents the number negative one (-1).
        /// </summary>
        /// <returns>
        /// An integer whose value is negative one (-1).
        /// </returns>
        public static readonly bigint MinusOne = new bigint(-1);

        /// <summary>
        /// Gets a value that represents the number 0 (zero).
        /// </summary>
        /// <returns>
        /// An integer whose value is 0 (zero).
        /// </returns>
        public static bigint Zero = new bigint(0);

        /// <summary>
        /// Indicates whether the value of the current bigint object
        /// is an even number.
        /// </summary>
        /// <returns>
        /// true if the value of the bigint object is an even number;
        /// otherwise, false.
        /// </returns>
        public bool IsEven => Value.IsEven;

        /// <summary>
        /// Indicates whether the value of the current bigint object
        /// is bigint.Zero.
        /// </summary>
        /// <returns>
        /// true if the value of the bigint object is bigint.Zero;
        /// otherwise, false.
        /// </returns>
        public bool IsZero => Value.IsZero;

        /// <summary>
        /// Indicates whether the value of the current bigint object
        /// is a power of two.
        /// </summary>
        /// <returns>
        /// true if the value of the bigint object is a power of two;
        /// otherwise, false.
        /// </returns>
        public bool IsPowerOfTwo => Value.IsPowerOfTwo;

        /// <summary>
        /// Indicates whether the value of the current bigint object
        /// is bigint.One.
        /// </summary>
        /// <returns>
        /// true if the value of the bigint object is bigint.One;
        /// otherwise, false.
        /// </returns>
        public bool IsOne => Value.IsOne;

        /// <summary>
        /// Gets a number that indicates the sign (negative, positive, or zero) of the current
        /// bigint object.
        /// </summary>
        /// <returns>
        /// A number that indicates the sign of the bigint object, as
        /// shown in the following table.NumberDescription-1The value of this object is negative.0The
        /// value of this object is 0 (zero).1The value of this object is positive.
        /// </returns>
        public int Sign => Value.Sign;

        /// <summary>
        /// Gets the absolute value of a bigint object.
        /// </summary>
        /// <returns>
        /// The absolute value of value.
        /// </returns>
        public static bigint Abs(bigint value) => 
            new bigint(BigInteger.Abs(value.Value));

        /// <summary>
        /// Adds two bigint values and returns the result.
        /// </summary>
        /// <returns>
        /// The sum of left and right.
        /// </returns>
        public static bigint Add(bigint left, bigint right) =>
            BigInteger.Add(left.Value, right.Value);

        /// <summary>
        /// Compares two bigint values and returns an integer that indicates
        /// whether the first value is less than, equal to, or greater than the second value.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of left and right, as shown
        /// in the following table.ValueConditionLess than zeroleft is less than right.Zeroleft
        /// equals right.Greater than zeroleft is greater than right.
        /// </returns>
        public static int Compare(bigint left, bigint right) =>
            left.Value.CompareTo(right.Value);

        /// <summary>
        /// Divides one bigint value by another and returns the result.
        ///
        /// </summary><remarks>
        /// dividend:
        /// The value to be divided.
        ///
        /// divisor:
        /// The value to divide by.
        ///
        /// </remarks><returns>
        /// The quotient of the division.
        ///
        /// </returns>
        public static bigint Divide(bigint dividend, bigint divisor) =>
            BigInteger.Divide(dividend.Value, divisor.Value);

        /// <summary>
        /// Divides one bigint value by another, returns the quotient and remainder.
        ///
        /// </summary><remarks>
        /// dividend:
        /// The value to be divided.
        ///
        /// divisor:
        /// The value to divide by.
        ///
        /// </remarks><returns>
        /// The quotient and remainder of the division as a tuple
        ///
        /// </returns>
        public static (bigint Quotient, bigint Remainder) DivRem(bigint dividend, bigint divisor)
        {
            var res = BigInteger.DivRem(dividend.Value, divisor.Value, out BigInteger rem);
            return (new bigint(res), new bigint(rem));
        }

        /// <summary>
        /// Finds the greatest common divisor of two bigint values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value.
        ///
        /// right:
        /// The second value.
        ///
        /// </remarks><returns>
        /// The greatest common divisor of left and right.
        /// </returns>
        public static bigint GreatestCommonDivisor(bigint left, bigint right) =>
            new bigint(BigInteger.GreatestCommonDivisor(left.Value, right.Value));

        /// <summary>
        /// Returns the natural (base e) logarithm of a specified number.
        ///
        /// </summary><remarks>
        /// value:
        /// The number whose logarithm is to be found.
        ///
        /// </remarks><returns>
        /// The natural (base e) logarithm of value, as shown in the table in the Remarks
        /// section.
        ///
        /// </returns>
        public static double Log(bigint value) =>
            BigInteger.Log(value.Value);

        /// <summary>
        /// Returns the logarithm of a specified number in a specified base.
        ///
        /// </summary><remarks>
        /// value:
        /// A number whose logarithm is to be found.
        ///
        /// baseValue:
        /// The base of the logarithm.
        ///
        /// </remarks><returns>
        /// The base baseValue logarithm of value, as shown in the table in the Remarks section.
        ///
        /// </returns>
        public static double Log(bigint value, double baseValue) =>
            BigInteger.Log(value.Value, baseValue);

        /// <summary>
        /// Returns the base 10 logarithm of a specified number.
        ///
        /// </summary><remarks>
        /// value:
        /// A number whose logarithm is to be found.
        ///
        /// </remarks><returns>
        /// The base 10 logarithm of value, as shown in the table in the Remarks section.
        ///
        /// </returns>
        public static double Log10(bigint value) =>
            BigInteger.Log10(value.Value);

        /// <summary>
        /// Returns the larger of two bigint values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// The left or right parameter, whichever is larger.
        /// </returns>
        public static bigint Max(bigint left, bigint right) =>
            new bigint(BigInteger.Max(left.Value, right.Value));

        /// <summary>
        /// Returns the smaller of two bigint values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// The left or right parameter, whichever is smaller.
        /// </returns>
        public static bigint Min(bigint left, bigint right) =>
            new bigint(BigInteger.Min(left.Value, right.Value));

        /// <summary>
        /// Performs modulus division on a number raised to the power of another number.
        ///
        /// </summary><remarks>
        /// value:
        /// The number to raise to the exponent power.
        ///
        /// exponent:
        /// The exponent to raise value by.
        ///
        /// modulus:
        /// The number by which to divide value raised to the exponent power.
        ///
        /// </remarks><returns>
        /// The remainder after dividing value exponent by modulus.
        ///
        /// </returns>
        public static bigint ModPow(bigint value, bigint exponent, bigint modulus) =>
            new bigint(BigInteger.ModPow(value.Value, exponent.Value, modulus.Value));

        /// <summary>
        /// Returns the product of two bigint values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first number to multiply.
        ///
        /// right:
        /// The second number to multiply.
        ///
        /// </remarks><returns>
        /// The product of the left and right parameters.
        /// </returns>
        public static bigint Multiply(bigint left, bigint right) =>
            new bigint(BigInteger.Multiply(left.Value, right.Value));

        /// <summary>
        /// Negates a specified bigint value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to negate.
        ///
        /// </remarks><returns>
        /// The result of the value parameter multiplied by negative one (-1).
        /// </returns>
        public static bigint Negate(bigint value) =>
            new bigint(BigInteger.Negate(value.Value));

        /// <summary>
        /// Converts the string representation of a number in a specified style to its bigint
        /// equivalent.
        ///
        /// </summary><remarks>
        /// value:
        /// A string that contains a number to convert.
        ///
        /// style:
        /// A bitwise combination of the enumeration values that specify the permitted format
        /// of value.
        ///
        /// </remarks><returns>
        /// A value that is equivalent to the number specified in the value parameter.
        ///
        /// </returns>
        public static bigint Parse(string value, NumberStyles style) =>
            new bigint(BigInteger.Parse(value, style));

        /// <summary>
        /// Converts the string representation of a number in a specified culture-specific
        /// format to its bigint equivalent.
        ///
        /// </summary><remarks>
        /// value:
        /// A string that contains a number to convert.
        ///
        /// provider:
        /// An object that provides culture-specific formatting information about value.
        ///
        /// </remarks><returns>
        /// A value that is equivalent to the number specified in the value parameter.
        ///
        /// </returns>
        public static bigint Parse(string value, IFormatProvider provider) =>
            new bigint(BigInteger.Parse(value, provider));

        /// <summary>
        /// Converts the string representation of a number to its bigint
        /// equivalent.
        ///
        /// </summary><remarks>
        /// value:
        /// A string that contains the number to convert.
        ///
        /// </remarks><returns>
        /// A value that is equivalent to the number specified in the value parameter.
        ///
        /// </returns>
        public static bigint Parse(string value) =>
            new bigint(BigInteger.Parse(value));

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific
        /// format to its bigint equivalent.
        ///
        /// </summary><remarks>
        /// value:
        /// A string that contains a number to convert.
        ///
        /// style:
        /// A bitwise combination of the enumeration values that specify the permitted format
        /// of value.
        ///
        /// provider:
        /// An object that provides culture-specific formatting information about value.
        ///
        /// </remarks><returns>
        /// A value that is equivalent to the number specified in the value parameter.
        ///
        /// </returns>
        public static bigint Parse(string value, NumberStyles style, IFormatProvider provider) =>
            new bigint(BigInteger.Parse(value, style, provider));

        /// <summary>
        /// Raises a bigint value to the power of a specified value.
        ///
        /// </summary><remarks>
        /// value:
        /// The number to raise to the exponent power.
        ///
        /// exponent:
        /// The exponent to raise value by.
        ///
        /// </remarks><returns>
        /// The result of raising value to the exponent power.
        ///
        /// </returns>
        public static bigint Pow(bigint value, int exponent) =>
            new bigint(BigInteger.Pow(value.Value, exponent));

        /// <summary>
        /// Performs integer division on two bigint values and returns
        /// the remainder.
        ///
        /// </summary><remarks>
        /// dividend:
        /// The value to be divided.
        ///
        /// divisor:
        /// The value to divide by.
        ///
        /// </remarks><returns>
        /// The remainder after dividing dividend by divisor.
        ///
        /// </returns>
        public static bigint Remainder(bigint dividend, bigint divisor) =>
            new bigint(BigInteger.Remainder(dividend.Value, divisor.Value));

        /// <summary>
        /// Subtracts one bigint value from another and returns the result.
        ///
        /// </summary><remarks>
        /// left:
        /// The value to subtract from (the minuend).
        ///
        /// right:
        /// The value to subtract (the subtrahend).
        ///
        /// </remarks><returns>
        /// The result of subtracting right from left.
        /// </returns>
        public static bigint Subtract(bigint left, bigint right) =>
            new bigint(BigInteger.Subtract(left.Value, right.Value));

        /// <summary>
        /// Tries to convert the string representation of a number in a specified style and
        /// culture-specific format to its bigint equivalent, and returns
        /// a value that indicates whether the conversion succeeded.
        ///
        /// </summary><remarks>
        /// value:
        /// The string representation of a number. The string is interpreted using the style
        /// specified by style.
        ///
        /// style:
        /// A bitwise combination of enumeration values that indicates the style elements
        /// that can be present in value. A typical value to specify is System.Globalization.NumberStyles.Integer.
        ///
        /// provider:
        /// An object that supplies culture-specific formatting information about value.
        ///
        /// </remarks><returns>
        /// Optional value 
        ///
        /// </returns>
        public static Option<bigint> TryParse(string value, NumberStyles style, IFormatProvider provider) =>
            BigInteger.TryParse(value, style, provider, out BigInteger res)
                ? Some(new bigint(res))
                : None;

        /// <summary>
        /// Tries to convert the string representation of a number to its bigint
        /// equivalent, and returns a value that indicates whether the conversion succeeded.
        ///
        /// </summary><remarks>
        /// value:
        /// The string representation of a number.
        ///
        /// </remarks><returns>
        /// Optional value 
        ///
        /// </returns>
        public static Option<bigint> TryParse(string value) =>
            BigInteger.TryParse(value, out BigInteger res)
                ? Some(new bigint(res))
                : None;
        /// <summary>
        /// Compares this instance to a signed 64-bit integer and returns an integer that
        /// indicates whether the value of this instance is less than, equal to, or greater
        /// than the value of the signed 64-bit integer.
        ///
        /// </summary><remarks>
        /// other:
        /// The signed 64-bit integer to compare.
        ///
        /// </remarks><returns>
        /// A signed integer value that indicates the relationship of this instance to other,
        /// as shown in the following table.Return valueDescriptionLess than zeroThe current
        /// instance is less than other.ZeroThe current instance equals other.Greater than
        /// zeroThe current instance is greater than other.
        /// </returns>
        public int CompareTo(long other) =>
            Value.CompareTo(other);

        /// <summary>
        /// Compares this instance to an unsigned 64-bit integer and returns an integer that
        /// indicates whether the value of this instance is less than, equal to, or greater
        /// than the value of the unsigned 64-bit integer.
        ///
        /// </summary><remarks>
        /// other:
        /// The unsigned 64-bit integer to compare.
        ///
        /// </remarks><returns>
        /// A signed integer that indicates the relative value of this instance and other,
        /// as shown in the following table.Return valueDescriptionLess than zeroThe current
        /// instance is less than other.ZeroThe current instance equals other.Greater than
        /// zeroThe current instance is greater than other.
        /// </returns>
        public int CompareTo(ulong other) =>
            Value.CompareTo(other);

        /// <summary>
        /// Compares this instance to a second bigint and returns an
        /// integer that indicates whether the value of this instance is less than, equal
        /// to, or greater than the value of the specified object.
        ///
        /// </summary><remarks>
        /// other:
        /// The object to compare.
        ///
        /// </remarks><returns>
        /// A signed integer value that indicates the relationship of this instance to other,
        /// as shown in the following table.Return valueDescriptionLess than zeroThe current
        /// instance is less than other.ZeroThe current instance equals other.Greater than
        /// zeroThe current instance is greater than other.
        /// </returns>
        public int CompareTo(BigInteger other) =>
            Value.CompareTo(other);

        /// <summary>
        /// Compares this instance to a second bigint and returns an
        /// integer that indicates whether the value of this instance is less than, equal
        /// to, or greater than the value of the specified object.
        ///
        /// </summary><remarks>
        /// other:
        /// The object to compare.
        ///
        /// </remarks><returns>
        /// A signed integer value that indicates the relationship of this instance to other,
        /// as shown in the following table.Return valueDescriptionLess than zeroThe current
        /// instance is less than other.ZeroThe current instance equals other.Greater than
        /// zeroThe current instance is greater than other.
        /// </returns>
        public int CompareTo(bigint other) =>
            Value.CompareTo(other.Value);

        /// <summary>
        /// Returns a value that indicates whether the current instance and a signed 64-bit
        /// integer have the same value.
        ///
        /// </summary><remarks>
        /// other:
        /// The signed 64-bit integer value to compare.
        ///
        /// </remarks><returns>
        /// true if the signed 64-bit integer and the current instance have the same value;
        /// otherwise, false.
        /// </returns>
        public bool Equals(long other) =>
            Value.Equals(other);

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified bigint
        /// object have the same value.
        ///
        /// </summary><remarks>
        /// other:
        /// The object to compare.
        ///
        /// </remarks><returns>
        /// true if this bigint object and other have the same value;
        /// otherwise, false.
        /// </returns>
        public bool Equals(BigInteger other) =>
            Value.Equals(other);

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified bigint
        /// object have the same value.
        ///
        /// </summary><remarks>
        /// other:
        /// The object to compare.
        ///
        /// </remarks><returns>
        /// true if this bigint object and other have the same value;
        /// otherwise, false.
        /// </returns>
        public bool Equals(bigint other) =>
            Value.Equals(other.Value);

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified object
        /// have the same value.
        ///
        /// </summary><remarks>
        /// obj:
        /// The object to compare.
        ///
        /// </remarks><returns>
        /// true if the obj parameter is a bigint object or a type capable
        /// of implicit conversion to a bigint value, and its value is
        /// equal to the value of the current bigint object; otherwise,
        /// false.
        /// </returns>
        public override bool Equals(object obj) =>
            Value.Equals(obj);

        /// <summary>
        /// Returns a value that indicates whether the current instance and an unsigned 64-bit
        /// integer have the same value.
        ///
        /// </summary><remarks>
        /// other:
        /// The unsigned 64-bit integer to compare.
        ///
        /// </remarks><returns>
        /// true if the current instance and the unsigned 64-bit integer have the same value;
        /// otherwise, false.
        /// </returns>
        public bool Equals(ulong other) =>
            Value.Equals(other);

        /// <summary>
        /// Returns the hash code for the current bigint object.
        ///
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() =>
            Value.GetHashCode();

        public int CompareTo(object obj) =>
            obj is bigint t ? CompareTo(t) : 1;

        /// <summary>
        /// Converts a bigint value to a byte array.
        ///
        /// </summary><returns>
        /// The value of the current bigint object converted to an array
        /// of bytes.
        /// </returns>
        public byte[] ToByteArray() =>
            Value.ToByteArray();

        /// <summary>
        /// Converts the numeric value of the current bigint object to
        /// its equivalent string representation.
        /// </summary>
        /// <returns>
        /// The string representation of the current bigint value.
        /// </returns>
        public override string ToString() =>
            Value.ToString();

        /// <summary>
        /// Converts the numeric value of the current bigint object to
        /// its equivalent string representation by using the specified format.
        ///
        /// </summary><remarks>
        /// format:
        /// A standard or custom numeric format string.
        ///
        /// </remarks><returns>
        /// The string representation of the current bigint value in
        /// the format specified by the format parameter.
        ///
        /// </returns>
        public string ToString(string format) =>
            Value.ToString(format);

        /// <summary>
        /// Converts the numeric value of the current bigint object to
        /// its equivalent string representation by using the specified culture-specific
        /// formatting information.
        ///
        /// </summary><remarks>
        /// provider:
        /// An object that supplies culture-specific formatting information.
        ///
        /// </remarks><returns>
        /// The string representation of the current bigint value in
        /// the format specified by the provider parameter.
        /// </returns>
        public string ToString(IFormatProvider provider) =>
            Value.ToString(provider);

        /// <summary>
        /// Converts the numeric value of the current bigint object to
        /// its equivalent string representation by using the specified format and culture-specific
        /// format information.
        ///
        /// </summary><remarks>
        /// format:
        /// A standard or custom numeric format string.
        ///
        /// provider:
        /// An object that supplies culture-specific formatting information.
        ///
        /// </remarks><returns>
        /// The string representation of the current bigint value as
        /// specified by the format and provider parameters.
        ///
        /// </returns>
        public string ToString(string format, IFormatProvider provider) =>
            Value.ToString(format, provider);

        /// <summary>
        /// Returns the value of the bigint operand. (The sign of the
        /// operand is unchanged.)
        ///
        /// </summary><remarks>
        /// value:
        /// An integer value.
        ///
        /// </remarks><returns>
        /// The value of the value operand.
        /// </returns>
        public static bigint operator +(bigint value) =>
            value;

        /// <summary>
        /// Adds the values of two specified bigint objects.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to add.
        ///
        /// right:
        /// The second value to add.
        ///
        /// </remarks><returns>
        /// The sum of left and right.
        /// </returns>
        public static bigint operator +(bigint left, bigint right) =>
            new bigint(left.Value + right.Value);

        /// <summary>
        /// Negates a specified BigInteger value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to negate.
        ///
        /// </remarks><returns>
        /// The result of the value parameter multiplied by negative one (-1).
        /// </returns>
        public static bigint operator -(bigint value) =>
            new bigint(-value.Value);

        /// <summary>
        /// Subtracts a bigint value from another bigint
        /// value.
        ///
        /// </summary><remarks>
        /// left:
        /// The value to subtract from (the minuend).
        ///
        /// right:
        /// The value to subtract (the subtrahend).
        ///
        /// </remarks><returns>
        /// The result of subtracting right from left.
        /// </returns>
        public static bigint operator -(bigint left, bigint right) =>
            new bigint(left.Value - right.Value);

        /// <summary>
        /// Returns the bitwise one's complement of a bigint value.
        ///
        /// </summary><remarks>
        /// value:
        /// An integer value.
        ///
        /// </remarks><returns>
        /// The bitwise one's complement of value.
        /// </returns>
        public static bigint operator ~(bigint value) =>
            new bigint(~value.Value);

        /// <summary>
        /// Increments a bigint value by 1.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to increment.
        ///
        /// </remarks><returns>
        /// The value of the value parameter incremented by 1.
        /// </returns>
        public static bigint operator ++(bigint value) =>
            value + One;

        /// <summary>
        /// Decrements a bigint value by 1.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to decrement.
        ///
        /// </remarks><returns>
        /// The value of the value parameter decremented by 1.
        /// </returns>
        public static bigint operator --(bigint value) =>
            value - One;

        /// <summary>
        /// Multiplies two specified bigint values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to multiply.
        ///
        /// right:
        /// The second value to multiply.
        ///
        /// </remarks><returns>
        /// The product of left and right.
        /// </returns>
        public static bigint operator *(bigint left, bigint right) =>
            new bigint(left.Value * right.Value);

        /// <summary>
        /// Divides a specified bigint value by another specified bigint
        /// value by using integer division.
        ///
        /// </summary><remarks>
        /// dividend:
        /// The value to be divided.
        ///
        /// divisor:
        /// The value to divide by.
        ///
        /// </remarks><returns>
        /// The integral result of the division.
        ///
        /// </returns>
        public static bigint operator /(bigint dividend, bigint divisor) =>
            new bigint(dividend.Value / divisor.Value);

        /// <summary>
        /// Returns the remainder that results from division with two specified bigint
        /// values.
        ///
        /// </summary><remarks>
        /// dividend:
        /// The value to be divided.
        ///
        /// divisor:
        /// The value to divide by.
        ///
        /// </remarks><returns>
        /// The remainder that results from the division.
        ///
        /// </returns>
        public static bigint operator %(bigint dividend, bigint divisor) =>
            new bigint(dividend.Value % divisor.Value);

        /// <summary>
        /// Performs a bitwise And operation on two bigint values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value.
        ///
        /// right:
        /// The second value.
        ///
        /// </remarks><returns>
        /// The result of the bitwise And operation.
        /// </returns>
        public static bigint operator &(bigint left, bigint right) =>
            new bigint(left.Value & right.Value);

        /// <summary>
        /// Performs a bitwise Or operation on two bigint values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value.
        ///
        /// right:
        /// The second value.
        ///
        /// </remarks><returns>
        /// The result of the bitwise Or operation.
        /// </returns>
        public static bigint operator |(bigint left, bigint right) =>
            new bigint(left.Value | right.Value);

        /// <summary>
        /// Performs a bitwise exclusive Or (XOr) operation on two bigint
        /// values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value.
        ///
        /// right:
        /// The second value.
        ///
        /// </remarks><returns>
        /// The result of the bitwise Or operation.
        /// </returns>
        public static bigint operator ^(bigint left, bigint right) =>
            new bigint(left.Value ^ right.Value);

        /// <summary>
        /// Shifts a bigint value a specified number of bits to the left.
        ///
        /// </summary><remarks>
        /// value:
        /// The value whose bits are to be shifted.
        ///
        /// shift:
        /// The number of bits to shift value to the left.
        ///
        /// </remarks><returns>
        /// A value that has been shifted to the left by the specified number of bits.
        /// </returns>
        public static bigint operator <<(bigint value, int shift) =>
            new bigint(value.Value << shift);

        /// <summary>
        /// Shifts a bigint value a specified number of bits to the right.
        ///
        /// </summary><remarks>
        /// value:
        /// The value whose bits are to be shifted.
        ///
        /// shift:
        /// The number of bits to shift value to the right.
        ///
        /// </remarks><returns>
        /// A value that has been shifted to the right by the specified number of bits.
        /// </returns>
        public static bigint operator >>(bigint value, int shift) =>
            new bigint(value.Value >> shift);

        /// <summary>
        /// Returns a value that indicates whether a bigint value and
        /// an unsigned long integer value are equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if the left and right parameters have the same value; otherwise, false.
        /// </returns>
        public static bool operator ==(bigint left, ulong right) =>
            left.Value == right;

        /// <summary>
        /// Returns a value that indicates whether the values of two bigint
        /// objects are equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if the left and right parameters have the same value; otherwise, false.
        /// </returns>
        public static bool operator ==(BigInteger left, bigint right) =>
            left == right.Value;

        /// <summary>
        /// Returns a value that indicates whether the values of two bigint
        /// objects are equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if the left and right parameters have the same value; otherwise, false.
        /// </returns>
        public static bool operator ==(bigint left, BigInteger right) =>
            left.Value == right;

        /// <summary>
        /// Returns a value that indicates whether a signed long integer value and a bigint
        /// value are equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if the left and right parameters have the same value; otherwise, false.
        /// </returns>
        public static bool operator ==(long left, bigint right) =>
            left == right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value and
        /// a signed long integer value are equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if the left and right parameters have the same value; otherwise, false.
        /// </returns>
        public static bool operator ==(bigint left, long right) =>
            left.Value == right;

        /// <summary>
        /// Returns a value that indicates whether the values of two bigint
        /// objects are equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if the left and right parameters have the same value; otherwise, false.
        /// </returns>
        public static bool operator ==(bigint left, bigint right) =>
            left.Value == right.Value;

        /// <summary>
        /// Returns a value that indicates whether an unsigned long integer value and a bigint
        /// value are equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if the left and right parameters have the same value; otherwise, false.
        /// </returns>
        public static bool operator ==(ulong left, bigint right) =>
            left == right.Value;

        /// <summary>
        /// Returns a value that indicates whether a 64-bit unsigned integer and a bigint
        /// value are not equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left and right are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(ulong left, bigint right) =>
            left != right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value and
        /// a 64-bit unsigned integer are not equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left and right are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(bigint left, ulong right) =>
            left.Value != right;

        /// <summary>
        /// Returns a value that indicates whether a 64-bit signed integer and a bigint
        /// value are not equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left and right are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(long left, bigint right) =>
            left != right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value and
        /// a 64-bit signed integer are not equal.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left and right are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(bigint left, long right) =>
            left.Value != right;

        /// <summary>
        /// Returns a value that indicates whether two bigint objects
        /// have different values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left and right are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(bigint left, bigint right) =>
            left.Value != right.Value;

        /// <summary>
        /// Returns a value that indicates whether two bigint objects
        /// have different values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left and right are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(BigInteger left, bigint right) =>
            left != right.Value;

        /// <summary>
        /// Returns a value that indicates whether two bigint objects
        /// have different values.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left and right are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(bigint left, BigInteger right) =>
            left.Value != right;

        /// <summary>
        /// Returns a value that indicates whether a 64-bit signed integer is less than a
        /// bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than right; otherwise, false.
        /// </returns>
        public static bool operator <(long left, bigint right) =>
            left < right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// less than a 64-bit unsigned integer.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than right; otherwise, false.
        /// </returns>
        public static bool operator <(bigint left, ulong right) =>
            left.Value < right;

        /// <summary>
        /// Returns a value that indicates whether a 64-bit unsigned integer is less than
        /// a bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than right; otherwise, false.
        /// </returns>
        public static bool operator <(ulong left, bigint right) =>
            left < right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// less than another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than right; otherwise, false.
        /// </returns>
        public static bool operator <(bigint left, bigint right) =>
            left.Value < right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// less than another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than right; otherwise, false.
        /// </returns>
        public static bool operator <(BigInteger left, bigint right) =>
            left < right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// less than another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than right; otherwise, false.
        /// </returns>
        public static bool operator <(bigint left, BigInteger right) =>
            left.Value < right;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// less than a 64-bit signed integer.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than right; otherwise, false.
        /// </returns>
        public static bool operator <(bigint left, long right) =>
            left.Value < right;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// greater than a 64-bit unsigned integer.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >(ulong left, bigint right) =>
            left < right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// greater than another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >(bigint left, bigint right) =>
            left.Value > right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// greater than another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >(BigInteger left, bigint right) =>
            left > right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// greater than another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >(bigint left, BigInteger right) =>
            left.Value > right;

        /// <summary>
        /// Returns a value that indicates whether a 64-bit signed integer is greater than
        /// a bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >(long left, bigint right) =>
            left > right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint is greater
        /// than a 64-bit signed integer value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >(bigint left, long right) =>
            left.Value > right;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// greater than a 64-bit unsigned integer.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >(bigint left, ulong right) =>
            left.Value > right;

        /// <summary>
        /// Returns a value that indicates whether a 64-bit signed integer is less than or
        /// equal to a bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than or equal to right; otherwise, false.
        /// </returns>
        public static bool operator <=(long left, bigint right) =>
            left <= right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// less than or equal to a 64-bit signed integer.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than or equal to right; otherwise, false.
        /// </returns>
        public static bool operator <=(bigint left, long right) =>
            left.Value <= right;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// less than or equal to another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than or equal to right; otherwise, false.
        /// </returns>
        public static bool operator <=(bigint left, bigint right) =>
            left.Value <= right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// less than or equal to another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than or equal to right; otherwise, false.
        /// </returns>
        public static bool operator <=(BigInteger left, bigint right) =>
            left <= right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// less than or equal to another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than or equal to right; otherwise, false.
        /// </returns>
        public static bool operator <=(bigint left, BigInteger right) =>
            left.Value <= right;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// less than or equal to a 64-bit unsigned integer.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than or equal to right; otherwise, false.
        /// </returns>
        public static bool operator <=(bigint left, ulong right) =>
            left.Value <= right;

        /// <summary>
        /// Returns a value that indicates whether a 64-bit unsigned integer is less than
        /// or equal to a bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is less than or equal to right; otherwise, false.
        /// </returns>
        public static bool operator <=(ulong left, bigint right) =>
            left <= right.Value;

        /// <summary>
        /// Returns a value that indicates whether a 64-bit signed integer is greater than
        /// or equal to a bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >=(long left, bigint right) =>
            left >= right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// greater than or equal to a 64-bit signed integer value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >=(bigint left, long right) =>
            left.Value >= right;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// greater than or equal to another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >=(bigint left, bigint right) =>
            left.Value >= right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// greater than or equal to another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >=(BigInteger left, bigint right) =>
            left >= right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// greater than or equal to another bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >=(bigint left, BigInteger right) =>
            left.Value >= right;

        /// <summary>
        /// Returns a value that indicates whether a 64-bit unsigned integer is greater than
        /// or equal to a bigint value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >=(ulong left, bigint right) =>
            left >= right.Value;

        /// <summary>
        /// Returns a value that indicates whether a bigint value is
        /// greater than or equal to a 64-bit unsigned integer value.
        ///
        /// </summary><remarks>
        /// left:
        /// The first value to compare.
        ///
        /// right:
        /// The second value to compare.
        ///
        /// </remarks><returns>
        /// true if left is greater than right; otherwise, false.
        /// </returns>
        public static bool operator >=(bigint left, ulong right) =>
            left.Value >= right;

        /// <summary>
        /// Defines an implicit conversion of a BigInteger to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static implicit operator bigint(BigInteger value) =>
            new bigint(value);

        /// <summary>
        /// Defines an implicit conversion of an unsigned byte to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static implicit operator bigint(byte value) =>
            new bigint(value);

        /// <summary>
        /// Defines an implicit conversion of a 16-bit unsigned integer to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static implicit operator bigint(ushort value) =>
            new bigint(value);

        /// <summary>
        /// Defines an implicit conversion of an 8-bit signed integer to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static implicit operator bigint(sbyte value) =>
            new bigint(value);

        /// <summary>
        /// Defines an implicit conversion of a 32-bit unsigned integer to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static implicit operator bigint(uint value) =>
            new bigint(value);

        /// <summary>
        /// Defines an implicit conversion of a signed 64-bit integer to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static implicit operator bigint(long value) =>
            new bigint(value);

        /// <summary>
        /// Defines an implicit conversion of a signed 32-bit integer to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static implicit operator bigint(int value) =>
            new bigint(value);

        /// <summary>
        /// Defines an implicit conversion of a signed 16-bit integer to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static implicit operator bigint(short value) =>
            new bigint(value);

        /// <summary>
        /// Defines an implicit conversion of a 64-bit unsigned integer to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static implicit operator bigint(ulong value) =>
            new bigint(value);

        /// <summary>
        /// Defines an explicit conversion of a System.Decimal object to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static explicit operator bigint(decimal value) =>
            new bigint(value);

        /// <summary>
        /// Defines an explicit conversion of a System.Double value to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static explicit operator bigint(double value) =>
            new bigint(value);

        /// <summary>
        /// Defines an explicit conversion of a bigint object to an unsigned
        /// byte value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a System.Byte.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static explicit operator byte(bigint value) =>
            (byte)value.Value;

        /// <summary>
        /// Defines an explicit conversion of a bigint object to an bigint value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a System.Byte.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static implicit operator BigInteger(bigint value) =>
            value.Value;

        /// <summary>
        /// Defines an explicit conversion of a bigint object to a System.Decimal
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a System.Decimal.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static explicit operator decimal(bigint value) =>
            (decimal)value.Value;

        /// <summary>
        /// Defines an explicit conversion of a bigint object to a System.Double
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a System.Double.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        /// </returns>
        public static explicit operator double(bigint value) =>
            (double)value.Value;

        /// <summary>
        /// Defines an explicit conversion of a bigint object to a 16-bit
        /// signed integer value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a 16-bit signed integer.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static explicit operator short(bigint value) =>
            (short)value.Value;

        /// <summary>
        /// Defines an explicit conversion of a bigint object to a 64-bit
        /// signed integer value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a 64-bit signed integer.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static explicit operator long(bigint value) =>
            (long)value.Value;

        /// <summary>
        /// Defines an explicit conversion of a bigint object to a signed
        /// 8-bit value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a signed 8-bit value.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static explicit operator sbyte(bigint value) =>
            (sbyte)value.Value;

        /// <summary>
        /// Defines an explicit conversion of a bigint object to an unsigned
        /// 16-bit integer value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to an unsigned 16-bit integer.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter
        ///
        /// </returns>
        public static explicit operator ushort(bigint value) =>
            (ushort)value.Value;

        /// <summary>
        /// Defines an explicit conversion of a bigint object to an unsigned
        /// 32-bit integer value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to an unsigned 32-bit integer.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static explicit operator uint(bigint value) =>
            (uint)value.Value;

        /// <summary>
        /// Defines an explicit conversion of a bigint object to an unsigned
        /// 64-bit integer value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to an unsigned 64-bit integer.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static explicit operator ulong(bigint value) =>
            (ulong)value.Value;

        /// <summary>
        /// Defines an explicit conversion of a System.Single object to a bigint
        /// value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a bigint.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static explicit operator bigint(float value) =>
            new bigint(value);

        /// <summary>
        /// Defines an explicit conversion of a bigint object to a 32-bit
        /// signed integer value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a 32-bit signed integer.
        ///
        /// </remarks><returns>
        /// An object that contains the value of the value parameter.
        ///
        /// </returns>
        public static explicit operator int(bigint value) =>
            (int)value.Value;

        /// <summary>
        /// Defines an explicit conversion of a bigint object to a single-precision
        /// floating-point value.
        ///
        /// </summary><remarks>
        /// value:
        /// The value to convert to a single-precision floating-point value.
        ///
        /// </remarks><returns>
        /// An object that contains the closest possible representation of the value parameter.
        /// </returns>
        public static explicit operator float(bigint value) =>
            (float)value.Value;

    }
}
