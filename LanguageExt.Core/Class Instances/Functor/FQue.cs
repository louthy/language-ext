using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FQue<A, B> : 
    Functor<Que<A>, Que<B>, A, B>
{
    [Pure]
    public static Que<B> Map(Que<A> ma, Func<A, B> f) =>
        new (ma.Map(f));
}
