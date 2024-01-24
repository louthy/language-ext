using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Array trait instance
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public struct MArr<A> :
    Monad<Arr<A>, A>,
    Ord<Arr<A>>,
    Monoid<Arr<A>>
{
    [Pure]
    public static Arr<A> Append(Arr<A> x, Arr<A> y) =>
        x.ConcatFast(y).ToArray();

    [Pure]
    public static MB Bind<MONADB, MB, B>(Arr<A> ma, Func<A, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        traverse<MArr<A>, MONADB, Arr<A>, MB, A, B>(ma, f);

    [Pure]
    public static Func<Unit, int> Count(Arr<A> fa) =>
        _ => fa.Count();

    [Pure]
    public static Arr<A> Empty() =>
        Arr<A>.Empty;

    [Pure]
    public static bool Equals(Arr<A> x, Arr<A> y) =>
        EqEnumerable<A>.Equals(x, y);

    [Pure]
    public static int Compare(Arr<A> x, Arr<A> y)
    {
        int cmp = x.Value.Length.CompareTo(y.Value.Length);
        if (cmp != 0) return cmp;

        using var iterA = x.Value.AsEnumerable().GetEnumerator();
        using var iterB = y.Value.AsEnumerable().GetEnumerator();
        while(iterA.MoveNext() && iterB.MoveNext())
        {
            cmp = OrdDefault<A>.Compare(iterA.Current, iterB.Current);
            if (cmp != 0) return cmp;
        }
        return 0;
    }

    [Pure]
    public static Arr<A> Fail(object? err = null) =>
        Empty();

    [Pure]
    public static Func<Unit, S> Fold<S>(Arr<A> fa, S state, Func<S, A, S> f) =>
        _ => fa.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Arr<A> fa, S state, Func<S, A, S> f) =>
        _ => fa.FoldBack(state, f);

    [Pure]
    public static Arr<A> Plus(Arr<A> ma, Arr<A> mb) =>
        ma + mb;

    [Pure]
    public static Arr<A> Return(Func<Unit, A> f) =>
        Arr.create(f(unit));

    [Pure]
    public static Arr<A> Return(A x) =>
        Return(_ => x);

    [Pure]
    public static Arr<A> Zero() =>
        Empty();

    [Pure]
    public static int GetHashCode(Arr<A> x) =>
        x.GetHashCode();

    [Pure]
    public static Arr<A> Run(Func<Unit, Arr<A>> f) =>
        f(unit);

    [Pure]
    public static Arr<A> BindReturn(Unit _, Arr<A> fmb) =>
        fmb;

    [Pure]
    public static Arr<A> Apply(Func<A, A, A> f, Arr<A> fa, Arr<A> fb) =>
        new Arr<A>(
            from a in fa
            from b in fb
            select f(a, b));
}
