using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances;

public struct MReader<Env, A> : 
    MonadReader<Env, A>, 
    Monad<Env, Unit, Reader<Env, A>, A>
{
    [Pure]
    public static MB Bind<MONADB, MB, B>(Reader<Env, A> ma, Func<A, MB> f) where MONADB : Monad<Env, Unit, MB, B> =>
        MONADB.Run(env =>
        {
            var resA = ma(env);
            if (resA.IsFail) return MONADB.Fail(resA.Error);
            return f(resA.Value);
        });

    [Pure]
    public static Reader<Env, A> Fail(object? err = null) => _ =>
        Error.FromObject(err);

    [Pure]
    public static Reader<Env, A> Reader(Func<Env, A> f) => env =>
        f(env);

    [Pure]
    public static Reader<Env, Env> Ask() => env =>
        env;

    [Pure]
    public static Reader<Env, A> Local(Reader<Env, A> ma, Func<Env, Env> f) => env =>
        ma(f(env));

    [Pure]
    public static Reader<Env, A> Return(Func<Env, A> f) => env =>
        f(env);

    [Pure]
    public static Reader<Env, A> Run(Func<Env, Reader<Env, A>> f) => env =>
        f(env)(env);

    [Pure]
    public static Reader<Env, A> Plus(Reader<Env, A> ma, Reader<Env, A> mb) => env =>
    {
        var resA = ma(env);
        return resA.IsFail
                   ? mb(env)
                   : resA;
    };

    [Pure]
    public static Reader<Env, A> Zero() =>
        _ => Errors.Bottom;

    [Pure]
    public static Func<Env, S> Fold<S>(Reader<Env, A> fa, S state, Func<S, A, S> f) => env =>
    {
        var resA = fa(env);
        return resA.IsFail
                   ? state
                   : f(state, resA.Value);
    };

    [Pure]
    public static Func<Env, S> FoldBack<S>(Reader<Env, A> fa, S state, Func<S, A, S> f) =>
        Fold(fa, state, f);

    [Pure]
    public static Func<Env, int> Count(Reader<Env, A> fa) =>
        Fold(fa, 0, (_, _) => 1);

    [Pure]
    public static Reader<Env, A> BindReturn(Unit _, Reader<Env, A> mb) => mb;

    [Pure]
    public static Reader<Env, A> Apply(Func<A, A, A> f, Reader<Env, A> fa, Reader<Env, A> fb) =>
        from a in fa
        from b in fb
        select f(a, b);
}
