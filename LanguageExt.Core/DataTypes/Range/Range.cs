using LanguageExt.Traits;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics.Contracts;

namespace LanguageExt;

/// <summary>
/// Represents a range of values
/// </summary>
/// <typeparam name="SELF">The type that is deriving from this type</typeparam>
/// <typeparam name="NumOrdA">Monoid of A class instance</typeparam>
/// <typeparam name="A">Bound values type</typeparam>
[Serializable]
public class Range<SELF, NumOrdA, A> : IEnumerable<A>
    where SELF : Range<SELF, NumOrdA, A>
    where NumOrdA : Ord<A>, Num<A>
{
    static Func<A, A, A, SELF> Ctor = IL.Ctor<A, A, A, SELF>();

    /// <summary>
    /// Zero range using MonoidOrdA.Empty() 
    /// </summary>
    public static readonly SELF Zero = 
        FromMinMax(NumOrdA.FromInteger(0), NumOrdA.FromInteger(0), NumOrdA.FromInteger(0));

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
    public object? Case =>
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
        Ctor(min, NumOrdA.Plus(min, NumOrdA.Subtract(NumOrdA.Product(count, step), step)), step);

    /// <summary>
    /// Construct a range
    /// </summary>
    /// <param name="from">The minimum value in the range</param>
    /// <param name="to">The maximum value in the range</param>
    /// <param name="step">The size of each step in the range</param>
    protected Range(A from, A to, A step)
    {
        From = from;
        To = to;
        Step = step;

        StepIsAscending = NumOrdA.Compare(step, NumOrdA.FromInteger(0)) >= 0;
    }
    
    // 2 * 2 >= 2  --- True
    // -2 * -2 >= 2  --- True

    /// <summary>
    /// Returns true if the value provided is in range
    /// </summary>
    /// <param name="value">Value to test</param>
    /// <returns>True if the value provided is in range</returns>
    [Pure]
    public bool InRange(A value)
    {
        var from = NumOrdA.Compare(From, To) > 0 ? To : From;
        var to   = NumOrdA.Compare(From, To)          > 0 ? From : To;

        return NumOrdA.Compare(value, from) >= 0 &&
               NumOrdA.Compare(value, to)   <= 0;
    }

    /// <summary>
    /// Returns true if the range provided overlaps this range
    /// </summary>
    /// <param name="other">The range to test</param>
    /// <returns>True if the range provided overlaps this range</returns>
    [Pure]
    public bool Overlaps(SELF other)
    {
        var xfrom = NumOrdA.Compare(From, To)             > 0 ? To : From;
        var xto   = NumOrdA.Compare(From, To)             > 0 ? From : To;
        var yfrom = NumOrdA.Compare(other.From, other.To) > 0 ? other.To : other.From;
        var yto   = NumOrdA.Compare(other.From, other.To) > 0 ? other.From : other.To;

        return NumOrdA.Compare(xfrom, yto) < 0 &&
               NumOrdA.Compare(yfrom, xto) < 0;
    }

    [Pure]
    public Seq<A> ToSeq() =>
        Prelude.toSeq(AsEnumerable());

    [Pure]
    public IEnumerable<A> AsEnumerable()
    {
        if (StepIsAscending)
        {
            for (A x = From; NumOrdA.Compare(x, To) <= 0; x = NumOrdA.Plus(x, Step))
            {
                yield return x;
            }
        }
        else
        {
            for (A x = From; NumOrdA.Compare(x, To) >= 0; x = NumOrdA.Plus(x, Step))
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
