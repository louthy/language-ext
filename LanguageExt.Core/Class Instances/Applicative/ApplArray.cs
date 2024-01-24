#nullable enable
using System;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct ApplArray<A, B> :
    Functor<A[], B[], A, B>,
    Applicative<Func<A, B>[], A[], B[], A, B>
{
    [Pure]
    public static B[] Action(A[] fa, B[] fb) =>
        (from a in fa
         from b in fb
         select b).ToArray();

    [Pure]
    public static B[] Apply(Func<A, B>[] fab, A[] fa) =>
        (from f in fab
         from a in fa
         select f(a)).ToArray();

    [Pure]
    public static B[] Map(A[] ma, Func<A, B> f) =>
        FArray<A, B>.Inst.Map(ma, f);

    [Pure]
    public static A[] Pure(A x) =>
        [x];
}

public struct ApplArray<A, B, C> :
    Applicative<Func<A, Func<B, C>>[], Func<B, C>[], A[], B[], C[], A, B>
{
    [Pure]
    public static Func<B, C>[] Apply(Func<A, Func<B, C>>[] fabc, A[] fa) =>
        (from f in fabc
         from a in fa
         select f(a)).ToArray();

    [Pure]
    public static C[] Apply(Func<A, Func<B, C>>[] fabc, A[] fa, B[] fb) =>
        (from f in fabc
         from a in fa
         from b in fb
         select f(a)(b)).ToArray();

    [Pure]
    public static A[] Pure(A x) =>
        [x];
}

public struct ApplArray<A> :
    Applicative<Func<A, A>[], A[], A[], A, A>,
    Applicative<Func<A, Func<A, A>>[], Func<A, A>[], A[], A[], A[], A, A>
{
    [Pure]
    public static A[] Action(A[] fa, A[] fb) =>
        (from a in fa
         from b in fb
         select b).ToArray();

    [Pure]
    public static A[] Apply(Func<A, A>[] fab, A[] fa) =>
        (from f in fab
         from a in fa
         select f(a)).ToArray();

    [Pure]
    public static Func<A, A>[] Apply(Func<A, Func<A, A>>[] fabc, A[] fa) =>
        (from f in fabc
         from a in fa
         select f(a)).ToArray();

    [Pure]
    public static A[] Apply(Func<A, Func<A, A>>[] fabc, A[] fa, A[] fb) =>
        (from f in fabc
         from a in fa
         from b in fb
         select f(a)(b)).ToArray();

    [Pure]
    public static A[] Pure(A x) =>
        [x];
}
