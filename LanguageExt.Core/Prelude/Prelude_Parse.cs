using System;
using System.Diagnostics.Contracts;
using System.Net;

namespace LanguageExt
{
    public static partial class Prelude
    {
        [Pure]
        public static Option<T> convert<T>(object value)
        {
            if (value == null)
            {
                return None;
            }

            try
            {
                var nvalue = (T)Convert.ChangeType(value, typeof(T));
                return nvalue;
            }
            catch
            {
                return None;
            }
        }

        [Pure]
        public static Option<long> parseLong(string value) =>
            Parse<long>(long.TryParse, value);

        [Pure]
        public static Option<int> parseInt(string value) =>
            Parse<int>(int.TryParse, value);

        [Pure]
        public static Option<int> parseInt(string value, int fromBase)
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
        public static Option<short> parseShort(string value) =>
            Parse<short>(short.TryParse, value);

        [Pure]
        public static Option<char> parseChar(string value) =>
            Parse<char>(char.TryParse, value);

        [Pure]
        public static Option<sbyte> parseSByte(string value) =>
            Parse<sbyte>(sbyte.TryParse, value);

        [Pure]
        public static Option<byte> parseByte(string value) =>
            Parse<byte>(byte.TryParse, value);

        [Pure]
        public static Option<ulong> parseULong(string value) =>
            Parse<ulong>(ulong.TryParse, value);

        [Pure]
        public static Option<uint> parseUInt(string value) =>
            Parse<uint>(uint.TryParse, value);

        [Pure]
        public static Option<ushort> parseUShort(string value) =>
            Parse<ushort>(ushort.TryParse, value);

        [Pure]
        public static Option<float> parseFloat(string value) =>
            Parse<float>(float.TryParse, value);

        [Pure]
        public static Option<double> parseDouble(string value) =>
            Parse<double>(double.TryParse, value);

        [Pure]
        public static Option<decimal> parseDecimal(string value) =>
            Parse<decimal>(decimal.TryParse, value);

        [Pure]
        public static Option<bool> parseBool(string value) =>
            Parse<bool>(bool.TryParse, value);

        [Pure]
        public static Option<Guid> parseGuid(string value) =>
            Parse<Guid>(Guid.TryParse, value);

        [Pure]
        public static Option<DateTime> parseDateTime(string value) =>
            Parse<DateTime>(DateTime.TryParse, value);

        [Pure]
        public static Option<DateTimeOffset> parseDateTimeOffset(string value) =>
            Parse<DateTimeOffset>(DateTimeOffset.TryParse, value);
        
        [Pure]
        public static Option<TimeSpan> parseTimeSpan(string value) =>
            Parse<TimeSpan>(TimeSpan.TryParse, value);

        [Pure]
        public static Option<TEnum> parseEnum<TEnum>(string value)
            where TEnum : struct =>
            Parse<TEnum>(Enum.TryParse, value);

        [Pure]
        public static Option<TEnum> parseEnumIgnoreCase<TEnum>(string value)
            where TEnum : struct =>
            ParseIgnoreCase<TEnum>(Enum.TryParse, value);

        [Pure]
        public static Option<IPAddress> parseIPAddress(string value) =>
            Parse<IPAddress>(IPAddress.TryParse, value);

        private delegate bool TryParse<T>(string value, out T result);

        private delegate bool TryParseIgnoreCase<T>(string value, bool ignoreCase, out T result);

        private static Option<T> Parse<T>(TryParse<T> tryParse, string value) =>
            tryParse(value, out T result)
                ? Some(result)
                : None;

        private static Option<T> ParseIgnoreCase<T>(TryParseIgnoreCase<T> tryParse, string value) =>
            tryParse(value, true, out T result)
                ? Some(result)
                : None;
    }
}
