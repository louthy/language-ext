using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FStck<A, B> : 
    Functor<Stck<A>, Stck<B>, A, B>
{
    [Pure]
    public static Stck<B> Map(Stck<A> ma, Func<A, B> f) =>
        new (ma.Map(f));
}
