using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Array trait instance
/// </summary>
/// <typeparam name="A"></typeparam>
public struct MArray<A> :
    Monad<A[], A>,
    Ord<A[]>,
    Monoid<A[]>
{
    [Pure]
    public static A[] Append(A[] x, A[] y) =>
        x.ConcatFast(y).ToArray();

    [Pure]
    public static MB Bind<MONADB, MB, B>(A[] ma, Func<A, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        ma.Fold(MONADB.Zero(), (s, a) => MONADB.Plus(s, f(a)));

    [Pure]
    public static Func<Unit, int> Count(A[] fa) =>
        _ => fa.Count();

    [Pure]
    public static A[] Subtract(A[] x, A[] y) =>
        Enumerable.Except(x, y).ToArray();

    [Pure]
    public static A[] Empty() =>
        System.Array.Empty<A>();

    [Pure]
    public static bool Equals(A[] x, A[] y) =>
        EqEnumerable<A>.Equals(x, y);

    [Pure]
    public static int Compare(A[] x, A[] y)
    {
        int cmp = x.Length.CompareTo(y.Length);
        if (cmp != 0) return cmp;

        using var iterA = x.AsEnumerable().GetEnumerator();
        using var iterB = y.AsEnumerable().GetEnumerator();
        while (iterA.MoveNext() && iterB.MoveNext())
        {
            cmp = OrdDefault<A>.Compare(iterA.Current, iterB.Current);
            if (cmp != 0) return cmp;
        }
        return 0;
    }

    [Pure]
    public static A[] Fail(object? err = null) =>
        Empty();

    [Pure]
    public static Func<Unit, S> Fold<S>(A[] fa, S state, Func<S, A, S> f) =>
        _ => fa.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(A[] fa, S state, Func<S, A, S> f) =>
        _ => fa.FoldBack(state, f);

    [Pure]
    public static A[] Plus(A[] ma, A[] mb) =>
        PlusSeq(ma, mb).ToArray();

    [Pure]
    static IEnumerable<A> PlusSeq(A[] ma, A[] mb)
    {
        foreach (var a in ma) yield return a;
        foreach (var b in mb) yield return b;
    }

    [Pure]
    public static A[] Return(Func<Unit, A> x) =>
        new[] { x(unit) };

    [Pure]
    public static A[] Return(A x) =>
        Return(_ => x);

    [Pure]
    public static A[] Zero() =>
        Empty();

    [Pure]
    public static int GetHashCode(A[] x) =>
        hash(x);

    [Pure]
    public static A[] Run(Func<Unit, A[]> ma) =>
        ma(unit);

    [Pure]
    public static A[] BindReturn(Unit _, A[] mb) =>
        mb;

    [Pure]
    public static A[] Apply(Func<A, A, A> f, A[] fa, A[] fb) =>
        (from a in fa
         from b in fb
         select f(a, b)).ToArray();

    [Pure]
    public static Task<bool> EqualsAsync(A[] x, A[] y) =>
        Equals(x, y).AsTask();

    [Pure]
    public static Task<int> GetHashCodeAsync(A[] x) =>
        GetHashCode(x).AsTask();    
        
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<int> CompareAsync(A[] x, A[] y) =>
        Compare(x, y).AsTask();
}
