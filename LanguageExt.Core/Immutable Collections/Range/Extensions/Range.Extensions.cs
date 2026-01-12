using System.Diagnostics.Contracts;
using System.Numerics;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RangeExtensions
{
    extension<A>(K<Range, A> ma)
    {
        /// <summary>
        /// Cast the structure to the actual type
        /// </summary>
        public Range<A> As() =>
            (Range<A>)ma;
    }

    extension<A>(K<Range, A> ma)
        where A : IComparisonOperators<A, A, bool>
    {
        /// <summary>
        /// Returns true if the value provided is in range
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <returns>True if the value provided is in range</returns>
        [Pure]
        public bool InRange(A value)
        {
            var range = +ma;
            var from  = range.From > range.To ? range.To : range.From;
            var to    = range.From > range.To ? range.From : range.To;
            return value >= from && value <= to;
        }

        /// <summary>
        /// Returns true if the range provided overlaps this range
        /// </summary>
        /// <param name="rhs">The range to test</param>
        /// <returns>True if the range provided overlaps this range</returns>
        [Pure]
        public bool Overlaps(K<Range, A> rhs)
        {
            var range = +ma;
            var other = +rhs;
            var xfrom = range.From > range.To ? range.To : range.From;
            var xto   = range.From > range.To ? range.From : range.To;
            var yfrom = other.From > other.To ? other.To : other.From;
            var yto   = other.From > other.To ? other.From : other.To;
            return xfrom < yto && yfrom < xto;
        }
    }
}
