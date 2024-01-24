using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FLst<A, B> :
    Functor<Lst<A>, Lst<B>, A, B>
{
    [Pure]
    public static Lst<B> Map(Lst<A> ma, Func<A, B> f) =>
        ma.Map(f);
}
