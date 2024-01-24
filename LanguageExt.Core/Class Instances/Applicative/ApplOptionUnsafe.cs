#nullable enable
using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct ApplOptionUnsafe<A, B> : 
    Functor<OptionUnsafe<A>, OptionUnsafe<B>, A, B>,
    BiFunctor<OptionUnsafe<A>, OptionUnsafe<B>, A, Unit, B>,
    Applicative<OptionUnsafe<Func<A, B>>, OptionUnsafe<A>, OptionUnsafe<B>, A, B>
{
    [Pure]
    public static OptionUnsafe<B> BiMap(OptionUnsafe<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
        FOptionUnsafe<A, B>.Inst.BiMap(ma, fa, fb);

    [Pure]
    public static OptionUnsafe<B> Map(OptionUnsafe<A> ma, Func<A, B> f) =>
        FOptionUnsafe<A, B>.Inst.Map(ma, f);

    [Pure]
    public static OptionUnsafe<B> Apply(OptionUnsafe<Func<A, B>> fab, OptionUnsafe<A> fa) =>
        ApplOptionalUnsafe<
                MOptionUnsafe<Func<A, B>>, MOptionUnsafe<A>, MOptionUnsafe<B>, 
                OptionUnsafe<Func<A, B>>, OptionUnsafe<A>, OptionUnsafe<B>, 
                A, B>.Apply(fab, fa);

    [Pure]
    public static OptionUnsafe<A> Pure(A x) =>
        ApplOptionalUnsafe<
                MOptionUnsafe<Func<A, B>>, MOptionUnsafe<A>, MOptionUnsafe<B>, 
                OptionUnsafe<Func<A, B>>, OptionUnsafe<A>, OptionUnsafe<B>, 
                A, B>.Pure(x);

    [Pure]
    public static OptionUnsafe<B> Action(OptionUnsafe<A> fa, OptionUnsafe<B> fb) =>
        ApplOptionalUnsafe<
                MOptionUnsafe<Func<A, B>>, MOptionUnsafe<A>, MOptionUnsafe<B>, 
                OptionUnsafe<Func<A, B>>, OptionUnsafe<A>, OptionUnsafe<B>, 
                A, B>.Action(fa, fb);
}

public struct ApplOptionUnsafe<A, B, C> :
    Applicative<OptionUnsafe<Func<A, Func<B, C>>>, OptionUnsafe<Func<B, C>>, OptionUnsafe<A>, OptionUnsafe<B>, OptionUnsafe<C>, A, B>
{
    [Pure]
    public static OptionUnsafe<Func<B, C>> Apply(OptionUnsafe<Func<A, Func<B, C>>> fab, OptionUnsafe<A> fa) =>
        ApplOptionalUnsafe<
                MOptionUnsafe<Func<A, Func<B, C>>>, MOptionUnsafe<Func<B, C>>, MOptionUnsafe<A>, MOptionUnsafe<B>, MOptionUnsafe<C>,
                OptionUnsafe<Func<A, Func<B, C>>>, OptionUnsafe<Func<B, C>>, OptionUnsafe<A>, OptionUnsafe<B>, OptionUnsafe<C>,
                A, B, C>.Apply(fab, fa);

    [Pure]
    public static OptionUnsafe<C> Apply(OptionUnsafe<Func<A, Func<B, C>>> fab, OptionUnsafe<A> fa, OptionUnsafe<B> fb) =>
        ApplOptionalUnsafe<
                MOptionUnsafe<Func<A, Func<B, C>>>, MOptionUnsafe<Func<B, C>>, MOptionUnsafe<A>, MOptionUnsafe<B>, MOptionUnsafe<C>,
                OptionUnsafe<Func<A, Func<B, C>>>, OptionUnsafe<Func<B, C>>, OptionUnsafe<A>, OptionUnsafe<B>, OptionUnsafe<C>,
                A, B, C>.Apply(fab, fa, fb);

    [Pure]
    public static OptionUnsafe<A> Pure(A x) =>
        ApplOptionalUnsafe<
                MOptionUnsafe<Func<A, Func<B, C>>>, MOptionUnsafe<Func<B, C>>, MOptionUnsafe<A>, MOptionUnsafe<B>, MOptionUnsafe<C>,
                OptionUnsafe<Func<A, Func<B, C>>>, OptionUnsafe<Func<B, C>>, OptionUnsafe<A>, OptionUnsafe<B>, OptionUnsafe<C>,
                A, B, C>.Pure(x);
}
