using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public struct ProcessName
    {
        public readonly string Value;

        public ProcessName(Some<string> name)
        {
            if (name.Value.Length == 0)
            {
                throw new InvalidProcessNameException();
            }

            var invalid = System.IO.Path.GetInvalidFileNameChars();
            if ((from c in name.Value where invalid.Contains(c) select c).Count() > 0)
            {
                throw new InvalidProcessNameException();
            }
            Value = name.Value.ToLower();
        }

        public override string ToString() =>
            Value;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public static implicit operator ProcessName(string value) =>
            new ProcessName(value);
    }
}
