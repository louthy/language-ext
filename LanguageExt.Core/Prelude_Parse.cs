using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        [Pure]
        public static Option<T> convert<T>(string text)
        {
            if (text == null)
            {
                return None;
            }

            try
            {
                var val = (T)Convert.ChangeType(text, typeof(T));
                return val;
            }
            catch
            {
                return None;
            }
        }

        [Pure]
        public static Option<long> parseLong(string value)
        {
            long result;
            return Int64.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<int> parseInt(string value)
        {
            int result;
            return Int32.TryParse(value, out result)
                ? Some(result)
                : None;
        }

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
        public static Option<short> parseShort(string value)
        {
            short result;
            return Int16.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<char> parseChar(string value)
        {
            char result;
            return Char.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<byte> parseByte(string value)
        {
            byte result;
            return Byte.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<ulong> parseULong(string value)
        {
            ulong result;
            return UInt64.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<uint> parseUInt(string value)
        {
            uint result;
            return UInt32.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<ushort> parseUShort(string value)
        {
            ushort result;
            return UInt16.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<float> parseFloat(string value)
        {
            float result;
            return float.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<double> parseDouble(string value)
        {
            double result;
            return double.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<decimal> parseDecimal(string value)
        {
            decimal result;
            return decimal.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<bool> parseBool(string value)
        {
            bool result;
            return bool.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<Guid> parseGuid(string value)
        {
            Guid result;
            return Guid.TryParse(value, out result)
                ? Some(result)
                : None;
        }

        [Pure]
        public static Option<DateTime> parseDateTime(string value)
        {
            DateTime result;
            return DateTime.TryParse(value, out result)
                ? Some(result)
                : None;
        }
    }
}
