using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances;

public struct FFin<A, B> : 
    Functor<Fin<A>, Fin<B>, A, B>,
    BiFunctor<Fin<A>, Fin<B>, A, Error, B>,
    BiFunctor<Fin<A>, Fin<B>, A, Error, B, Error>
{
    [Pure]
    public static Fin<B> BiMap(Fin<A> ma, Func<A, B> fa, Func<Error, B> fb) =>
        ma.BiMap(fa, fb);

    [Pure]
    public static Fin<B> BiMap(Fin<A> ma, Func<A, B> fa, Func<Error, Error> fb) =>
        ma.BiMap(fa, fb);

    [Pure]
    public static Fin<B> Map(Fin<A> ma, Func<A, B> f) =>
        ma.Map(f);
}
