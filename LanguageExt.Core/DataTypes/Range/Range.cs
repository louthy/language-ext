using LanguageExt.Traits;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics.Contracts;

namespace LanguageExt;

/// <summary>
/// Represents a range of values
/// </summary>
/// <typeparam name="A">Bound values type</typeparam>
[Serializable]
public record Range<A>(A From, A To, A Step, A BackStep, IEnumerable<A> runRange, IEnumerable<A> runRangeBack) : 
    IEnumerable<A>, 
    K<Range, A>
{
    /// <summary>
    /// Reference version for use in pattern-matching
    /// </summary>
    [Pure]
    public object? Case =>
        Prelude.Seq(this).Case;

    [Pure]
    public Seq<A> ToSeq() =>
        Prelude.toSeq(AsIterable());

    [Pure]
    public Iterable<A> AsIterable() =>
        runRange.AsIterable();

    [Pure]
    public IEnumerator<A> GetEnumerator() => 
        // ReSharper disable once NotDisposedResourceIsReturned
        runRange.GetEnumerator();

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() =>
        // ReSharper disable once NotDisposedResourceIsReturned
        runRange.GetEnumerator();

    [Pure]
    public Range<A> Reverse() =>
        new(To, From, BackStep, Step, runRangeBack, runRange);
}

record CharRange(char From, char To, IEnumerable<char> runRange, IEnumerable<char> runRangeBack) : 
    Range<char>(From, To, char.MinValue, char.MinValue, runRange, runRangeBack);
