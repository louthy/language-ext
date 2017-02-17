using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{

    /// <summary>
    /// Ordered values form a semigroup under minimum.
    /// </summary>
    /// <typeparam name="A">The type of the ordered values.</typeparam>
    public struct Min<ORD, A> : Semigroup<A> where ORD : struct, Ord<A>
    {
        public static readonly Min<ORD, A> Inst = default(Min<ORD, A>);

        [Pure]
        public A Append(A x, A y) =>
            lessOrEq<ORD, A>(x, y) ? x : y;
    }

    /// <summary>
    /// Ordered values form a semigroup under maximum.
    /// </summary>
    /// <typeparam name="A">The type of the ordered values.</typeparam>
    public struct Max<ORD, A> : Semigroup<A> where ORD : struct, Ord<A>
    {
        public static readonly Max<ORD, A> Inst = default(Max<ORD, A>);

        [Pure]
        public A Append(A x, A y) =>
            lessOrEq<ORD, A>(x, y) ? y : x;
    }
}
