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

        public static readonly char[] InvalidNameChars =
            System.IO.Path.GetInvalidFileNameChars().Append(new[] { '[', ']', ',' }).Distinct().ToArray();

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

            value = value.ToLower();
            if (value[0] == '[' && value[value.Length - 1] == ']')
            {
                // Validate the inner ProcessIds
                value.Substring(1, value.Length - 2).Split(',').Map(x => new ProcessId(x)).ToList();
            }
            else
            {
                if ((from c in value where InvalidNameChars.Contains(c) select c).Any())
                {
                    throw new InvalidProcessNameException();
                }
            }
            Value = value;
        }

        private ProcessName(IEnumerable<ProcessId> values)
        {
            if(values == null) throw new InvalidProcessNameException();
            Value = $"[{String.Join(",", values)}]";
        }

        public static ProcessName FromSelection(IEnumerable<ProcessId> pids) =>
            new ProcessName(pids);

        public bool IsSelection =>
            Value != null && Value.Length > 0 && Value[0] == '[' && Value[Value.Length - 1] == ']';

        public IEnumerable<ProcessId> GetSelection() =>
            IsSelection
                ? Value.Substring(1, Value.Length - 2).Split(',').Map(x => new ProcessId(x))
                : new ProcessId[0];

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
