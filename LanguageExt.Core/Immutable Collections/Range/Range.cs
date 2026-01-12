using System;
using System.Collections;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Represents a range of values.
/// </summary>
/// <remarks>
/// This type is iterable and foldable.  
/// </remarks>
/// <param name="From">Range start</param>
/// <param name="To">Range end</param>
/// <param name="Step">Step size</param>
/// <param name="Eq">Equality function</param>
/// <typeparam name="A"></typeparam>
public record Range<A>(A From, A To, Func<A, A> Step, Func<A, A, bool> Eq) :
    K<Range, A>, IEnumerable<A>
{
    public IEnumerator<A> GetEnumerator()
    {
        var fs = Range.IteratorState<A>.Setup(From, To, Step, Eq);
        while (fs.Step(out var value))
        {
            yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();
}
