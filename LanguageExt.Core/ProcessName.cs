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
    public struct ProcessName
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
    }
}
