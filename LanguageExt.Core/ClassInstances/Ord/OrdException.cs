using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct OrdException : Ord<Exception>
    {
        public int GetHashCode(Exception x) =>
            default(HashableException).GetHashCode(x);

        public bool Equals(Exception x, Exception y) =>
            default(EqException).Equals(x, y);

        public int Compare(Exception x, Exception y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(x, null)) return -1;
            if (ReferenceEquals(y, null)) return 1;
            return x.Message.CompareTo(y.Message);
        }
    }
}
