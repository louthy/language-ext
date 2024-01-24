using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

/// <summary>
/// MQue trait instance
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public struct MQue<A> :
    Monad<Que<A>, A>,
    Eq<Que<A>>,
    Monoid<Que<A>>
{
    [Pure]
    public static Que<A> Append(Que<A> x, Que<A> y) =>
        x + y;

    [Pure]
    public static MB Bind<MONADB, MB, B>(Que<A> ma, Func<A, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        ma.Fold(MONADB.Zero(), (s, a) => MONADB.Plus(s, f(a)));

    [Pure]
    public static Func<Unit, int> Count(Que<A> fa) => _ =>
        fa.Count();

    [Pure]
    public static Que<A> Subtract(Que<A> x, Que<A> y) =>
        x - y;

    [Pure]
    public static Que<A> Empty() =>
        Que<A>.Empty;

    [Pure]
    public static bool Equals(Que<A> x, Que<A> y) =>
        x == y;

    [Pure]
    public static Que<A> Fail(object? err = null) =>
        Que<A>.Empty;

    [Pure]
    public static Func<Unit, S> Fold<S>(Que<A> fa, S state, Func<S, A, S> f) => _ =>
        fa.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Que<A> fa, S state, Func<S, A, S> f) => _ =>
        fa.FoldBack(state, f);

    [Pure]
    public static Que<A> Plus(Que<A> ma, Que<A> mb) =>
        ma + mb;

    [Pure]
    public static Que<A> Return(Func<Unit, A> f) =>
        Queue(f(unit));

    [Pure]
    public static Que<A> Zero() =>
        Que<A>.Empty;

    [Pure]
    public static int GetHashCode(Que<A> x) =>
        x.GetHashCode();

    [Pure]
    public static Que<A> Run(Func<Unit, Que<A>> ma) =>
        ma(unit);

    [Pure]
    public static Que<A> BindReturn(Unit _, Que<A> mb) =>
        mb;

    [Pure]
    public static Que<A> Return(A x) =>
        Queue(x);

    [Pure]
    public static Que<A> Apply(Func<A, A, A> f, Que<A> fa, Que<A> fb) =>
        toQueue(
            from a in fa
            from b in fb
            select f(a, b));
        
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<bool> EqualsAsync(Que<A> x, Que<A> y) =>
        Equals(x, y).AsTask();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<int> GetHashCodeAsync(Que<A> x) =>
        GetHashCode(x).AsTask();       
}
