using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Vector clock
    /// </summary>
    public class VectorClock : IEquatable<VectorClock>
    {
        /// <summary>
        /// The result of comparing two times
        ///     either t1 is before t2, 
        ///     t1 is after t2, 
        ///     or t1 happens concurrently to t2
        /// </summary>
        public enum Occured
        {
            /// <summary>
            /// Event occurred before
            /// </summary>
            Before = -1,

            /// <summary>
            /// Event occurred after
            /// </summary>
            After = 1,

            /// <summary>
            /// Event occurred independantly
            /// </summary>
            Concurrently = 0
        }
        private const int MaxVersions = Int16.MaxValue;
        public readonly Lst<ClockEntry> Versions;
        public static readonly VectorClock Empty = new VectorClock();

        /// <summary>
        /// Ctor
        /// </summary>
        VectorClock() : this(List.empty<ClockEntry>())
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        VectorClock(Lst<ClockEntry> versions)
        {
            Versions = versions;
        }

        /// <summary>
        /// Increment the version info associated with the given node
        /// </summary>
        /// <param name="nodeId">Node to increment</param>
        public VectorClock Incr(int nodeId)
        {
            // stop on the index greater or equal to the node
            int index = 0;
            bool found = false;
            var versions = Versions;

            foreach(var version in versions)
            {
                if (version.NodeId == nodeId)
                {
                    found = true;
                    break;
                }
                else if (version.NodeId > nodeId)
                {
                    break;
                }
                index++;
            }

            if (found)
            {
                versions = versions.SetItem(index, versions[index].Incr());
            }
            else if (index < versions.Count - 1)
            {
                versions = versions.Insert(index, new ClockEntry(nodeId, 1));
            }
            else
            {
                // we don't already have a version for this, so add it
                if (versions.Count > MaxVersions) throw new ArgumentOutOfRangeException("Vector clock is full");
                versions.Add(new ClockEntry(nodeId, 1));
            }
            return new VectorClock(versions);
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("version(");

            if (Versions.Count > 0)
            {
                for (int i = 0; i < Versions.Count - 1; i++)
                {
                    builder.Append(Versions[i]);
                    builder.Append(", ");
                }
                builder.Append(Versions[Versions.Count - 1]);
            }
            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        /// Merge clocks
        /// </summary>
        public VectorClock Merge(VectorClock clock)
        {
            int i = 0;
            int j = 0;

            var versions = Versions.ToArray();
            var clockVersions = clock.Versions.ToArray();
            var newClockVersions = List.empty<ClockEntry>();

            while (i < versions.Count && j < clockVersions.Count)
            {
                var v1 = versions[i];
                var v2 = clockVersions[j];

                if (v1.NodeId == v2.NodeId)
                {
                    var nc = new ClockEntry(v1.NodeId, (int)Math.Max(v1.Version, v2.Version));
                    newClockVersions = newClockVersions.Add(nc);

                    i++;
                    j++;
                }
                else if (v1.NodeId < v2.NodeId)
                {
                    newClockVersions = newClockVersions.Add(v1);
                    i++;
                }
                else
                {
                    newClockVersions = newClockVersions.Add(v2);
                    j++;
                }
            }

            // Okay now there may be leftovers on one or the other list remaining
            for (int k = i; k < versions.Count; k++)
            {
                newClockVersions = newClockVersions.Add(versions[k]);
            }
            for (int k = j; k < clockVersions.Count; k++)
            {
                newClockVersions = newClockVersions.Add(clockVersions[k]);
            }
            return new VectorClock(newClockVersions);
        }

        /// <summary>
        /// <para>
        /// Is this reflexive, antisymetic, and transitive? Compare two VectorClocks,
        /// the outcomes will be one of the following: 
        /// </para>
        /// <para>
        ///     * Clock 1 is /before/ clock 2
        ///         if there exists an i such that c1(i) &lt;= c(2) and there does not 
        ///         exist a j such that c1(j) > c2(j). 
        /// </para>
        /// <para>
        ///     * Clock 1 is /concurrent/ to clock 2 if there exists an i, j such that 
        ///         c1(i) &lt; c2(i) and c1(j) > c2(j)
        /// </para>
        /// <para>
        ///     * Clock 1 is /after/ clock 2  otherwise
        /// </para>
        /// </summary>
        /// <param name="v1">The first VectorClock</param>
        /// <param name="v2">The second VectorClock</param>
        /// <returns>Whether the change occured before, after or concurrently</returns>
        public static Occured Compare(VectorClock v1, VectorClock v2)
        {
            if (v1 == null || v2 == null)
                throw new ArgumentException("Can't compare null vector clocks!");

            // We do two checks: v1 <= v2 and v2 <= v1 if both are true then
            bool v1Bigger = false;
            bool v2Bigger = false;
            int p1 = 0;
            int p2 = 0;

            while (p1 < v1.Versions.Count && p2 < v2.Versions.Count)
            {
                ClockEntry ver1 = v1.Versions[p1];
                ClockEntry ver2 = v2.Versions[p2];

                if (ver1.NodeId == ver2.NodeId)
                {
                    if (ver1.Version > ver2.Version)
                        v1Bigger = true;
                    else if (ver2.Version > ver1.Version)
                        v2Bigger = true;
                    p1++;
                    p2++;
                }
                else if (ver1.NodeId > ver2.NodeId)
                {
                    // since ver1 is bigger that means it is missing a version that ver2 has
                    v2Bigger = true;
                    p2++;
                }
                else
                {
                    // this means ver2 is bigger which means it is missing a version ver1 has
                    v1Bigger = true;
                    p1++;
                }
            }

            // Check for left overs
            if (p1 < v1.Versions.Count)
            {
                v1Bigger = true;
            }
            else if (p2 < v2.Versions.Count)
            {
                v2Bigger = true;
            }

            // This is the case where they are equal, return BEFORE arbitrarily
            if (!v1Bigger && !v2Bigger)
            {
                return Occured.Before;
            }
            // This is the case where v1 is a successor clock to v2
            else if (v1Bigger && !v2Bigger)
            {
                return Occured.After;
            }
            // This is the case where v2 is a successor clock to v1
            else if (!v1Bigger && v2Bigger)
            {
                return Occured.Before;
            }
            // This is the case where both clocks are parallel to one another
            else
            {
                return Occured.Concurrently;
            }
        }

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) =>
            obj is VectorClock
                ? Equals((VectorClock)obj)
                : false;

        public bool Equals(VectorClock rhs)
        {
            if (ReferenceEquals(this, rhs)) return true;
            if (ReferenceEquals(rhs, null)) return false;
            if (Versions.Count != rhs.Versions.Count) return false;
            if (Versions.Zip(rhs.Versions, (x, y) => x == y).Exists(x => !x)) return false;
            return true;
        }

        public override int GetHashCode() =>
            Versions.Fold(13, (s, x) => s * Math.Max(1, x.GetHashCode()));
    }

    public class ClockEntry
    {
        public readonly int NodeId;
        public readonly long Version;

        public ClockEntry(int nodeId, long version)
        {
            NodeId = nodeId;
            Version = version;
        }

        public ClockEntry Incr() =>
            new ClockEntry(NodeId, Version + 1);

        public override int GetHashCode() =>
            Tuple(NodeId, Version).GetHashCode();

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            var clockEntry = obj as ClockEntry;
            return clockEntry == null
                ? false
                : clockEntry.NodeId == NodeId && clockEntry.Version == Version;
        }

        public override string ToString() =>
            $"{NodeId}:{Version}";
    }
}