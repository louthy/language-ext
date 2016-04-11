using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

namespace LanguageExt
{
    /// <summary>
    /// <para>
    /// Process identifier
    /// </para>
    /// <para>
    /// Use this to 'tell' a message to a process.  It can be serialised and passed around
    /// without concerns for internals.
    /// </para>
    /// </summary>
    public struct ProcessId : IEquatable<ProcessId>, IComparable<ProcessId>, IComparable
    {
        readonly ProcessName[] parts;
        readonly ProcessName name;

        /// <summary>
        /// The Process system qualifier
        /// </summary>
        public readonly SystemName System;

        /// <summary>
        /// Absolute path of the process ID
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="path">Path of the process, in the form: /name/name/name </param>
        [JsonConstructor]
        public ProcessId(string path)
        {
            var res = TryParse(path).IfLeft(ex => raise<ProcessId>(ex));

            parts = res.parts;
            name = res.name;
            Path = res.Path;
            System = res.System;
        }

        ProcessId(string path, SystemName system)
        {
            var res = TryParse(path).IfLeft(ex => raise<ProcessId>(ex));

            parts = res.parts;
            name = res.name;
            Path = res.Path;
            System = system;
        }

        ProcessId(ProcessName[] parts, SystemName system, ProcessName name, string path)
        {
            this.parts = parts;
            this.name = name;
            this.Path = path;
            this.System = system;
        }

        public static Either<Exception, ProcessId> TryParse(string path)
        {
            if (path == null || path.Length == 0)
            {
                return new InvalidProcessIdException();
            }

            var system = "";

            if( path.StartsWith("//"))
            {
                var end = path.IndexOf('/', 2);
                end = end == -1
                    ? path.IndexOf("@", 2)
                    : end;

                if(end == -1)
                {
                    return new InvalidProcessIdException($"Invalid ProcessId ({path}), nothing following the system specifier");
                }

                system = path.Substring(2, end - 2);
                path = path.Substring(end);
            }

            if (path[0] == '@')
            {
                return ParseRegisteredProcess(path.Substring(1));
            }

            if (path[0] != Sep)
            {
                return new InvalidProcessIdException();
            }

            ProcessName[] parts = null;
            ProcessName name;
            string finalPath = null;

            if (path.Length == 1)
            {
                parts = new ProcessName[0];
            }
            else
            {
                try
                {
                    parts = SplitOnSep(path).ToArray();
                }
                catch (InvalidProcessNameException)
                {
                    return new InvalidProcessIdException();
                }
            }

            finalPath = parts == null
                ? ""
                : parts.Length == 0
                    ? Sep.ToString()
                    : Sep + String.Join(Sep.ToString(), parts);

            if (path != Sep.ToString())
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

            return new ProcessId(parts, system == "" ? default(SystemName) : system, name, finalPath);
        }

        static ProcessId ParseRegisteredProcess(string name) =>
            TryParseRegisteredProcess(name)
                .Match(
                    Right: r => r,
                    Left: ex => raise<ProcessId>(ex));

        static Either<Exception, ProcessId> TryParseRegisteredProcess(string name)
        {
            if (name.Length == 0) return new InvalidProcessNameException("Registerd process name has nothing following the '@'");

            var parts = name.Split(':');
            if (parts.Length == 0) return new InvalidProcessNameException();
            if (parts.Length > 2) return new InvalidProcessNameException("Too many ':' in the registered process name");

            if ((from p in parts
                 from c in p
                 where ProcessName.InvalidNameChars.Contains(c)
                 select c)
                .Any())
            {
                return new InvalidProcessNameException();
            }

            if (parts.Length == 1)
            {
                return Process.find(Role.Current, parts[0]);
            }
            else
            {
                return Process.find(parts[0], parts[1]);
            }
        }

