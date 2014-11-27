using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public struct ProcessId
    {
        internal ProcessName[] Parts;

        public ProcessId(string path)
        {
            if (path == null || path.Length == 0)
            {
                throw new InvalidProcessIdException();
            }
            if (path[0] != '/')
            {
                throw new InvalidProcessIdException();
            }
            try
            {
                Parts = path.Substring(1)
                            .Split('/')
                            .Select(p => new ProcessName(p))
                            .ToArray();
            }
            catch(InvalidProcessNameException)
            {
                throw new InvalidProcessIdException();
            }
        }

        public string Value =>
            Parts == null
                ? ""
                : "/" + String.Join("/", Parts);

        public ProcessName Name =>
            Parts == null
                ? ""
                : Parts.Last();

        public static implicit operator ProcessId(string value) =>
            new ProcessId(value);

        public override string ToString() =>
            Value;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public static ProcessId None => ActorContext.NoSender;
    }
}
