#nullable enable
using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances;

public struct ApplFin<A, B> : 
    Functor<Fin<A>, Fin<B>, A, B>,
    BiFunctor<Fin<A>, Fin<B>, A, Error, B>,
    Applicative<Fin<Func<A, B>>, Fin<A>, Fin<B>, A, B>
{
    [Pure]
    public static Fin<B> BiMap(Fin<A> ma, Func<A, B> fa, Func<Error, B> fb) =>
        FFin<A, B>.Inst.BiMap(ma, fa, fb);

    [Pure]
    public static Fin<B> Map(Fin<A> ma, Func<A, B> f) =>
        FFin<A, B>.Inst.Map(ma, f);

    [Pure]
    public static Fin<B> Apply(Fin<Func<A, B>> fab, Fin<A> fa) =>
        from f in fab
        from a in fa
        select f(a);

    [Pure]
    public static Fin<A> Pure(A x) =>
        MFin<A>.Inst.Return(x);

    [Pure]
    public static Fin<B> Action(Fin<A> fa, Fin<B> fb) =>
        from a in fa
        from b in fb
        select b;
}

public struct ApplFin<A, B, C> :
    Applicative<Fin<Func<A, Func<B, C>>>, Fin<Func<B, C>>, Fin<A>, Fin<B>, Fin<C>, A, B>
{
    [Pure]
    public static Fin<Func<B, C>> Apply(Fin<Func<A, Func<B, C>>> fabc, Fin<A> fa) =>
        from f in fabc
        from a in fa
        select f(a);

    [Pure]
    public static Fin<C> Apply(Fin<Func<A, Func<B, C>>> fabc, Fin<A> fa, Fin<B> fb) =>
        from f in fabc
        from a in fa
        from b in fb
        select f(a)(b);

    [Pure]
    public static Fin<A> Pure(A x) =>
        MFin<A>.Inst.Return(x);
}

public struct ApplFin<A> : 
    Functor<Fin<A>, Fin<A>, A, A>,
    BiFunctor<Fin<A>, Fin<A>, A, Error, A>,
    Applicative<Fin<Func<A, A>>, Fin<A>, Fin<A>, A, A>,
    Applicative<Fin<Func<A, Func<A, A>>>, Fin<Func<A, A>>, Fin<A>, Fin<A>, Fin<A>, A, A>
{
    [Pure]
    public static Fin<A> BiMap(Fin<A> ma, Func<A, A> fa, Func<Error, A> fb) =>
        ma.BiMap(fa, fb);

    [Pure]
    public static Fin<A> Map(Fin<A> ma, Func<A, A> f) =>
        ma.Map(f);

    [Pure]
    public static Fin<A> Apply(Fin<Func<A, A>> fab, Fin<A> fa) =>
        from f in fab
        from a in fa
        select f(a);

    [Pure]
    public static Fin<A> Pure(A x) =>
        MFin<A>.Inst.Return(x);

    [Pure]
    public static Fin<A> Action(Fin<A> fa, Fin<A> fb) =>
        from a in fa
        from b in fb
        select b;

    [Pure]
    public static Fin<Func<A, A>> Apply(Fin<Func<A, Func<A, A>>> fabc, Fin<A> fa) =>
        from f in fabc
        from a in fa
        select f(a);

    [Pure]
    public static Fin<A> Apply(Fin<Func<A, Func<A, A>>> fabc, Fin<A> fa, Fin<A> fb) =>
        from f in fabc
        from a in fa
        from b in fb
        select f(a)(b);
}
