using LanguageExt.TypeClasses;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Que<A> equality
    /// </summary>
    public struct EqQueue<A> : Eq<Que<A>>
    {
        public static readonly EqQueue<A> Inst = default(EqQueue<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Que<A> x, Que<A> y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (x.Count != y.Count) return false;
            return x == y;
        }
    }
}
