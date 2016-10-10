using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class Versioned
    {
        public static VectorClock.Occured Compare<T>(Versioned<T> lhs, Versioned<T> rhs) =>
            VectorClock.Compare(lhs.Version, rhs.Version);
    }

    public class Versioned<T> : IEquatable<Versioned<T>>, IComparable<Versioned<T>>
    {
        public readonly T Value;
        public readonly VectorClock Version = VectorClock.Empty;

        public Versioned(T value, VectorClock version)
        {
            Value = value;
            Version = version;
        }

        public Versioned<T> SetValue(int nodeId, T value) =>
            new Versioned<T>(value, Version.Incr(nodeId));

        public override int GetHashCode() =>
            Tuple(Value, Version).GetHashCode();

        public bool Equals(Versioned<T> rhs) =>
            ReferenceEquals(this, rhs) || (Version == rhs.Version && Value.Equals(rhs.Value));

        public int CompareTo(Versioned<T> rhs) =>
            (int)VectorClock.Compare(Version, rhs.Version);
    }
}