        /// <summary>
        /// Traverses a string extracting an enumerable of ProcessNames that
        /// make up the path.
        /// </summary>
        /// <param name="path">Path to traverse</param>
        /// <returns>Enumerable of ProcessNames</returns>
        static IEnumerable<ProcessName> SplitOnSep(string path)
        {
            var builder = new StringBuilder();
            var selection = false;
            foreach (var c in path)
            {
                if (selection)
                {
                    if (c == ']')
                    {
                        selection = false;
                    }
                    builder.Append(c);
                }
                else
                {
                    switch (c)
                    {
                        case '[':
                            selection = true;
                            if (builder.Length > 0)
                            {
                                throw new InvalidProcessIdException($"Selection types must cover entire names. ie. {Sep}[{Sep}root,{Sep}node]{Sep}user, not {Sep}test[{Sep}root,{Sep}node]{Sep}user");
                            }
                            builder.Append('[');
                            break;

                        case Sep:
                            if (builder.Length > 0)
                            {
                                yield return new ProcessName(builder.ToString());
                                builder.Length = 0;
                            }
                            break;

                        default:
                            builder.Append(c);
                            break;
                    }
                }
            }
            if (selection)
            {
                throw new InvalidProcessIdException("ProcessId has an opening selection: '[', but no selection close: ']'");
            }
            if (builder.Length > 0)
            {
                yield return new ProcessName(builder.ToString());
            }
        }

        /// <summary>
        /// Generate new ProcessId that represents a child of this process ID
        /// </summary>
        /// <returns>Process ID</returns>
        public ProcessId this[ProcessName child] => 
            Child(child);

        /// <summary>
        /// Generate new ProcessId that represents a child of this process ID
        /// </summary>
        /// <returns>Process ID</returns>
        public ProcessId this[IEnumerable<ProcessId> child] =>
            Child(child);

        /// <summary>
        /// Generate new ProcessId that represents a child of this process ID
        /// </summary>
        /// <param name="name">Name of the child process</param>
        /// <returns>Process ID</returns>
        public ProcessId Child(ProcessName name) =>
            parts == null
                ? failwith<ProcessId>("ProcessId is None")
                : parts.Length == 0
                    ? new ProcessId("" + Sep + name, System)
                    : new ProcessId(Path + Sep + name, System);

        /// <summary>
        /// Generate new ProcessId that represents a child of this process ID
        /// </summary>
        /// <param name="name">Name of the child process</param>
        /// <returns>Process ID</returns>
        public ProcessId Child(IEnumerable<ProcessId> name) =>
            parts == null
                ? failwith<ProcessId>("ProcessId is None")
                : parts.Length == 0
                    ? new ProcessId("" + Sep + ProcessName.FromSelection(name), System)
                    : new ProcessId(Path + Sep + ProcessName.FromSelection(name), System);

        /// <summary>
        /// Returns true if the ProcessId represents a selection of N process
        /// paths
        /// </summary>
        public bool IsSelection =>
            parts == null || parts.Length == 0
                ? false
                : parts[0].IsSelection;

        /// <summary>
        /// If this ProcessId represents a selection of N process paths then
        /// this function will return those paths as separate ProcessIds.
        /// </summary>
        /// <returns>An enumerable of ProcessIds representing the selection.
        /// Zero ProcessIds will be returned if the ProcessId is invalid.
        /// One ProcessId will be returned if this ProcessId doesn't represent
        /// a selection.
        /// </returns>
        public IEnumerable<ProcessId> GetSelection()
        {
            var self = this;

            return parts == null || parts.Length == 0
                ? new ProcessId[0]
                : parts[0].IsSelection
                    ? from x in parts[0].GetSelection()
                      from y in x.Append(self.Skip(1)).GetSelection()
                      select y
                    : new ProcessId[] { self };
        }

        /// <summary>
        /// Get the parent ProcessId
        /// </summary>
        /// <returns>Parent process ID</returns>
        public ProcessId Parent() =>
            parts == null
                ? failwith<ProcessId>("ProcessId is None")
                : parts.Length == 0
                    ? failwith<ProcessId>("ProcessId doesn't have a parent")
                    : Take(parts.Length - 1);

