using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Compare the equality of any type in the Either type-class
    /// </summary>
    public struct EqChoice<EQA, EQB, A, B> : Eq<Choice<A, B>>
        where EQA : struct, Eq<A>
        where EQB : struct, Eq<B>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Choice<A, B> x, Choice<A, B> y) =>
            x.Match( x,
                Choice1: a =>
                    y.Match(y, Choice1: b => equals<EQA, A>(a, b),
                               Choice2: _ => false),
                Choice2: a =>
                    y.Match(y, Choice1: _ => false,
                               Choice2: b => equals<EQB, B>(a, b)));
    }
}
