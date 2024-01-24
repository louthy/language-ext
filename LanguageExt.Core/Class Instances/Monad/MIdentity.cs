using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Identity
/// </summary>
public struct MIdentity<A> : 
    Monad<Identity<A>, A>,
    AsyncPair<Identity<A>, Task<A>>
{
    [Pure]
    public static MB Bind<MONADB, MB, B>(Identity<A> ma, Func<A, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        f(ma.Value);

    [Pure]
    public static Identity<A> BindReturn(Unit maOutput, Identity<A> mb) =>
        mb;

    [Pure]
    public static Func<Unit, int> Count(Identity<A> fa) => _ =>
        1;

    [Pure]
    public static Identity<A> Fail(object? err = null) =>
        Identity<A>.Bottom;

    [Pure]
    public static Func<Unit, S> Fold<S>(Identity<A> fa, S state, Func<S, A, S> f) =>
        _ => f(state, fa.Value);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Identity<A> fa, S state, Func<S, A, S> f) =>
        _ => f(state, fa.Value);

    [Pure]
    public static Identity<A> Run(Func<Unit, Identity<A>> ma) =>
        ma(unit);

    [Pure]
    public static Identity<A> Plus(Identity<A> a, Identity<A> b) =>
        a.IsBottom
            ? b
            : a;

    [Pure]
    public static Identity<A> Return(Func<Unit, A> f) =>
        new (f(unit));

    [Pure]
    public static Identity<A> Zero() =>
        Identity<A>.Bottom;

    [Pure]
    public static Identity<A> Return(A x) =>
        Return(_ => x);

    [Pure]
    public static Identity<A> Apply(Func<A, A, A> f, Identity<A> fa, Identity<A> fb) =>
        Bind<MIdentity<A>, Identity<A>, A>(fa, a => Bind<MIdentity<A>, Identity<A>, A>(fb, b => Return(_ => f(a, b))));

    [Pure]
    public static Task<A> ToAsync(Identity<A> sa) =>
        sa.Value.AsTask();
}
