using LanguageExt.Traits;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace LanguageExt;

/// <summary>
/// Represents a range of values
/// </summary>
/// <typeparam name="A">Bound values type</typeparam>
[Serializable]
public record Range<A>(A From, A To, A Step, IEnumerable<A> runRange) : 
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
        Prelude.toSeq(AsEnumerable());

    [Pure]
    public EnumerableM<A> AsEnumerable() =>
        new(runRange);

    [Pure]
    public IEnumerator<A> GetEnumerator() => 
        runRange.GetEnumerator();

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() =>
        runRange.GetEnumerator();
}
