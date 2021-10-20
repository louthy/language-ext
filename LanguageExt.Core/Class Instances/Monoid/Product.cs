using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Numbers form a monoid under addition.
    /// </summary>
    /// <typeparam name="A">The type of the number being added.</typeparam>
    public struct Product<NUM, A> : Monoid<A> where NUM : struct, Num<A>
    {
        public static readonly Product<NUM, A> Inst = default(Product<NUM, A>);

        [Pure]
        public A Append(A x, A y) =>
            product<NUM, A>(x, y);

        [Pure]
        public A Empty() =>
            fromInteger<NUM, A>(1);
    }
}
