using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Net;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    [Pure]
    public static Option<A> convert<A>(object? value)
    {
        if (value == null)
        {
            return None;
        }

        try
        {
            var nvalue = (A)Convert.ChangeType(value, typeof(A));
            return nvalue;
        }
        catch
        {
            return None;
        }
    }

    [Pure]
    public static K<M, A> convert<M, A>(object? value)
        where M : Alternative<M>
    {
        if (value == null)
        {
            return M.Empty<A>();
        }

        try
        {
            var nvalue = (A)Convert.ChangeType(value, typeof(A));
            return M.Pure(nvalue);
        }
        catch
        {
            return M.Empty<A>();
        }
    }

    [Pure]
    public static Option<long> parseLong(string? value) =>
        Parse<long>(long.TryParse, value);

    [Pure]
    public static K<M, long> parseLong<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, long>(long.TryParse, value);

    [Pure]
    public static Option<int> parseInt(string? value) =>
        Parse<int>(int.TryParse, value);

    [Pure]
    public static K<M, int> parseInt<M>(string? value)
        where M : Alternative<M> =>
        Parse<M, int>(int.TryParse, value);

    [Pure]
    public static Option<int> parseInt(string? value, int fromBase)
    {
        try
        {
            return Convert.ToInt32(value, fromBase);
        }
        catch
        {
            return None;
        }
    }

    [Pure]
    public static K<M, int> parseInt<M>(string? value, int fromBase)
        where M : Alternative<M>
    {
        try
        {
            return M.Pure(Convert.ToInt32(value, fromBase));
        }
        catch
        {
            return M.Empty<int>();
        }
    }

    [Pure]
    public static Option<short> parseShort(string? value) =>
        Parse<short>(short.TryParse, value);

    [Pure]
    public static K<M, short> parseShort<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, short>(short.TryParse, value);

    [Pure]
    public static Option<char> parseChar(string? value) =>
        Parse<char>(char.TryParse, value);

    [Pure]
    public static K<M, char> parseChar<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, char>(char.TryParse, value);

    [Pure]
    public static Option<sbyte> parseSByte(string? value) =>
        Parse<sbyte>(sbyte.TryParse, value);

    [Pure]
    public static K<M, sbyte> parseSByte<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, sbyte>(sbyte.TryParse, value);

    [Pure]
    public static Option<byte> parseByte(string? value) =>
        Parse<byte>(byte.TryParse, value);

    [Pure]
    public static K<M, byte> parseByte<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, byte>(byte.TryParse, value);

    [Pure]
    public static Option<ulong> parseULong(string? value) =>
        Parse<ulong>(ulong.TryParse, value);

    [Pure]
    public static K<M, ulong> parseULong<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, ulong>(ulong.TryParse, value);

    [Pure]
    public static Option<uint> parseUInt(string? value) =>
        Parse<uint>(uint.TryParse, value);

    [Pure]
    public static K<M, uint> parseUInt<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, uint>(uint.TryParse, value);

    [Pure]
    public static Option<ushort> parseUShort(string? value) =>
        Parse<ushort>(ushort.TryParse, value);

    [Pure]
    public static K<M, ushort> parseUShort<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, ushort>(ushort.TryParse, value);

    [Pure]
    public static Option<float> parseFloat(string? value) =>
        Parse<float>(float.TryParse, value);

    [Pure]
    public static K<M, float> parseFloat<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, float>(float.TryParse, value);

    [Pure]
    public static Option<double> parseDouble(string? value) =>
        Parse<double>(double.TryParse, value);

    [Pure]
    public static K<M, double> parseDouble<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, double>(double.TryParse, value);

    [Pure]
    public static Option<decimal> parseDecimal(string? value) =>
        Parse<decimal>(decimal.TryParse, value);

    [Pure]
    public static K<M, decimal> parseDecimal<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, decimal>(decimal.TryParse, value);

    [Pure]
    public static Option<bool> parseBool(string? value) =>
        Parse<bool>(bool.TryParse, value);

    [Pure]
    public static K<M, bool> parseBool<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, bool>(bool.TryParse, value);

    [Pure]
    public static Option<Guid> parseGuid(string? value) =>
        Parse<Guid>(Guid.TryParse, value);

    [Pure]
    public static K<M, Guid> parseGuid<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, Guid>(Guid.TryParse, value);

    [Pure]
    public static Option<DateTime> parseDateTime(string? value) =>
        Parse<DateTime>(DateTime.TryParse, value);

    [Pure]
    public static K<M, DateTime> parseDateTime<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, DateTime>(DateTime.TryParse, value);

    [Pure]
    public static Option<DateTimeOffset> parseDateTimeOffset(string? value) =>
        Parse<DateTimeOffset>(DateTimeOffset.TryParse, value);

    [Pure]
    public static K<M, DateTimeOffset> parseDateTimeOffset<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, DateTimeOffset>(DateTimeOffset.TryParse, value);
        
    [Pure]
    public static Option<TimeSpan> parseTimeSpan(string? value) =>
        Parse<TimeSpan>(TimeSpan.TryParse, value);
        
    [Pure]
    public static K<M, TimeSpan> parseTimeSpan<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, TimeSpan>(TimeSpan.TryParse, value);

    [Pure]
    public static Option<TEnum> parseEnum<TEnum>(string? value)
        where TEnum : struct =>
        Parse<TEnum>(Enum.TryParse, value);

    [Pure]
    public static K<M, TEnum> parseEnum<M, TEnum>(string? value)
        where TEnum : struct 
        where M : Alternative<M> =>
        Parse<M, TEnum>(Enum.TryParse, value);

    [Pure]
    public static Option<TEnum> parseEnumIgnoreCase<TEnum>(string? value)
        where TEnum : struct =>
        ParseIgnoreCase<TEnum>(Enum.TryParse, value);

    [Pure]
    public static K<M, TEnum> parseEnumIgnoreCase<M, TEnum>(string? value)
        where TEnum : struct 
        where M : Alternative<M> =>
        ParseIgnoreCase<M, TEnum>(Enum.TryParse, value);

    [Pure]
    public static Option<IPAddress> parseIPAddress(string? value) =>
        Parse<IPAddress>(IPAddress.TryParse, value);

    [Pure]
    public static K<M, IPAddress> parseIPAddress<M>(string? value) 
        where M : Alternative<M> =>
        Parse<M, IPAddress>(IPAddress.TryParse, value);

    delegate bool TryParse<T>(string value, [NotNullWhen(true)] out T? result);

    delegate bool TryParseIgnoreCase<T>(string value, bool ignoreCase, [NotNullWhen(true)] out T? result);

    static Option<A> Parse<A>(TryParse<A> tryParse, string? value) =>
        value is not null && tryParse(value, out var result)
            ? Some(result)
            : None;

    static K<M, A> Parse<M, A>(TryParse<A> tryParse, string? value) 
        where M : Alternative<M> =>
        value is not null && tryParse(value, out var result)
            ? M.Pure(result)
            : M.Empty<A>();
    
    static Option<A> ParseIgnoreCase<A>(TryParseIgnoreCase<A> tryParse, string? value) =>
        value is not null && tryParse(value, true, out var result)
            ? Some(result)
            : None;
    
    static K<M, A> ParseIgnoreCase<M, A>(TryParseIgnoreCase<A> tryParse, string? value) 
        where M : Alternative<M> =>
        value is not null && tryParse(value, true, out var result)
            ? M.Pure(result)
            : M.Empty<A>();
}
