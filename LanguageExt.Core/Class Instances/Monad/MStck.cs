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
/// MStack trait instance
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public struct MStck<A> :
    Monad<Stck<A>, A>,
    Eq<Stck<A>>,
    Monoid<Stck<A>>
{
    [Pure]
    public static Stck<A> Append(Stck<A> x, Stck<A> y) =>
        new (x.ConcatFast(y));

    [Pure]
    public static MB Bind<MONADB, MB, B>(Stck<A> ma, Func<A, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        traverse<MStck<A>, MONADB, Stck<A>, MB, A, B>(ma, f);

    [Pure]
    public static Func<Unit, int> Count(Stck<A> fa) => _ => 
        fa.Count();

    [Pure]
    public static Stck<A> Subtract(Stck<A> x, Stck<A> y) =>
        x - y;

    [Pure]
    public static Stck<A> Empty() =>
        Stck<A>.Empty;

    [Pure]
    public static bool Equals(Stck<A> x, Stck<A> y) =>
        x == y;

    [Pure]
    public static Stck<A> Fail(object? err = null) =>
        Stck<A>.Empty;

    [Pure]
    public static Func<Unit, S> Fold<S>(Stck<A> fa, S state, Func<S, A, S> f) => _ =>
        fa.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Stck<A> fa, S state, Func<S, A, S> f) => _ =>
        fa.FoldBack(state, f);

    [Pure]
    public static Stck<A> Plus(Stck<A> ma, Stck<A> mb) =>
        ma + mb;

    [Pure]
    public static Stck<A> Return(Func<Unit, A> f) =>
        Stack(f(unit));

    [Pure]
    public static Stck<A> Zero() =>
        Stck<A>.Empty;

    [Pure]
    public static int GetHashCode(Stck<A> x) =>
        x.GetHashCode();

    [Pure]
    public static Stck<A> Run(Func<Unit, Stck<A>> ma) =>
        ma(unit);

    [Pure]
    public static Stck<A> BindReturn(Unit _, Stck<A> mb) =>
        mb;

    [Pure]
    public static Stck<A> Return(A x) =>
        Stack(x);

    [Pure]
    public static Stck<A> Apply(Func<A, A, A> f, Stck<A> fa, Stck<A> fb) =>
        toStack(
            from a in fa
            from b in fb
            select f(a, b));
}
