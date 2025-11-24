using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
/// `Pos` is the type for positive integers. This is used to represent line
/// number, column number, and similar things like indentation level.
/// 
/// `Semigroup` instance can be  used to safely and efficiently add `Pos` values
/// together.
/// </summary>
/// <param name="Value"></param>
public readonly record struct Pos(int Value) : Monoid<Pos>, INumber<Pos>
{
    public Pos Combine(Pos rhs) => 
        new (Value + rhs.Value);

    /// <summary>
    /// Monoid identity: Zero position
    /// </summary>
    static Pos Monoid<Pos>.Empty { get; } = 
        new (0);

    /// <summary>
    /// Zero position
    /// </summary>
    public static Pos Zero { get; } = 
        new (0);

    public static Pos One { get; } =
        new(1);
    
    public static int Radix { get; } =
        10;
    
    public static Pos AdditiveIdentity { get; } =
        new(0);

    public static Pos MultiplicativeIdentity { get; } =
        new(1);

    /// <summary>
    /// Implicit conversion from int to Pos
    /// </summary>
    public static implicit operator Pos(int value) => 
        new (value);

    public static Pos Abs(Pos value) => 
        new(Math.Abs(value.Value));

    public static bool IsCanonical(Pos value) => 
        true;

    public static bool IsComplexNumber(Pos value) =>
        false;

    public static bool IsEvenInteger(Pos value) => 
        int.IsEvenInteger(value.Value);

    public static bool IsFinite(Pos value) => 
        true;

    public static bool IsImaginaryNumber(Pos value) => 
        false;

    public static bool IsInfinity(Pos value) => 
        false;

    public static bool IsInteger(Pos value) => 
        true;

    public static bool IsNaN(Pos value) => 
        false;

    public static bool IsNegative(Pos value) => 
        value.Value < 0;

    public static bool IsNegativeInfinity(Pos value) => 
        false;

    public static bool IsNormal(Pos value) =>
        true;

    public static bool IsOddInteger(Pos value) => 
        (value.Value & 1) == 1;

    public static bool IsPositive(Pos value) => 
        true;

    public static bool IsPositiveInfinity(Pos value) => 
        false;

    public static bool IsRealNumber(Pos value) => 
        false;

    public static bool IsSubnormal(Pos value) => 
        false;

    public static bool IsZero(Pos value) => 
        value.Value == 0;

    public static Pos MaxMagnitude(Pos x, Pos y) =>
        x.Value > y.Value
            ? x
            : y;

    public static Pos MaxMagnitudeNumber(Pos x, Pos y) => 
        x.Value > y.Value
            ? x
            : y;

    public static Pos MinMagnitude(Pos x, Pos y) => 
        x.Value < y.Value
            ? x
            : y;

    public static Pos MinMagnitudeNumber(Pos x, Pos y) => 
        x.Value < y.Value
            ? x
            : y;

    public static Pos Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => 
        new (int.Parse(s, style, provider));

    public static Pos Parse(string s, NumberStyles style, IFormatProvider? provider) => 
        new (int.Parse(s, style, provider));
    
    public static Pos Parse(string s, IFormatProvider? provider) => 
        new (int.Parse(s, provider));

    public static Pos Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => 
        new (int.Parse(s, provider));

    public static bool TryConvertFromChecked<TOther>(TOther value, out Pos result) where TOther : INumberBase<TOther> =>
        throw new NotImplementedException();

    public static bool TryConvertFromSaturating<TOther>(TOther value, out Pos result) where TOther : INumberBase<TOther> => 
        throw new NotImplementedException();

    public static bool TryConvertFromTruncating<TOther>(TOther value, out Pos result) where TOther : INumberBase<TOther> => 
        throw new NotImplementedException();

    public static bool TryConvertToChecked<TOther>(Pos value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => 
        throw new NotImplementedException();

    public static bool TryConvertToSaturating<TOther>(Pos value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => 
        throw new NotImplementedException();

    public static bool TryConvertToTruncating<TOther>(Pos value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => 
        throw new NotImplementedException();

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Pos result) 
    {
        if (int.TryParse(s, style, provider, out var x))
        {
            result = new Pos(x);
            return true;
        }
        result = default;
        return false;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Pos result) 
    {
        if (int.TryParse(s, style, provider, out var x))
        {
            result = new Pos(x);
            return true;
        }
        result = default;
        return false;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Pos result)
    {
        if (int.TryParse(s, provider, out var x))
        {
            result = new Pos(x);
            return true;
        }
        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Pos result)
    {
        if (int.TryParse(s, provider, out var x))
        {
            result = new Pos(x);
            return true;
        }
        result = default;
        return false;
    }

    public int CompareTo(object? obj) =>
        obj is Pos rhs
            ? CompareTo(rhs)
            : 1;

    public int CompareTo(Pos other) => 
        Value.CompareTo(other.Value);

    public string ToString(string? format, IFormatProvider? formatProvider) => 
        $"Pos({Value})";

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => 
        throw new NotImplementedException();

    public static Pos operator +(Pos left, Pos right) => 
        new (left.Value + right.Value);

    public static bool operator >(Pos left, Pos right) => 
        left.Value > right.Value;

    public static bool operator >=(Pos left, Pos right) => 
        left.Value >= right.Value;

    public static bool operator <(Pos left, Pos right) => 
        left.Value < right.Value;

    public static bool operator <=(Pos left, Pos right) => 
        left.Value <= right.Value;

    public static Pos operator --(Pos value) =>
        new(value.Value - 1);

    public static Pos operator /(Pos left, Pos right) => 
        new (left.Value / right.Value);

    public static Pos operator ++(Pos value) => 
        new (value.Value + 1);

    public static Pos operator %(Pos left, Pos right) => 
        new (left.Value % right.Value);
  
    public static Pos operator *(Pos left, Pos right) => 
        new (left.Value * right.Value);

    public static Pos operator -(Pos left, Pos right) => 
        new (left.Value - right.Value);

    public static Pos operator -(Pos value) => 
        new (-value.Value);

    public static Pos operator +(Pos value) => 
        new (+value.Value);
}
