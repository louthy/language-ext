using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Numbers form a monoid under addition.
    /// </summary>
    /// <typeparam name="A">The type of the number being added.</typeparam>
    public struct Sum<NUM, A> : Monoid<A> where NUM : struct, Num<A>
    {
        public static readonly Sum<NUM, A> Inst = default(Sum<NUM, A>);

        [Pure]
        public A Append(A x, A y) =>
            plus<NUM, A>(x, y);

        [Pure]
        public A Empty() =>
            fromInteger<NUM, A>(0);
    }
}