        /// <summary>
        /// Implicit conversion from a string to a ProcessId
        /// </summary>
        /// <param name="value">String representation of the process ID</param>
        public static implicit operator ProcessId(string value) =>
            value == null
                ? ProcessId.NoSender
                : new ProcessId(value);

        /// <summary>
        /// Convert the ProcessId to a string
        /// </summary>
        /// <returns>String representation of the process ID</returns>
        public override string ToString() =>
            System.IsValid
                ? $"//{System}{Path}"
                : Path;

        /// <summary>
        /// Hash code of process ID
        /// Do not rely on this between application sessions
        /// </summary>
        /// <returns>Integer hash code</returns>
        public override int GetHashCode() =>
            Path.GetHashCode();

        /// <summary>
        /// Returns true if this is a valid process ID
        /// </summary>
        public bool IsValid => 
            parts != null;

        /// <summary>
        /// Get the name of the process
        /// </summary>
        /// <returns></returns>
        public ProcessName GetName() =>
            name;

        /// <summary>
        /// NoSender process ID
        /// </summary>
        public readonly static ProcessId NoSender =
            new ProcessId();

        /// <summary>
        /// None process ID
        /// </summary>
        public readonly static ProcessId None =
            new ProcessId();

        /// <summary>
        /// Process ID name separator
        /// </summary>
        public const char Sep = '/';

        /// <summary>
        /// Equality check
        /// </summary>
        public bool Equals(ProcessId other) =>
            Path.Equals(other.Path);

        /// <summary>
        /// Equality check
        /// </summary>
        public override bool Equals(object obj) =>
            obj is ProcessId
                ? Equals((ProcessId)obj)
                : false;

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(ProcessId lhs, ProcessId rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        public static bool operator !=(ProcessId lhs, ProcessId rhs) =>
            !lhs.Equals(rhs);

        /// <summary>
        /// Compare
        /// </summary>
        public int CompareTo(ProcessId other) =>
            string.Compare(Path, other.Path, StringComparison.Ordinal);

        /// <summary>
        /// Compare
        /// </summary>
        public int CompareTo(object obj) =>
            obj is ProcessId
                ? CompareTo((ProcessId)obj)
                : -1;

        /// <summary>
        /// Remove path elements from the start of the path
        /// </summary>
        public ProcessId Skip(int count) =>
            new ProcessId(Top + String.Join(Sep.ToString(), parts.Skip(count)), System);

        /// <summary>
        /// Take N elements of the path
        /// </summary>
        public ProcessId Take(int count) =>
            new ProcessId(Top + String.Join(Sep.ToString(), parts.Take(count)), System);

        /// <summary>
        /// Take head of path
        /// </summary>
        public ProcessId Head() =>
            Take(1);

        /// <summary>
        /// Take tail of path
        /// </summary>
        public ProcessId Tail() =>
            Skip(1);

        /// <summary>
        /// Number of parts to the name
        /// </summary>
        /// <returns></returns>
        public int Count() => 
            parts == null 
                ? 0 
                : parts.Length;

        /// <summary>
        /// Append one process ID to another
        /// </summary>
        public ProcessId Append(ProcessId pid) =>
            IsValid && pid.IsValid
                ? new ProcessId(Path + pid.Path, pid.System)
                : IsValid
                    ? pid
                    : raise<ProcessId>(new InvalidProcessIdException());

        /// <summary>
        /// Absolute root of the process system
        /// </summary>
        public static readonly ProcessId Top = 
            new ProcessId(Sep.ToString());

        /// <summary>
        /// Set the Process system that this ProcessId belongs to
        /// </summary>
        public ProcessId SetSystem(SystemName system) =>
            new ProcessId(Path, system);

        static R failwith<R>(string message)
        {
            throw new Exception(message);
        }

        static R raise<R>(Exception ex)
        {
            throw ex;
        }
    }
}
