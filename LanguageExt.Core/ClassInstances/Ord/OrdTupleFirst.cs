using LanguageExt.TypeClasses;
using System;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Ord instance for a pair tuple.  It orders using the first
    /// item in the tuple only and the provided OrdA.
    /// </summary>
    public struct OrdTupleFirst<OrdA, A, B> : Ord<ValueTuple<A, B>> where OrdA : struct, Ord<A>
    {
        public int Compare((A, B) x, (A, B) y) =>
            default(OrdA).Compare(x.Item1, y.Item1);

        public bool Equals((A, B) x, (A, B) y) =>
            default(OrdA).Equals(x.Item1, y.Item1);

        public int GetHashCode((A, B) x) =>
            x.GetHashCode();
    }
}
