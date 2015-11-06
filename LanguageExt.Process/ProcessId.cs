using System;
using System.Linq;
using Newtonsoft.Json;

namespace LanguageExt
{
    /// <summary>
    /// Process identifier
    /// Use this to 'tell' a message to a process.  It can be serialised and passed around
    /// without concerns for internals.
    /// </summary>
    public struct ProcessId : IEquatable<ProcessId>, IComparable<ProcessId>, IComparable
    {
        readonly ProcessName[] parts;
        readonly ProcessName name;

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
        /// <param name="name">Name of the child process</param>
        /// <returns>Process ID</returns>
        public ProcessId Child(ProcessName name) =>
            parts == null
                ? failwith<ProcessId>("ProcessId is None")
                : parts.Length == 0
                    ? new ProcessId("" + Sep + name)
                    : new ProcessId(Path + Sep + name);

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
            new ProcessId(value);

        /// <summary>
        /// Convert the ProcessId to a string
        /// </summary>
        /// <returns>String representation of the process ID</returns>
        public override string ToString() =>
            Path;

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
        public readonly static char Sep = '/';

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
            new ProcessId(Top + String.Join(Sep.ToString(), parts.Skip(count)));

        /// <summary>
        /// Take N elements of the path
        /// </summary>
        public ProcessId Take(int count) =>
            new ProcessId(Top + String.Join(Sep.ToString(), parts.Take(count)));

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
                ? Path + pid.Path
                : raise<ProcessId>(new InvalidProcessIdException());

        /// <summary>
        /// Absolute root of the process system
        /// </summary>
        public static readonly ProcessId Top = 
            new ProcessId(Sep.ToString());

        private static R failwith<R>(string message)
        {
            throw new Exception(message);
        }

        private static R raise<R>(Exception ex)
        {
            throw ex;
        }
    }
}
