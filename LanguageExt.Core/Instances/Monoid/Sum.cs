using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Numbers form a monoid under addition.
    /// </summary>
    /// <typeparam name="A">The type of the number being added.</typeparam>
    public struct Sum<NUM, A> : Monoid<A> where NUM : struct, Num<A>
    {
        public A Append(A x, A y) =>
            plus<NUM, A>(x, y);

        public A Empty() =>
            fromInteger<NUM, A>(0);
    }
}
