using Newtonsoft.Json;
using System;
using System.Linq;
using LanguageExt.Instances;
using static LanguageExt.TypeClass;

namespace LanguageExt
{
    /// <summary>
    /// Name of a Process System in LanguageExt.Process system.
    /// </summary>
    /// <remarks>
    /// It enforces the rules for system names.  SystemNames have the same rules
    /// as file-names in windows.  
    /// </remarks>
    public struct SystemName : IEquatable<SystemName>, IComparable<SystemName>, IComparable
    {
        readonly string value;

        public static readonly char[] InvalidNameChars =
            MArray<char>.Inst.Append(System.IO.Path.GetInvalidFileNameChars(), new[] { '[', ']', ',' }).Distinct().ToArray();

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value">Process name</param>
        [JsonConstructor]
        public SystemName(string value)
        {
            var res = TryParse(value).IfLeft(ex => Prelude.raise<SystemName>(ex));
            this.value = res.Value;
        }

        private SystemName(string value, bool _)
        {
            this.value = value;
        }

        public string Value =>
            value == null
                ? ""
                : value;

        public bool IsValid =>
            !String.IsNullOrEmpty(Value);

        public static Either<Exception, SystemName> TryParse(string value)
        {
            if (value == null || value.Length == 0)
            {
                return new SystemName("", true);
            }

            value = value.ToLower();
            if ((from c in value where InvalidNameChars.Contains(c) select c).Any())
            {
                return new InvalidSystemNameException();
            }
            return new SystemName(value, true);
        }

        public override string ToString() =>
            Value;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public static implicit operator SystemName(string value) =>
            new SystemName(value);

        public bool Equals(SystemName other) =>
            Value.Equals(other.Value);

        public int CompareTo(SystemName other) =>
            String.Compare(Value, other.Value, StringComparison.Ordinal);

        public int CompareTo(object obj) =>
            obj == null
                ? -1
                : obj is SystemName
                    ? CompareTo((SystemName)obj)
                    : -1;

        public static bool operator == (SystemName lhs, SystemName rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(SystemName lhs, SystemName rhs) =>
            !lhs.Equals(rhs);

        public override bool Equals(object obj) =>
            obj is SystemName
                ? Equals((SystemName)obj)
                : false;
    }
}
