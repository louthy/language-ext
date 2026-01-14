
using System;
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
    
    /// <summary>
    /// Return true if the range supports fast iteration.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// Fast Iteration Support
    /// </para>
    /// <para>
    /// Fast iteration is a low-level, high-performance iteration mechanism that bypasses the standard IEnumerator
    /// interface. It uses a mutable state structure (IteratorState) to enable efficient iteration without allocating
    /// enumerator objects or using virtual dispatch.
    /// </para>
    /// <para>
    /// This is useful for performance-critical code paths where allocation overhead and virtual method calls would
    /// be problematic. Typical use cases include tight loops, LINQ optimisations, and scenarios where maximum
    /// throughput is required.
    /// </para>
    /// <para>
    /// To use fast iteration:
    /// 1. Check SupportsFastIteration returns true
    /// 2. Call FastIterationSetup() to initialise the state
    /// 3. Repeatedly call FastIterationStep() until it returns false
    /// </para>
    /// <para>
    /// WARNING: This is an advanced feature. Incorrect usage can lead to undefined behaviour. Only use this if you
    /// understand the implications and have measured that standard iteration is a bottleneck.
    /// </para>
    /// </remarks>
    bool SupportsFastIteration =>
        false;

    /// <summary>
    /// Set up the fast iteration state.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// Fast Iteration Support
    /// </para>
    /// <para>
    /// Fast iteration is a low-level, high-performance iteration mechanism that bypasses the standard IEnumerator
    /// interface. It uses a mutable state structure (IteratorState) to enable efficient iteration without allocating
    /// enumerator objects or using virtual dispatch.
    /// </para>
    /// <para>
    /// This is useful for performance-critical code paths where allocation overhead and virtual method calls would
    /// be problematic. Typical use cases include tight loops, LINQ optimisations, and scenarios where maximum
    /// throughput is required.
    /// </para>
    /// <para>
    /// To use fast iteration:
    /// 1. Check SupportsFastIteration returns true
    /// 2. Call FastIterationSetup() to initialise the state
    /// 3. Repeatedly call FastIterationStep() until it returns false
    /// </para>
    /// <para>
    /// WARNING: This is an advanced feature. Incorrect usage can lead to undefined behaviour. Only use this if you
    /// understand the implications and have measured that standard iteration is a bottleneck.
    /// </para>
    /// </remarks>
    /// <param name="state">State to set</param>
    void FastIterationSetup(ref Range.IteratorState state) =>
        throw new NotSupportedException("Fast iteration is not supported for this range");
    
    /// <summary>
    /// Single step of the fast iteration state.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// Fast Iteration Support
    /// </para>
    /// <para>
    /// Fast iteration is a low-level, high-performance iteration mechanism that bypasses the standard IEnumerator
    /// interface. It uses a mutable state structure (IteratorState) to enable efficient iteration without allocating
    /// enumerator objects or using virtual dispatch.
    /// </para>
    /// <para>
    /// This is useful for performance-critical code paths where allocation overhead and virtual method calls would
    /// be problematic. Typical use cases include tight loops, LINQ optimisations, and scenarios where maximum
    /// throughput is required.
    /// </para>
    /// <para>
    /// To use fast iteration:
    /// 1. Check SupportsFastIteration returns true
    /// 2. Call FastIterationSetup() to initialise the state
    /// 3. Repeatedly call FastIterationStep() until it returns false
    /// </para>
    /// <para>
    /// WARNING: This is an advanced feature. Incorrect usage can lead to undefined behaviour. Only use this if you
    /// understand the implications and have measured that standard iteration is a bottleneck.
    /// </para>
    /// <para>
    /// For range implementors: you can throw a `NotSupportedException` here if `SupportsFastIteration` is false.
    /// </para>
    /// </remarks>
    /// <param name="state">State to set</param>
    /// <returns>True if the iteration is returning a `value` and should continue</returns>
    bool FastIterationStep(ref Range.IteratorState state, out A value) =>
        throw new NotSupportedException("Fast iteration is not supported for this range");
}
