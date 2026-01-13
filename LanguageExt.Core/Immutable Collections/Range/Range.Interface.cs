
using System.Collections;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

public interface Range<A> : IEnumerable<A>, K<Range, A> 
{
    /// <summary>
    /// Returns true if the value is within the range
    /// </summary>
    /// <remarks>
    /// This takes into account the step size of the range. 
    /// </remarks>
    /// <param name="value">Value</param>
    /// <returns>True if the value is an element within the range</returns>
    bool InRange(A value);

    /// <summary>
    /// Returns true if the range overlaps with another range
    /// </summary>
    /// <param name="ra">The other range to test</param>
    /// <returns>True if the range overlaps with another range</returns>
    bool Overlaps(Range<A> ra);

    /// <summary>
    /// Get the extents of the range
    /// </summary>
    /// <returns>Returns a minimum value guaranteed to be less than or equal to the maximum value</returns>
    (A Min, A Max) GetExtents();
    
    /// <summary>
    /// Get an iterator over the range
    /// </summary>
    /// <returns></returns>
    Iterator<A> ForwardIterator();

    IEnumerator IEnumerable.GetEnumerator() =>
        ForwardIterator().GetEnumerator().GetEnumerator();

    IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
        ForwardIterator().GetEnumerator().GetEnumerator();
}
