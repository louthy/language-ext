using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public struct ProcessId
    {
        internal readonly ProcessName[] Parts;
        public readonly string Value;
        public readonly ProcessName Name;

        public ProcessId(string path)
        {
            if (path == null || path.Length == 0)
            {
                throw new InvalidProcessIdException();
            }
            if (path[0] != Sep)
            {
                throw new InvalidProcessIdException();
            }
            if (path.Length == 1)
            {
                Parts = new ProcessName[0];
            }
            else
            {
                try
                {
                    Parts = path.Substring(1)
                                .Split(Sep)
                                .Select(p => new ProcessName(p))
                                .ToArray();
                }
                catch (InvalidProcessNameException)
                {
                    throw new InvalidProcessIdException();
                }
            }

            Value = Parts == null
                ? ""
                : Parts.Length == 0
                    ? Sep.ToString()
                    : Sep + String.Join(Sep.ToString(), Parts);

            if (path != "/")
            {
                Name = Parts == null
                    ? ""
                    : Parts.Length == 0
                        ? Sep.ToString()
                        : Parts.Last();
            }
            else
            {
                Name = "$";
            }
        }

        public ProcessId MakeChildId(ProcessName name) =>
            Parts == null
                ? failwith<ProcessId>("ProcessId is None")
                : Parts.Length == 0
                    ? new ProcessId("" + Sep + name)
                    : new ProcessId(Value + Sep + name);

        public static implicit operator ProcessId(string value) =>
            new ProcessId(value);

        public override string ToString() =>
            Value;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public bool IsValid => 
            Parts != null;

        public readonly static ProcessId NoSender =
            new ProcessId();

        public readonly static ProcessId None =
            new ProcessId();

        public readonly static char Sep = '/';
    }
}
