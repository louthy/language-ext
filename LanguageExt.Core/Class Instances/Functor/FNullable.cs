using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FNullable<A, B> : 
    Functor<A?, B?, A, B>,
    BiFunctor<A?, B?, A, Unit, B>
    where A : struct
    where B : struct
{
    [Pure]
    public static B? BiMap(A? ma, Func<A, B> fa, Func<Unit, B> fb) =>
        FOptional<MNullable<A>, MNullable<B>, A?, B?, A, B>.BiMap(ma, fa, fb);

    [Pure]
    public static B? Map(A? ma, Func<A, B> f) =>
        FOptional<MNullable<A>, MNullable<B>, A?, B?, A, B>.Map(ma, f);
}
