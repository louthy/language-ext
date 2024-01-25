using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct ApplArr<A, B> : 
    Functor<Arr<A>, Arr<B>, A, B>,
    Applicative<Arr<Func<A, B>>, Arr<A>, Arr<B>, A, B>
{
    [Pure]
    public static Arr<B> Action(Arr<A> fa, Arr<B> fb) =>
        (from a in fa
         from b in fb
         select b).ToArr();

    [Pure]
    public static  Arr<B> Apply(Arr<Func<A, B>> fab, Arr<A> fa) =>
        (from f in fab
         from a in fa
         select f(a)).ToArr();

    [Pure]
    public static  Arr<B> Map(Arr<A> ma, Func<A, B> f) =>
        FArr<A, B>.Map(ma, f);

    [Pure]
    public static  Arr<A> Pure(A x) =>
        Array(x);
}

public struct ApplArr<A, B, C> :
    Applicative<Arr<Func<A, Func<B, C>>>, Arr<Func<B, C>>, Arr<A>, Arr<B>, Arr<C>, A, B>
{
    [Pure]
    public static Arr<Func<B, C>> Apply(Arr<Func<A, Func<B, C>>> fabc, Arr<A> fa) =>
        (from f in fabc
         from a in fa
         select f(a)).ToArr();

    [Pure]
    public static Arr<C> Apply(Arr<Func<A, Func<B, C>>> fabc, Arr<A> fa, Arr<B> fb) =>
        (from f in fabc
         from a in fa
         from b in fb
         select f(a)(b)).ToArr();

    [Pure]
    public static Arr<A> Pure(A x) =>
        Array(x);
}

public struct ApplArr<A> :
    Applicative<Arr<Func<A, A>>, Arr<A>, Arr<A>, A, A>,
    Applicative<Arr<Func<A, Func<A, A>>>, Arr<Func<A, A>>, Arr<A>, Arr<A>, Arr<A>, A, A>
{
    [Pure]
    public static Arr<A> Action(Arr<A> fa, Arr<A> fb) =>
        (from a in fa
         from b in fb
         select b).ToArr();

    [Pure]
    public static Arr<A> Apply(Arr<Func<A, A>> fab, Arr<A> fa) =>
        (from f in fab
         from a in fa
         select f(a)).ToArr();

    [Pure]
    public static Arr<Func<A, A>> Apply(Arr<Func<A, Func<A, A>>> fabc, Arr<A> fa) =>
        (from f in fabc
         from a in fa
         select f(a)).ToArr();

    [Pure]
    public static Arr<A> Apply(Arr<Func<A, Func<A, A>>> fabc, Arr<A> fa, Arr<A> fb) =>
        (from f in fabc
         from a in fa
         from b in fb
         select f(a)(b)).ToArr();

    [Pure]
    public static Arr<A> Pure(A x) =>
        Array(x);
}
