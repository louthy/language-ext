using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

public struct MFin<A> :
    Monad<Fin<A>, A>,
    BiFoldable<Fin<A>, A, Error>,
    Ord<Fin<A>>
{
    [Pure]
    public static Fin<A> None => 
        default;
 
    [Pure]
    public static MB Bind<MonadB, MB, B>(Fin<A> ma, Func<A, MB> f) where MonadB : Monad<Unit, Unit, MB, B> =>
        ma.IsSucc
            ? (f ?? throw new ArgumentNullException(nameof(f)))(ma.Value)
            : MonadB.Fail(ma.Error);

    [Pure]
    public static Fin<A> Fail(object? err = null) =>
        Error.FromObject(err);

    [Pure]
    public static Fin<A> Plus(Fin<A> a, Fin<A> b) =>
        a.IsSucc ? a : b;

    [Pure]
    public static Fin<A> Return(Func<Unit, A> f) =>
        Fin<A>.Succ(f(unit));

    [Pure]
    public static Fin<A> Zero() =>
        default;

    [Pure]
    public static Func<Unit, S> Fold<S>(Fin<A> ma, S state, Func<S, A, S> f) => _ =>
        ma.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Fin<A> ma, S state, Func<S, A, S> f) => _ =>
        ma.Fold(state, f);

    [Pure]
    public static S BiFold<S>(Fin<A> ma, S state, Func<S, A, S> fa, Func<S, Error, S> fb) =>
        ma.BiFold(state, fa, fb);

    [Pure]
    public static S BiFoldBack<S>(Fin<A> ma, S state, Func<S, A, S> fa, Func<S, Error, S> fb) =>
        ma.BiFold(state, fa, fb);

    [Pure]
    public static Func<Unit, int> Count(Fin<A> ma) => _ =>
        ma.IsSucc 
            ? 1 
            : 0;

    [Pure]
    public static Fin<A> Run(Func<Unit, Fin<A>> ma) =>
        ma(unit);

    [Pure]
    public static Fin<A> BindReturn(Unit _, Fin<A> mb) =>
        mb;

    [Pure]
    public static Fin<A> Return(A x) =>
        x;

    [Pure]
    public static Fin<A> Empty() =>
        default;

    [Pure]
    public static Fin<A> Append(Fin<A> x, Fin<A> y) =>
        Plus(x, y);

    [Pure]
    public static bool Equals(Fin<A> x, Fin<A> y) =>
        x.Equals(y);

    [Pure]
    public static int GetHashCode(Fin<A> x) =>
        x.GetHashCode();

    [Pure]
    public static Fin<A> Apply(Func<A, A, A> f, Fin<A> fa, Fin<A> fb) =>
        fa.IsSucc
            ? fb.IsSucc
                  ? FinSucc(f(fa.Value, fb.Value))
                  : fb
            : fa;
        
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(Fin<A> x, Fin<A> y) =>
        x.CompareTo(y);
}
