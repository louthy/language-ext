using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    ///  Numbers form a monoid under multiplication.
    /// </summary>
    /// <typeparam name="A">The type of the number being multiplied.</typeparam>
    public struct Product<NUM, A> : Semigroup<A> where NUM : struct, Num<A>
    {
        public A Append(A x, A y) =>
            product<NUM, A>(x, y);

        public A Empty() =>
            fromInteger<NUM, A>(1);
    }
}
