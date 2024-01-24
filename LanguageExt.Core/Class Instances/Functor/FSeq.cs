using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FSeq<A, B> :
    Functor<Seq<A>, Seq<B>, A, B>
{
    [Pure]
    public static Seq<B> Map(Seq<A> ma, Func<A, B> f) =>
        ma.Map(f);
}
