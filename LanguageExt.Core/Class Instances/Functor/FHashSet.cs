using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FHashSet<A, B> :
    Functor<HashSet<A>, HashSet<B>, A, B>
{
    [Pure]
    public static HashSet<B> Map(HashSet<A> ma, Func<A, B> f) =>
        ma.Map(f);
}
