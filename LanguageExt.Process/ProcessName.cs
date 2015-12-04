using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Name of a Process in LanguageExt.Process system.
    /// </summary>
    /// <remarks>
    /// It enforces the rules for process names.  Process have the same rules
    /// as file-names Wn windows.  
    /// </remarks>
    public struct ProcessName : IEquatable<ProcessName>, IComparable<ProcessName>, IComparable
    {
        public readonly string Value;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value">Process name</param>
        [JsonConstructor]
        public ProcessName(string value)
        {
            if (value == null || value.Length == 0)
            {
                throw new InvalidProcessNameException();
            }

            if (value.Length == 0)
            {
                throw new InvalidProcessNameException();
            }

            var invalid = System.IO.Path.GetInvalidFileNameChars();
            if ((from c in value where invalid.Contains(c) select c).Count() > 0)
            {
                throw new InvalidProcessNameException();
            }
            Value = value.ToLower();
        }

        public override string ToString() =>
            Value;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public static implicit operator ProcessName(string value) =>
            new ProcessName(value);

        public bool Equals(ProcessName other) =>
            Value.Equals(other.Value);

        public int CompareTo(ProcessName other) =>
            String.Compare(Value, other.Value, StringComparison.Ordinal);

        public int CompareTo(object obj) =>
            obj == null
                ? -1
                : obj is ProcessName
                    ? CompareTo((ProcessName)obj)
                    : -1;

        public static bool operator == (ProcessName lhs, ProcessName rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(ProcessName lhs, ProcessName rhs) =>
            !lhs.Equals(rhs);

        public override bool Equals(object obj) =>
            obj is ProcessName
                ? Equals((ProcessName)obj)
                : false;
    }
}
