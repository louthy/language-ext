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

        public ProcessId(string path)
        {
            if (path == null || path.Length == 0)
            {
                throw new InvalidProcessIdException();
            }
            if (path[0] != ProcessId.Sep)
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
                                .Split(ProcessId.Sep)
                                .Select(p => new ProcessName(p))
                                .ToArray();
                }
                catch (InvalidProcessNameException)
                {
                    throw new InvalidProcessIdException();
                }
            }
        }

        public ProcessId MakeChildId(ProcessName name) =>
            Parts == null
                ? failwith<ProcessId>("ProcessId is None")
                : Parts.Length == 0
                    ? new ProcessId("" + ProcessId.Sep + name)
                    : new ProcessId(Value + ProcessId.Sep + name);

        public string Value =>
            Parts == null
                ? ""
                : Parts.Length == 0
                    ? ProcessId.Sep.ToString()
                    : ProcessId.Sep + String.Join(ProcessId.Sep.ToString(), Parts);

        public ProcessName Name =>
            Parts == null
                ? ""
                : Parts.Length == 0
                    ? ProcessId.Sep.ToString()
                    : Parts.Last();

        public static implicit operator ProcessId(string value) =>
            new ProcessId(value);

        public override string ToString() =>
            Value;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public bool IsValid => Parts != null;

        public static ProcessId NoSender => 
            ActorContext.NoSender;

        public static ProcessId None =>
            new ProcessId();

        public static char Sep => '/';
    }
}
