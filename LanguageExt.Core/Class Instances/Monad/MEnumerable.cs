using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Enumerable trait instance
/// </summary>
/// <typeparam name="A"></typeparam>
public struct MEnumerable<A> :
    Monad<IEnumerable<A>, A>,
    Ord<IEnumerable<A>>,
    Monoid<IEnumerable<A>>
{
    [Pure]
    public static IEnumerable<A> Append(IEnumerable<A> x, IEnumerable<A> y) =>
        x.ConcatFast(y);

    [Pure]
    public static MB Bind<MONADB, MB, B>(IEnumerable<A> ma, Func<A, MB> f) where MONADB : Monad<Unit, Unit, MB, B>
    {
        if (typeof(Func<A, MB>) == typeof(Func<A, IEnumerable<B>>))
        {
            // The casts are not ideal, but it should still work reliably
            return (MB)BindLazy(ma, (Func<A, IEnumerable<B>>)(object)f);
        }
        else if (typeof(Func<A, MB>) == typeof(Func<A, Seq<B>>) && ma is Seq<A> seqA)
        {
            // The casts are not ideal, but it should still work reliably
            return (MB)(object)BindLazy(seqA, (Func<A, Seq<B>>)(object)f);
        }

        var b = MONADB.Zero();
        foreach (var a in ma)
        {
            b = MONADB.Plus(b, f(a));
        }
        return b;
    }

    static IEnumerable<B> BindLazy<B>(IEnumerable<A> ma, Func<A, IEnumerable<B>> f) =>
        ma.BindFast(f);

    static Seq<B> BindLazy<B>(Seq<A> ma, Func<A, Seq<B>> f) =>
        toSeq(ma.BindFast(a => f(a).AsEnumerable()));

    [Pure]
    public static Func<Unit, int> Count(IEnumerable<A> fa) => _ =>
        fa.Count();

    [Pure]
    public static IEnumerable<A> Subtract(IEnumerable<A> x, IEnumerable<A> y) =>
        x.Except(y);

    [Pure]
    public static IEnumerable<A> Empty() =>
        Enumerable.Empty<A>();

    [Pure]
    public static bool Equals(IEnumerable<A> x, IEnumerable<A> y) =>
        EqEnumerable<A>.Equals(x, y);

    [Pure]
    public static int Compare(IEnumerable<A> x, IEnumerable<A> y)
    {
        using var iterA = x.GetEnumerator();
        using var iterB = y.GetEnumerator();
        while (true)
        {
            var hasMovedA = iterA.MoveNext();
            var hasMovedB = iterB.MoveNext();

            if (hasMovedA && hasMovedB)
            {
                var cmp = OrdDefault<A>.Compare(iterA.Current, iterB.Current);
                if (cmp != 0) return cmp;
            }
            else if(hasMovedA)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }

    [Pure]
    public static IEnumerable<A> Fail(object? err = null) =>
        Empty();

    [Pure]
    public static Func<Unit, S> Fold<S>(IEnumerable<A> fa, S state, Func<S, A, S> f) => _ =>
        fa.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(IEnumerable<A> fa, S state, Func<S, A, S> f) => _ => 
        fa.FoldBack(state, f);

    [Pure]
    public static IEnumerable<A> Plus(IEnumerable<A> ma, IEnumerable<A> mb) =>
        ma.ConcatFast(mb);

    [Pure]
    public static IEnumerable<A> Zero() =>
        Empty();

    [Pure]
    public static IEnumerable<A> Return(Func<Unit, A> f) =>
        [f(unit)];

    [Pure]
    public static int GetHashCode(IEnumerable<A> x) =>
        hash(x);

    [Pure]
    public static IEnumerable<A> Run(Func<Unit, IEnumerable<A>> ma) =>
        ma(unit);

    [Pure]
    public static IEnumerable<A> BindReturn(Unit maOutput, IEnumerable<A> mb) =>
        mb;

    [Pure]
    public static IEnumerable<A> Return(A x) =>
        Return(_ => x);

    [Pure]
    public static MB Apply<MonadB, MB, B>(Func<A, A, B> faab, IEnumerable<A> fa, IEnumerable<A> fb) where MonadB : Monad<Unit, Unit, MB, B> =>
        Bind<MonadB, MB, B>(fa, a => Bind<MonadB, MB, B>(fb, b => MonadB.Return(_ => faab(a, b))));

    [Pure]
    public static IEnumerable<A> Apply(Func<A, A, A> f, IEnumerable<A> fa, IEnumerable<A> fb) =>
        from a in fa
        from b in fb
        select f(a, b);
}
