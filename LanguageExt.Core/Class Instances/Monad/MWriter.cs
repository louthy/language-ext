using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct MWriter<MonoidW, W, A> :
    MonadWriter<MonoidW, W, A>,
    Monad<Unit, (W, bool), Writer<MonoidW, W, A>, A>
    where MonoidW : Monoid<W>
{
    [Pure]
    public static MB Bind<MONADB, MB, B>(Writer<MonoidW, W, A> ma, Func<A, MB> f) where MONADB : Monad<Unit, (W, bool), MB, B> =>
        MONADB.Run(_ =>
        {
            var (a, output1, faulted) = ma();
            return faulted
                       ? MONADB.Fail()
                       : MONADB.BindReturn((output1, faulted), f(a));
        });

    [Pure]
    public static Writer<MonoidW, W, A> BindReturn((W, bool) output, Writer<MonoidW, W, A> mb)
    {
        var (b, output2, faulted) = mb();
        return () => (faulted
                          ? (default(A), MonoidW.Empty(), true)
                          : (b, MonoidW.Append(output.Item1, output2), false))!;
    }

    [Pure]
    public static Writer<MonoidW, W, A> Fail(object? err = null) =>
        () => (default, MonoidW.Empty(), true)!;

    [Pure]
    public static Writer<MonoidW, W, A> Writer(A value, W output) =>
        () => (value, output, false);

    [Pure]
    public static Writer<MonoidW, W, A> Run(Func<Unit, Writer<MonoidW, W, A>> f) =>
        f(unit);

    [Pure]
    public static Writer<MonoidW, W, A> Return(Func<Unit, A> f) =>
        () => (f(unit), MonoidW.Empty(), false);

    /// <summary>
    /// Tells the monad what you want it to hear.  The monad carries this 'packet'
    /// upwards, merging it if needed (hence the Monoid requirement).
    /// </summary>
    /// <typeparam name="W">Type of the value tell</typeparam>
    /// <param name="what">The value to tell</param>
    /// <returns>Updated writer monad</returns>
    [Pure]
    public static Writer<MonoidW, W, Unit> Tell(W what) => () =>
        (unit, what, false);

    /// <summary>
    /// 'listen' is an action that executes the monad and adds
    /// its output to the value of the computation.
    /// </summary>
    [Pure]
    public static Writer<MonoidW, W, (A, B)> Listen<B>(Writer<MonoidW, W, A> ma, Func<W, B> f) => () =>
    {
        var (a, output, faulted) = ma();
        if (faulted) return (default, MonoidW.Empty(), true);
        return ((a, f(output)), output, false);
    };

    [Pure]
    public static Writer<MonoidW, W, A> Plus(Writer<MonoidW, W, A> ma, Writer<MonoidW, W, A> mb) => () =>
    {
        var (a, output, faulted) = ma();
        return faulted
                   ? mb()
                   : (a, output, faulted);
    };

    [Pure]
    public static Writer<MonoidW, W, A> Zero() =>
        () => (default, MonoidW.Empty(), true)!;

    [Pure]
    public static Func<Unit, S> Fold<S>(Writer<MonoidW, W, A> fa, S state, Func<S, A, S> f) => _ =>
    {
        var (a, _, faulted) = fa();
        return faulted
                   ? state
                   : f(state, a);
    };

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Writer<MonoidW, W, A> fa, S state, Func<S, A, S> f) =>
        Fold(fa, state, f);

    [Pure]
    public static Func<Unit, int> Count(Writer<MonoidW, W, A> fa) =>
        Fold(fa, 0, (_, _) => 1);

    [Pure]
    public static Writer<MonoidW, W, A> Apply(Func<A, A, A> f, Writer<MonoidW, W, A> fa, Writer<MonoidW, W, A> fb) =>
        from a in fa
        from b in fb
        select f(a, b);
}
