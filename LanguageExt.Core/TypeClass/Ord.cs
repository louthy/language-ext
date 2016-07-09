using System;
using System.Linq;
using LanguageExt;
using LanguageExt.TypeClass;
using static LanguageExt.TypeClass.Prelude;

namespace LanguageExt.TypeClass
{
    public interface Ord<A> : Eq<A>
    {
        /// <summary>
        /// Returns true if x is less than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than or equal to y
        bool LessOrEq(A x, A y);

        /// <summary>
        /// Returns true if x is less than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than y
        bool LessThan(A x, A y);

        /// <summary>
        /// Returns true if x is greater than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than or equal to y
        bool GreaterOrEq(A x, A y);

        /// <summary>
        /// Returns true if x is greater than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than y
        bool GreaterThan(A x, A y);
    }

    /// <summary>
    /// Bound monadic value equality
    /// </summary>
    public struct OrdOption<ORD, A> : Ord<Option<A>> where ORD : struct, Ord<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Option<A> x, Option<A> y) =>
            x.IsNone() && y.IsNone()
                ? true
                : x.IsNone() || y.IsNone()
                    ? false
                    : (from a in x
                       from b in y
                       select @equals<ORD, A>(a, b))
                      .IfNone(false);

        public bool GreaterOrEq(Option<A> x, Option<A> y) =>
            (x.IsNone() && y.IsNone()) || 
            (x.IsSome() && y.IsSome()) || 
            (x.IsSome() && y.IsNone())
                ? true
                : x.IsNone() && y.IsSome()
                    ? false
                    : (from a in x
                       from b in y
                       select greaterOrEq<ORD, A>(a, b))
                      .IfNone(false);

        public bool GreaterThan(Option<A> x, Option<A> y) =>
            (x.IsSome() && y.IsNone())
                ? true
                : x.IsNone() && y.IsSome()
                    ? false
                    : (from a in x
                       from b in y
                       select greaterThan<ORD, A>(a, b))
                      .IfNone(false);

        public bool LessOrEq(Option<A> x, Option<A> y) =>
            !GreaterThan(x, y);

        public bool LessThan(Option<A> x, Option<A> y) =>
            !GreaterOrEq(x, y);
    }
}
