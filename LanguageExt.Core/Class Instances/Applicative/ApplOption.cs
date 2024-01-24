#nullable enable
using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct ApplOption<A, B> : 
    Functor<Option<A>, Option<B>, A, B>,
    BiFunctor<Option<A>, Option<B>, A, Unit, B>,
    Applicative<Option<Func<A, B>>, Option<A>, Option<B>, A, B>
{
    [Pure]
    public static Option<B> BiMap(Option<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
        FOption<A, B>.BiMap(ma, fa, fb);

    [Pure]
    public static Option<B> Map(Option<A> ma, Func<A, B> f) =>
        FOption<A, B>.Map(ma, f);

    [Pure]
    public static Option<B> Apply(Option<Func<A, B>> fab, Option<A> fa) =>
        ApplOptional<
                MOption<Func<A, B>>, MOption<A>, MOption<B>, 
                Option<Func<A, B>>, Option<A>, Option<B>, 
                A, B>.Apply(fab, fa);

    [Pure]
    public static Option<A> Pure(A x) =>
        ApplOptional<
                MOption<Func<A, B>>, MOption<A>, MOption<B>, 
                Option<Func<A, B>>, Option<A>, Option<B>, 
                A, B>.Pure(x);

    [Pure]
    public static  Option<B> Action(Option<A> fa, Option<B> fb) =>
        ApplOptional<
                MOption<Func<A, B>>, MOption<A>, MOption<B>, 
                Option<Func<A, B>>, Option<A>, Option<B>, 
                A, B>.Action(fa, fb);
}
