using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances;

public struct MRWS<MonoidW, R, W, S, A> :
    MonadRWS<MonoidW, R, W, S, A>,
    Monad<(R Env, S State), Fin<(W Output, S State, A Value)>, RWS<MonoidW, R, W, S, A>, A>
    where MonoidW : Monoid<W>
{
    [Pure]
    public static RWS<MonoidW, R, W, S, A> Apply(Func<A, A, A> f, RWS<MonoidW, R, W, S, A> fa, RWS<MonoidW, R, W, S, A> fb) =>
        from a in fa
        from b in fb
        select f(a,b);

    [Pure]
    public static RWS<MonoidW, R, W, S, R> Ask() => (env, state) =>
        (MonoidW.Empty(), state, env);

    [Pure]
    public static MB Bind<MONADB, MB, B>(RWS<MonoidW, R, W, S, A> ma, Func<A, MB> f) 
        where MONADB : Monad<(R Env, S State), Fin<(W Output, S State, A Value)>, MB, B> =>
        MONADB.Run(initial =>
        {
            var next = ma(initial.Env, initial.State);
            if (next.IsFail)
            {
                return MONADB.Fail(next.Error);
            }
            else
            {
                var b = f(next.Value.Value);
                return MONADB.BindReturn(next.Value, b);
            }
        });

    [Pure]
    public static RWS<MonoidW, R, W, S, A> BindReturn(
        Fin<(W Output, S State, A Value)> previous, RWS<MonoidW, R, W, S, A> mb) =>
        (env, _) =>
        {
            if (previous.IsFail)
            {
                return previous.Error;
            }
            else
            {
                var next = mb(env, previous.Value.State);
                return next.IsFail
                           ? next.Error
                           : (MonoidW.Append(previous.Value.Output, next.Value.Output), next.Value.State, next.Value.Value);
            }
        };

    [Pure]
    public static Func<(R Env, S State), int> Count(RWS<MonoidW, R, W, S, A> fa) =>
        Fold(fa, 0, (_, _) => 1);

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Fail(object? err = null) =>
        (_, _) => Error.FromObject(err);

    [Pure]
    public static Func<(R Env, S State), S1> Fold<S1>(RWS<MonoidW, R, W, S, A> fa, S1 initialValue, Func<S1, A, S1> f) => input =>
    {
        var res = fa(input.Env, input.State);
        return res.IsFail
                   ? initialValue
                   : f(initialValue, res.Value.Value);
    };

    [Pure]
    public static Func<(R Env, S State), S1> FoldBack<S1>(RWS<MonoidW, R, W, S, A> fa, S1 state, Func<S1, A, S1> f) =>
        Fold(fa, state, f);

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Run(Func<(R Env, S State), RWS<MonoidW, R, W, S, A>> ma) =>
        (env, state) => ma((env, state))(env, state);

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Local(RWS<MonoidW, R, W, S, A> ma, Func<R, R> f) => (env, state) =>
        ma(f(env), state);

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Plus(RWS<MonoidW, R, W, S, A> ma, RWS<MonoidW, R, W, S, A> mb) => (env, state) =>
    {
        var res = ma(env, state);
        return res.IsFail
                   ? mb(env, state)
                   : res;
    };

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Return(Func<(R Env, S State), A> f) =>
        (env, state) =>
            (MonoidW.Empty(), state, f((env, state)));

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Zero() => 
        (_, _) => Errors.Bottom;

    [Pure]
    public static RWS<MonoidW, R, W, S, S> Get() => 
        (_, state) => (MonoidW.Empty(), state, state);

    [Pure]
    public static RWS<MonoidW, R, W, S, Unit> Put(S state) =>
        (_, _) =>(MonoidW.Empty(), state, unit);

    [Pure]
    public static RWS<MonoidW, R, W, S, Unit> Tell(W what) =>
        (_, state) => (what, state, unit);

    [Pure]
    public static RWS<MonoidW, R, W, S, (A, B)> Listen<B>(RWS<MonoidW, R, W, S, A> ma, Func<W, B> f) => (env, state) =>
    {
        var res = ma(env, state);
        return res.IsFail
                   ? res.Error
                   : (res.Value.Output, res.Value.State, (res.Value.Value, f(res.Value.Output)));
    };
}
