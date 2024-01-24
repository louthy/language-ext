using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FTryOption<A, B> : 
    Functor<TryOption<A>, TryOption<B>, A, B>,
    BiFunctor<TryOption<A>, TryOption<B>, A, Unit, B>
{
    [Pure]
    public static TryOption<B> BiMap(TryOption<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
        FOptional<MTryOption<A>, MTryOption<B>, TryOption<A>, TryOption<B>, A, B>.BiMap(ma, fa, fb);

    [Pure]
    public static TryOption<B> Map(TryOption<A> ma, Func<A, B> f) =>
        FOptional<MTryOption<A>, MTryOption<B>, TryOption<A>, TryOption<B>, A, B>.Map(ma, f);
}
