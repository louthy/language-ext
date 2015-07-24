using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using Newtonsoft.Json;

namespace LanguageExt
{
    public struct ProcessId
    {
        readonly ProcessName[] parts;
        readonly ProcessName name;

        public readonly string Path;

        [JsonConstructor]
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
                parts = new ProcessName[0];
            }
            else
            {
                try
                {
                    parts = path.Substring(1)
                                .Split(Sep)
                                .Select(p => new ProcessName(p))
                                .ToArray();
                }
                catch (InvalidProcessNameException)
                {
                    throw new InvalidProcessIdException();
                }
            }

            Path = parts == null
                ? ""
                : parts.Length == 0
                    ? Sep.ToString()
                    : Sep + String.Join(Sep.ToString(), parts);

            if (path != "/")
            {
                name = parts == null
                    ? ""
                    : parts.Length == 0
                        ? Sep.ToString()
                        : parts.Last();
            }
            else
            {
                name = "$";
            }
        }

        public ProcessId MakeChildId(ProcessName name) =>
            parts == null
                ? failwith<ProcessId>("ProcessId is None")
                : parts.Length == 0
                    ? new ProcessId("" + Sep + name)
                    : new ProcessId(Path + Sep + name);

        public static implicit operator ProcessId(string value) =>
            new ProcessId(value);

        public override string ToString() =>
            Path;

        public override int GetHashCode() =>
            Path.GetHashCode();

        public bool IsValid => 
            parts != null;

        public ProcessName GetName() =>
            name;

        public readonly static ProcessId NoSender =
            new ProcessId();

        public readonly static ProcessId None =
            new ProcessId();

        public readonly static char Sep = '/';
    }
}
