using System.Collections.Generic;

namespace LanguageExt
{
    internal class ReferenceEqualityComparer<A> : IEqualityComparer<A>
    {
        public bool Equals(A x, A y) =>
            ReferenceEquals(x, y);

        public int GetHashCode(A obj) =>
            obj?.GetHashCode() ?? 0;
    }
}
