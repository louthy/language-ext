using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
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

        /// <summary>
        /// Generate new ProcessId that represents a child of this process ID
        /// </summary>
        /// <param name="name">Name of the child process</param>
        /// <returns>Process ID</returns>
        public ProcessId MakeChildId(ProcessName name) =>
            parts == null
                ? failwith<ProcessId>("ProcessId is None")
                : parts.Length == 0
                    ? new ProcessId("" + Sep + name)
                    : new ProcessId(Path + Sep + name);

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
            Path.CompareTo(other.Path);

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
            new ProcessId(Sep.ToString() + String.Join(Sep.ToString(), parts.Skip(count)));

        /// <summary>
        /// Take N elements of the path
        /// </summary>
        public ProcessId Take(int count) =>
            new ProcessId(Sep.ToString() + String.Join(Sep.ToString(), parts.Take(count)));
    }
}
