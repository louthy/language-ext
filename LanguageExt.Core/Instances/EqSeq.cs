using LanguageExt.TypeClasses;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    public struct EqSeq<EQ, A> : Eq<Seq<A>> where EQ : struct, Eq<A>
    {
        public bool Equals(Seq<A> x, Seq<A> y)
        {
            if (ReferenceEquals(x, y)) return true;

            var enumx = x.AsEnumerable().GetEnumerator();
            var enumy = y.AsEnumerable().GetEnumerator();
            while(true)
            {
                bool a = enumx.MoveNext();
                bool b = enumy.MoveNext();
                if (a != b) return false;
                if (!a && !b) return true;
                if (default(EQ).Equals(enumx.Current, enumy.Current)) return false;
            }
        }
    }
}
