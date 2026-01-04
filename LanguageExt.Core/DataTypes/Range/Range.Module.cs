using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace LanguageExt;

public partial class Range
{
    /// <summary>
    /// Zero range  
    /// </summary>
    public static Range<A> zero<A>() 
        where A : IAdditiveIdentity<A, A> =>
        new(A.AdditiveIdentity, A.AdditiveIdentity, A.AdditiveIdentity, A.AdditiveIdentity, Iterable<A>.Empty, Iterable<A>.Empty);

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
        IComparisonOperators<A, A, bool>,
        IUnaryNegationOperators<A, A> 
    {
        return min > max
                   ? new(min, max, step, -step, Backward(), Forward())
                   : new(min, max, step, -step, Forward(), Backward());

        IEnumerable<A> Forward()
        {
            for (var x = min; x <= max; x += step)
            {
                yield return x;
            }
        }

        IEnumerable<A> Backward()
        {
            for (var x = min; x >= max; x += step)
            {
                yield return x;
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
        INumberBase<A> =>
        fromCount(min, count, A.One);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="count">The number of items in the range</param>
    /// <param name="step">The size of each step in the range</param>
    [Pure]
    public static Range<A> fromCount<A>(A min, A count, A step)
        where A : INumberBase<A>,
                  IComparisonOperators<A, A, bool>
    {
        var max = min + (count * step - step);
        return new(min, max, step, -step, Go(), GoBack());

        IEnumerable<A> Go()
        {
            var c = count;
            for (var x = min; c != A.Zero; x += step, c -= A.One)
            {
                yield return x;
                if (x == max) yield break;
            }
        }
        
        IEnumerable<A> GoBack()
        {
            var c = count;
            for (var x = max; c != A.Zero; x -= step, c -= A.One)
            {
                yield return x;
                if (x == min) yield break;
            }
        }
    }
 }
