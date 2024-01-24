using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FTry<A, B> : 
    Functor<Try<A>, Try<B>, A, B>,
    BiFunctor<Try<A>, Try<B>, A, Unit, B>
{
    [Pure]
    public static Try<B> BiMap(Try<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
        FOptional<MTry<A>, MTry<B>, Try<A>, Try<B>, A, B>.BiMap(ma, fa, fb);

    [Pure]
    public static Try<B> Map(Try<A> ma, Func<A, B> f) =>
        FOptional<MTry<A>, MTry<B>, Try<A>, Try<B>, A, B>.Map(ma, f);
}
