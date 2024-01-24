using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

/// <summary>
/// MLst trait instance
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public struct MLst<A> :
    Monad<Lst<A>, A>,
    Ord<Lst<A>>,
    Monoid<Lst<A>>
{
    [Pure]
    public static Lst<A> Append(Lst<A> x, Lst<A> y) =>
        x.ConcatFast(y).Freeze();

    [Pure]
    public static MB Bind<MONADB, MB, B>(Lst<A> ma, Func<A, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        traverse<MLst<A>, MONADB, Lst<A>, MB, A, B>(ma, f);
    
    [Pure]
    public static Func<Unit, int> Count(Lst<A> fa) => _ =>
        fa.Count();

    [Pure]
    public static Lst<A> Subtract(Lst<A> x, Lst<A> y) =>
        Enumerable.Except(x, y).Freeze();

    [Pure]
    public static Lst<A> Empty() =>
        List.empty<A>();

    [Pure]
    public static bool Equals(Lst<A> x, Lst<A> y) =>
        EqEnumerable<A>.Equals(x, y);

    [Pure]
    public static int Compare(Lst<A> x, Lst<A> y)
    {
        int cmp = x.Count.CompareTo(y.Count);
        if (cmp != 0) return cmp;

        using var iterA = x.GetEnumerator();
        using var iterB = y.GetEnumerator();
        while (iterA.MoveNext() && iterB.MoveNext())
        {
            cmp = OrdDefault<A>.Compare(iterA.Current, iterB.Current);
            if (cmp != 0) return cmp;
        }
        return 0;
    }

    [Pure]
    public static Lst<A> Fail(object? err = null) =>
        Empty();

    [Pure]
    public static Func<Unit, S> Fold<S>(Lst<A> fa, S state, Func<S, A, S> f) => _ =>
        fa.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Lst<A> fa, S state, Func<S, A, S> f) => _ =>
        fa.FoldBack(state, f);

    [Pure]
    public static Lst<A> Plus(Lst<A> ma, Lst<A> mb) =>
        ma + mb;

    [Pure]
    public static Lst<A> Return(Func<Unit, A> f) =>
        List(f(unit));

    [Pure]
    public static Lst<A> Zero() =>
        Empty();

    [Pure]
    public static int GetHashCode(Lst<A> x) =>
        x.GetHashCode();

    [Pure]
    public static Lst<A> Run(Func<Unit, Lst<A>> ma) =>
        ma(unit);

    [Pure]
    public static Lst<A> BindReturn(Unit maOutput, Lst<A> mb) =>
        mb;

    [Pure]
    public static Lst<A> Return(A x) =>
        Return(_ => x);

    [Pure]
    public static Lst<A> Apply(Func<A, A, A> f, Lst<A> fa, Lst<A> fb) =>
        from a in fa
        from b in fb
        select f(a, b);
        
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<bool> EqualsAsync(Lst<A> x, Lst<A> y) =>
        Equals(x, y).AsTask();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<int> GetHashCodeAsync(Lst<A> x) =>
        GetHashCode(x).AsTask();         
        
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<int> CompareAsync(Lst<A> x, Lst<A> y) =>
        Compare(x, y).AsTask();
}
