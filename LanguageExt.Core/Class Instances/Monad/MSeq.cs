using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Seq trait instance
/// </summary>
/// <typeparam name="A"></typeparam>
public struct MSeq<A> :
    Monad<Seq<A>, A>,
    Eq<Seq<A>>,
    Monoid<Seq<A>>
{
    [Pure]
    public static Seq<A> Append(Seq<A> x, Seq<A> y) =>
        x.Concat(y);

    [Pure]
    public static MB Bind<MONADB, MB, B>(Seq<A> ma, Func<A, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        traverse<MSeq<A>, MONADB, Seq<A>, MB, A, B>(ma, f);

    [Pure]
    public static Func<Unit, int> Count(Seq<A> fa) => _ =>
        fa.Count;

    [Pure]
    public static Seq<A> Subtract(Seq<A> x, Seq<A> y) =>
        toSeq(Enumerable.Except(x, y));

    [Pure]
    public static Seq<A> Empty() =>
        Seq<A>.Empty;

    [Pure]
    public static bool Equals(Seq<A> x, Seq<A> y) =>
        EqEnumerable<A>.Equals(x, y);

    [Pure]
    public static Seq<A> Fail(object? err = null) =>
        Empty();

    [Pure]
    public static Func<Unit, S> Fold<S>(Seq<A> fa, S state, Func<S, A, S> f) => _ =>
        fa.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Seq<A> fa, S state, Func<S, A, S> f) => _ => 
        fa.FoldBack(state, f);

    [Pure]
    public static Seq<A> Plus(Seq<A> ma, Seq<A> mb) =>
        ma.Concat(mb);

    [Pure]
    public static Seq<A> Zero() =>
        Empty();

    [Pure]
    public static Seq<A> Return(Func<Unit, A> f) =>
        f(unit).Cons();

    [Pure]
    public static int GetHashCode(Seq<A> x) =>
        hash(x);

    [Pure]
    public static Seq<A> Run(Func<Unit, Seq<A>> ma) =>
        ma(unit);

    [Pure]
    public static Seq<A> BindReturn(Unit maOutput, Seq<A> mb) =>
        mb;

    [Pure]
    public static Seq<A> Return(A x) =>
        x.Cons();

    [Pure]
    public static Seq<A> Apply(Func<A, A, A> f, Seq<A> fa, Seq<A> fb) =>
        from a in fa
        from b in fb
        select f(a, b);
}
