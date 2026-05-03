using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LanguageExt;

/// <summary>
/// Represents an index with the range of a `long` rather than an `int`.
/// </summary>
public readonly struct LongIndex : IEquatable<LongIndex>
{
    readonly long value;

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LongIndex(int value, bool fromEnd = false)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");

        this.value = fromEnd
                         ? ~value
                         : value;
    }

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LongIndex(Index value) =>
        this.value = value.IsFromEnd
                         ? ~value.Value
                         : value.Value;

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LongIndex(long value, bool fromEnd = false)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
        this.value = fromEnd
                         ? ~value
                         : value;
    }

    // The following private constructors mainly created for perf reason to avoid the checks
    LongIndex(int value) =>
        this.value = value;

    // The following private constructors mainly created for perf reason to avoid the checks
    LongIndex(long value) =>
        this.value = value;

    /// <summary>Create a LongIndex pointing at first element.</summary>
    public static LongIndex Start => 
        new (0L);

    /// <summary>Create a LongIndex pointing at beyond last element.</summary>
    public static LongIndex End => 
        new (~0L);

    /// <summary>Create a LongIndex from the start at the position indicated by the value.</summary>
    /// <param name="value">The index value from the start.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LongIndex FromStart(int value)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
        return new LongIndex(value);
    }

    /// <summary>Create a LongIndex from the start at the position indicated by the value.</summary>
    /// <param name="value">The index value from the start.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LongIndex FromStart(long value)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
        return new LongIndex(value);
    }

    /// <summary>Create a LongIndex from the end at the position indicated by the value.</summary>
    /// <param name="value">The index value from the end.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LongIndex FromEnd(int value)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
        return new LongIndex(~value);
    }

    /// <summary>Create a LongIndex from the end at the position indicated by the value.</summary>
    /// <param name="value">The index value from the end.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LongIndex FromEnd(long value)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
        return new LongIndex(~value);
    }

    /// <summary>Returns the index value.</summary>
    public long Value =>
         value < 0 
             ? ~value 
             : value;

    /// <summary>Indicates whether the index is from the start or the end.</summary>
    public bool IsFromEnd =>
        value < 0;

    /// <summary>Calculate the offset from the start using the given collection length.</summary>
    /// <param name="length">The length of the collection that the Index will be used with. length has to be a positive value</param>
    /// <remarks>
    /// For performance reason, we don't validate the input length parameter and the returned offset value against negative values.
    /// we don't validate either the returned offset is greater than the input length.
    /// It is expected Index will be used with collections which always have non negative length/count. If the returned offset is negative and
    /// then used to index a collection will get out of range exception which will be same affect as the validation.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetOffset(long length)
    {
        var offset = value;
        if (IsFromEnd)
        {
            // offset = length - (~value)
            // offset = length + (~(~value) + 1)
            // offset = length + value + 1

            offset += length + 1;
        }

        return offset;
    }

    /// <summary>Indicates whether the current Index object is equal to another object of the same type.</summary>
    /// <param name="value">An object to compare with this object</param>
    public override bool Equals([NotNullWhen(true)] object? value) => 
        value is LongIndex index && 
        this.value == index.value;

    /// <summary>Indicates whether the current Index object is equal to another Index object.</summary>
    /// <param name="other">An object to compare with this object</param>
    public bool Equals(LongIndex other) => 
        value == other.value;

    /// <summary>Returns the hash code for this instance.</summary>
    public override int GetHashCode() => 
        value.GetHashCode();

    /// <summary>Converts integer number to an LongIndex.</summary>
    public static implicit operator LongIndex(int value) => 
        FromStart(value);

    /// <summary>Converts long integer number to an LongIndex.</summary>
    public static implicit operator LongIndex(long value) => 
        FromStart(value);

    /// <summary>Converts Index to a LongIndex.</summary>
    public static implicit operator LongIndex(Index value) => 
        new (value);

    /// <summary>Converts the value of the current Index object to its equivalent string representation.</summary>
    public override string ToString() =>
        IsFromEnd
            ? ToStringFromEnd()
            : ((ulong)Value).ToString();

    string ToStringFromEnd()
    {
        Span<char> span = stackalloc char[21]; // 1 for ^ and 20 for longest possible ulong value
        ((ulong)Value).TryFormat(span[1..], out var charsWritten);
        span[0] = '^';
        return new string(span[..(charsWritten + 1)]);
    }
}
