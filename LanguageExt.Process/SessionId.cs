using Newtonsoft.Json;
using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Session ID
    /// </summary>
    /// <remarks>
    /// It enforces the rules for session IDs.
    /// </remarks>
    public struct SessionId : IEquatable<SessionId>, IComparable<SessionId>, IComparable
    {
        public readonly string Value;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value">SessionId</param>
        [JsonConstructor]
        public SessionId(string value)
        {
            Value = value;
        }

        const int DefaultSessionIdSizeInBytes = 32;

        /// <summary>
        /// Make a cryptographically strong session ID
        /// </summary>
        /// <param name="sizeInBytes">Size in bytes.  This is not the final string length, the final length depends
        /// on the Base64 encoding of a byte-array sizeInBytes long.  As a guide a 64 byte session ID turns into
        /// an 88 character string.</returns>
        public static SessionId Generate(int sizeInBytes = DefaultSessionIdSizeInBytes) =>
            new SessionId(randomBase64(sizeInBytes));

        public bool IsValid =>
            !String.IsNullOrEmpty(Value);

        public override string ToString() =>
            Value;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public static implicit operator SessionId(string value) =>
            new SessionId(value);

        public bool Equals(SessionId other) =>
            Value.Equals(other.Value);

        public int CompareTo(SessionId other) =>
            String.Compare(Value, other.Value, StringComparison.Ordinal);

        public int CompareTo(object obj) =>
            obj == null
                ? -1
                : obj is SessionId
                    ? CompareTo((SessionId)obj)
                    : -1;

        public static bool operator == (SessionId lhs, SessionId rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(SessionId lhs, SessionId rhs) =>
            !lhs.Equals(rhs);

        public override bool Equals(object obj) =>
            obj is SessionId
                ? Equals((SessionId)obj)
                : false;
    }
}
