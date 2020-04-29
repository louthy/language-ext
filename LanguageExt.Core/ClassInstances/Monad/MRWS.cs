using System;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct MRWS<MonoidW, R, W, S, A> :
        MonadRWS<MonoidW, R, W, S, A>,
        Monad<(R Env, S State), RWSState<W, S>, RWS<MonoidW, R, W, S, A>, A>
        where MonoidW : struct, Monoid<W>
    {
        [Pure]
        public RWS<MonoidW, R, W, S, A> Apply(Func<A, A, A> f, RWS<MonoidW, R, W, S, A> fa, RWS<MonoidW, R, W, S, A> fb) =>
            from a in fa
            from b in fb
            select f(a,b);

        [Pure]
        public RWS<MonoidW, R, W, S, R> Ask() => (env, state) =>
            RWSResult<MonoidW, R, W, S, R>.New(default(MonoidW).Empty(), state, env);

        [Pure]
        public MB Bind<MONADB, MB, B>(RWS<MonoidW, R, W, S, A> ma, Func<A, MB> f) where MONADB : struct, Monad<(R Env, S State), RWSState<W, S>, MB, B> =>
            default(MONADB).Run(initial =>
            {
                var next = ma(initial.Env, initial.State);
                if (next.IsFaulted)
                {
                    return default(MONADB).Fail(next.Error);
                }
                else
                {
                    var b = f(next.Value);
                    return default(MONADB).BindReturn(next.ToRwsState(), b);
                }
            });

        [Pure]
        public MB BindAsync<MONADB, MB, B>(RWS<MonoidW, R, W, S, A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<(R Env, S State), RWSState<W, S>, MB, B> =>
            default(MONADB).RunAsync(initial =>
            {
                var next = ma(initial.Env, initial.State);
                if (next.IsFaulted)
                {
                    return default(MONADB).Fail(next.Error).AsTask();
                }
                else
                {
                    var b = f(next.Value);
                    return default(MONADB).BindReturn(next.ToRwsState(), b).AsTask();
                }
            });

        [Pure]
        public RWS<MonoidW, R, W, S, A> BindReturn(RWSState<W, S> previous, RWS<MonoidW, R, W, S, A> mb) => (env, state) =>
        {
            if (previous.IsFaulted)
            {
                return RWSResult<MonoidW, R, W, S, A>.New(previous.Output, previous.State, previous.Error);
            }
            else
            {
                var next = mb(env, previous.State);
                return next.IsFaulted
                    ? RWSResult<MonoidW, R, W, S, A>.New(default(MonoidW).Append(previous.Output, next.Output), state, next.Error)
                    : RWSResult<MonoidW, R, W, S, A>.New(default(MonoidW).Append(previous.Output, next.Output), next.State, next.Value);
            }
        };

        [Pure]
        public Func<(R Env, S State), int> Count(RWS<MonoidW, R, W, S, A> fa) =>
            Fold(fa, 0, (_, __) => 1);


        [Pure]
        public RWS<MonoidW, R, W, S, A> Fail(object err = null) =>
            (env, state) =>
                RWSResult<MonoidW, R, W, S, A>.New(default(MonoidW).Empty(), state, Error.FromObject(err));

        [Pure]
        public Func<(R Env, S State), S1> Fold<S1>(RWS<MonoidW, R, W, S, A> fa, S1 initialValue, Func<S1, A, S1> f) => input =>
        {
            var res = fa(input.Env, input.State);
            return res.IsFaulted
                ? initialValue
                : f(initialValue, res.Value);
        };

        [Pure]
        public Func<(R Env, S State), S1> FoldBack<S1>(RWS<MonoidW, R, W, S, A> fa, S1 state, Func<S1, A, S1> f) =>
            Fold(fa, state, f);

        [Pure]
        public RWS<MonoidW, R, W, S, A> Run(Func<(R Env, S State), RWS<MonoidW, R, W, S, A>> ma) =>
            (env, state) => ma((env, state))(env, state);

        [Pure]
        public RWS<MonoidW, R, W, S, A> Local(RWS<MonoidW, R, W, S, A> ma, Func<R, R> f) => (env, state) =>
            ma(f(env), state);

        [Pure]
        public RWS<MonoidW, R, W, S, A> Plus(RWS<MonoidW, R, W, S, A> ma, RWS<MonoidW, R, W, S, A> mb) => (env, state) =>
        {
            var res = ma(env, state);
            return res.IsFaulted
                ? mb(env, state)
                : res;
        };

        [Pure]
        public RWS<MonoidW, R, W, S, A> Return(Func<(R Env, S State), A> f) =>
            (env, state) =>
                RWSResult<MonoidW, R, W, S, A>.New(state, f((env, state)));

        [Pure]
        public RWS<MonoidW, R, W, S, A> Zero() => 
            (env, state) =>
                RWSResult<MonoidW, R, W, S, A>.New(state, Error.Bottom);

        [Pure]
        public RWS<MonoidW, R, W, S, S> Get() => 
            (env, state) =>
                RWSResult<MonoidW, R, W, S, S>.New(state, state);

        [Pure]
        public RWS<MonoidW, R, W, S, Unit> Put(S state) =>
            (env, _) =>
                RWSResult<MonoidW, R, W, S, Unit>.New(state, unit);

        [Pure]
        public RWS<MonoidW, R, W, S, Unit> Tell(W what) => 
            (env, state) =>
                RWSResult<MonoidW, R, W, S, Unit>.New(what, state, unit);

        [Pure]
        public RWS<MonoidW, R, W, S, (A, B)> Listen<B>(RWS<MonoidW, R, W, S, A> ma, Func<W, B> f) => (env, state) =>
        {
            var res = ma(env, state);
            return res.IsFaulted
                ? RWSResult<MonoidW, R, W, S, (A, B)>.New(res.State, res.Error)
                : RWSResult<MonoidW, R, W, S, (A, B)>.New(res.Output, res.State, (res.Value, f(res.Output)));
        };
    }
}
