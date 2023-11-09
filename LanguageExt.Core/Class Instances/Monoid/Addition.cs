#nullable enable
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Numbers form a monoid under addition.
    /// </summary>
    /// <typeparam name="A">The type of the number being added.</typeparam>
    public struct Addition<NUM, A> : Monoid<A> where NUM : struct, Num<A>
    {
        public static readonly Addition<NUM, A> Inst = default;

        [Pure]
        public A Append(A x, A y) =>
            plus<NUM, A>(x, y);

        [Pure]
        public A Empty() =>
            fromInteger<NUM, A>(0);
    }
}
