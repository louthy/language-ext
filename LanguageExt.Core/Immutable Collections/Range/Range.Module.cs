using System;
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
        new(A.AdditiveIdentity,
            A.AdditiveIdentity,
            static x => x,
            static (_, _) => true);
    
    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    [Pure]
    public static Range<A> fromMinMax<A>(A from, A to)
        where A :
        INumberBase<A>,
        IComparisonOperators<A, A, bool> =>
        fromMinMax(
            from, 
            to, 
            to >= from 
                ? static x => x + A.One 
                : static x => x - A.One, 
            static (x, y) => x == y);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    /// <param name="step">The size of each step in the range</param>
    [Pure]
    public static Range<A> fromMinMax<A>(A from, A to, A step)
        where A :
        IAdditionOperators<A, A, A>,
        IEqualityOperators<A, A, bool> =>
        fromMinMax(
            from, 
            to, 
            x => x + step, 
            static (x, y) => x == y);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    /// <param name="step">The action to perform each step</param>
    /// <param name="equality">An equality test for the `A` type</param>
    [Pure]
    public static Range<A> fromMinMax<A>(A from, A to, Func<A, A> step, Func<A, A, bool> equality) =>
        new (from, to, step, equality);

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
        count == A.Zero
            ? fromMinMax(min, min, A.One)
            : fromMinMax(min, min + count - A.One);

    /// <summary>
    /// Construct a new range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="count">The number of items in the range</param>
    /// <param name="step">The size of each step in the range</param>
    [Pure]
    public static Range<A> fromCount<A>(A min, A count, A step)
        where A : INumberBase<A> =>
        count == A.Zero
            ? fromMinMax(min, min, step)
            : fromMinMax(min, min + (count * step - step), step);
}
