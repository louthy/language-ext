using System;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FEnumerable<A, B> :
    Functor<IEnumerable<A>, IEnumerable<B>, A, B>
{
    [Pure]
    public static IEnumerable<B> Map(IEnumerable<A> ma, Func<A, B> f)
    {
        foreach (var a in ma)
            yield return f(a);
    }
}
