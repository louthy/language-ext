using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Collections;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Represents a range of values
    /// </summary>
    /// <typeparam name="SELF">The type that is deriving from this type</typeparam>
    /// <typeparam name="MonoidOrdA">Monoid of A class instance</typeparam>
    /// <typeparam name="A">Bound values type</typeparam>
    [Serializable]
    public class Range<SELF, MonoidOrdA, A> : IEnumerable<A>
        where SELF : Range<SELF, MonoidOrdA, A>
        where MonoidOrdA : struct, Monoid<A>, Ord<A>, Arithmetic<A>
    {
        static Func<A, A, A, SELF> Ctor = IL.Ctor<A, A, A, SELF>();
        static MonoidOrdA M = new MonoidOrdA();

        /// <summary>
        /// Zero range using MonoidOrdA.Empty()
        /// </summary>
        public readonly static SELF Zero = FromMinMax(default(MonoidOrdA).Empty(), default(MonoidOrdA).Empty(), default(MonoidOrdA).Empty());

        /// <summary>
        /// First value in the range
        /// </summary>
        public readonly A From;

        /// <summary>
        /// Last (inclusive) value in the range
        /// </summary>
        public readonly A To;

        /// <summary>
        /// Step size to the next item in the range
        /// </summary>
        public readonly A Step;

        /// <summary>
        /// True if adding step to n makes the resulting value greater than n
        /// </summary>
        public readonly bool StepIsAscending;

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public SeqCase<A> Case =>
            Prelude.Seq(this).Case;

        /// <summary>
        /// Construct a new range
        /// </summary>
        /// <param name="from">The minimum value in the range</param>
        /// <param name="to">The maximum value in the range</param>
        /// <param name="step">The size of each step in the range</param>
        [Pure]
        public static SELF FromMinMax(A min, A max, A step) =>
            Ctor(min, max, step);

        /// <summary>
        /// Construct a new range
        /// </summary>
        /// <param name="from">The minimum value in the range</param>
        /// <param name="count">The number of items in the range</param>
        /// <param name="step">The size of each step in the range</param>
        [Pure]
        public static SELF FromCount(A min, A count, A step) =>
            Ctor(min, M.Plus(min, M.Subtract(M.Product(count, step), step)), step);

        /// <summary>
        /// Construct a range
        /// </summary>
        /// <param name="from">The minimum value in the range</param>
        /// <param name="to">The maximum value in the range</param>
        /// <param name="step">The size of each step in the range</param>
        [Pure]
        protected Range(A from, A to, A step)
        {
            From = from;
            To = to;
            Step = step;
            StepIsAscending = M.Compare(step, M.Empty()) >= 0;
        }

        /// <summary>
        /// Returns true if the value provided is in range
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <returns>True if the value provided is in range</returns>
        [Pure]
        public bool InRange(A value)
        {
            var from = M.Compare(From, To) > 0 ? To : From;
            var to   = M.Compare(From, To) > 0 ? From : To;

            return M.Compare(value, from) >= 0 &&
                   M.Compare(value, to) <= 0;
        }

        /// <summary>
        /// Returns true if the range provided overlaps this range
        /// </summary>
        /// <param name="other">The range to test</param>
        /// <returns>True if the range provided overlaps this range</returns>
        [Pure]
        public bool Overlaps(SELF other)
        {
            var xfrom = M.Compare(From, To) > 0 ? To : From;
            var xto   = M.Compare(From, To) > 0 ? From : To;
            var yfrom = M.Compare(other.From, other.To) > 0 ? other.To : other.From;
            var yto   = M.Compare(other.From, other.To) > 0 ? other.From : other.To;

            return M.Compare(xfrom, yto) < 0 &&
                   M.Compare(yfrom, xto) < 0;
        }

        [Pure]
        public Seq<A> ToSeq() =>
            Prelude.Seq(AsEnumerable());

        [Pure]
        public IEnumerable<A> AsEnumerable()
        {
            if (StepIsAscending)
            {
                for (A x = From; M.Compare(x, To) <= 0; x = M.Plus(x, Step))
                {
                    yield return x;
                }
            }
            else
            {
                for (A x = From; M.Compare(x, To) >= 0; x = M.Plus(x, Step))
                {
                    yield return x;
                }
            }
        }

        [Pure]
        public IEnumerator<A> GetEnumerator() => 
            AsEnumerable().GetEnumerator();

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        [Pure]
        public S Fold<S>(S state, Func<S, A, S> f)
        {
            foreach(var x in AsEnumerable())
            {
                state = f(state, x);
            }
            return state;
        }
    }
}
