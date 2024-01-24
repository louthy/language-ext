using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FOption<A, B> : 
    Functor<Option<A>, Option<B>, A, B>,
    BiFunctor<Option<A>, Option<B>, A, Unit, B>
{
    [Pure]
    public static Option<B> BiMap(Option<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
        ma is { IsSome: true, Value: not null }
            ? Some(fa(ma.Value))
            : Some(fb(unit));

    [Pure]
    public static Option<B> Map(Option<A> ma, Func<A, B> f) =>
        ma is { IsSome: true, Value: not null }
            ? Some(f(ma.Value))
            : None;
}
