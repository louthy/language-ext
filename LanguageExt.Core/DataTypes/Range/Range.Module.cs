using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;

namespace LanguageExt;

public partial class Range
{
    /// <summary>
    /// Zero range  
    /// </summary>
    public static Range<A> zero<A>() 
        where A : IAdditiveIdentity<A, A> =>
        new(A.AdditiveIdentity, A.AdditiveIdentity, A.AdditiveIdentity, EnumerableM<A>.Empty);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    [Pure]
    public static Range<A> fromMinMax<A>(A min, A max)
        where A : 
            INumberBase<A>,
            ISubtractionOperators<A, A, A>,
            IComparisonOperators<A, A, bool>,     
            IAdditionOperators<A, A, A>, 
            IEqualityOperators<A, A, bool> =>
        fromMinMax(min, max, max >= min ? A.One : A.Zero - A.One);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    /// <param name="step">The size of each step in the range</param>
    [Pure]
    public static Range<A> fromMinMax<A>(A min, A max, A step)
        where A : 
        IAdditionOperators<A, A, A>, 
        IComparisonOperators<A, A, bool>
    {
        return min > max
                   ? new(min, max, step, Enumerable.Empty<A>())
                   : new(min, max, step, Go());

        IEnumerable<A> Go()
        {
            for (var x = min;; x += step)
            {
                yield return x;
                if (x == max) yield break;
            }
        }
    }

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="count">The number of items in the range</param>
    [Pure]
    public static Range<A> fromCount<A>(A min, A count)
        where A : 
        IComparisonOperators<A, A, bool>,
        INumberBase<A>,
        IAdditionOperators<A, A, A>,
        ISubtractionOperators<A, A, A>,
        IMultiplyOperators<A, A, A> =>
        fromCount(min, count, A.One);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="count">The number of items in the range</param>
    /// <param name="step">The size of each step in the range</param>
    [Pure]
    public static Range<A> fromCount<A>(A min, A count, A step)
        where A : IComparisonOperators<A, A, bool>,
                  IAdditionOperators<A, A, A>,
                  ISubtractionOperators<A, A, A>,
                  IMultiplyOperators<A, A, A>
    {
        var max = min + (count * step - step);
        return min > max
               ? new(min, max, step, Enumerable.Empty<A>())
               : new(min, max, step, Go());

        IEnumerable<A> Go()
        {
            for (var x = min;; x += step)
            {
                yield return x;
                if (x == max) yield break;
            }
        }
    }
}
