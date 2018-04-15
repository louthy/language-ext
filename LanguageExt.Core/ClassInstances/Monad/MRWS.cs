using System;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MRWS<MonoidW, R, W, S, A> :
        MonadRWS<MonoidW, R, W, S, A>,
        Monad<(R Env, S State), (W Output, S State, bool IsFaulted), RWS<MonoidW, R, W, S, A>, A>
        where MonoidW : struct, Monoid<W>
    {
        public RWS<MonoidW, R, W, S, A> Apply(Func<A, A, A> f, RWS<MonoidW, R, W, S, A> fa, RWS<MonoidW, R, W, S, A> fb) =>
            from a in fa
            from b in fb
            select f(a,b);

        public RWS<MonoidW, R, W, S, R> Ask() => (env, state) =>
            (env, default(MonoidW).Empty(), state, false);

        public MB Bind<MONADB, MB, B>(RWS<MonoidW, R, W, S, A> ma, Func<A, MB> f) where MONADB : struct, Monad<(R Env, S State), (W, S, bool), MB, B> =>
            default(MONADB).Run(initial =>
            {
                var next = ma(initial.Env, initial.State);
                return next.IsFaulted
                    ? default(MONADB).Fail(default(MRWS<MonoidW,R,W,S,A>).Fail())
                    : default(MONADB).BindReturn((next.Output,next.State,next.IsFaulted), f(next.Value));
            });

        public MB BindAsync<MONADB, MB, B>(RWS<MonoidW, R, W, S, A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<(R Env, S State), (W, S, bool), MB, B> =>
            default(MONADB).RunAsync(initial =>
            {
                var next = ma(initial.Env, initial.State);
                return next.IsFaulted
                    ? default(MONADB).Fail(default(MRWS<MonoidW, R, W, S, A>).Fail()).AsTask()
                    : default(MONADB).BindReturn((next.Output, next.State, next.IsFaulted), f(next.Value)).AsTask();
            });

        public RWS<MonoidW, R, W, S, A> BindReturn((W Output, S State, bool IsFaulted) previous, RWS<MonoidW, R, W, S, A> mb) => (env, state) =>
        {
            var next = mb(env, previous.State);
            return previous.IsFaulted
                ? (default(A), default(MonoidW).Empty(), state, true)
                : (next.Value, default(MonoidW).Append(previous.Output, next.Output), next.State, next.IsFaulted);
        };

        public Func<(R Env, S State), int> Count(RWS<MonoidW, R, W, S, A> fa) =>
            Fold(fa, 0, (_, __) => 1);


        public RWS<MonoidW, R, W, S, A> Fail(object err = null) =>
            (env, state) => (default(A), default(MonoidW).Empty(), state, true);

        public Func<(R Env, S State), S1> Fold<S1>(RWS<MonoidW, R, W, S, A> fa, S1 initialValue, Func<S1, A, S1> f) => input =>
        {
            var (a, output, nextState, isFaulted) = fa(input.Env, input.State);
            return isFaulted
                ? initialValue
                : f(initialValue, a);
        };

        public Func<(R Env, S State), S1> FoldBack<S1>(RWS<MonoidW, R, W, S, A> fa, S1 state, Func<S1, A, S1> f) =>
            Fold(fa, state, f);

        public RWS<MonoidW, R, W, S, A> Run(Func<(R Env, S State), RWS<MonoidW, R, W, S, A>> ma) =>
            (env, state) => ma((env, state))(env, state);

        public RWS<MonoidW, R, W, S, A> Local(RWS<MonoidW, R, W, S, A> ma, Func<R, R> f) => (env, state) =>
            ma(f(env), state);

        public RWS<MonoidW, R, W, S, A> Plus(RWS<MonoidW, R, W, S, A> ma, RWS<MonoidW, R, W, S, A> mb) => (env, state) =>
        {
            var (a, output, nextState, isFaulted) = ma(env, state);
            return isFaulted
                ? mb(env, state)
                : (a, output, nextState, isFaulted);
        };

        public RWS<MonoidW, R, W, S, A> Return(Func<(R Env, S State), A> f) =>
            (env, state) => (f((env, state)), default(MonoidW).Empty(), state, false);

        public RWS<MonoidW, R, W, S, A> Zero() => (env, state) =>
            (default(A), default(W), state, false);

        public RWS<MonoidW, R, W, S, S> Get() => (env, state) =>
            (state, default(MonoidW).Empty(), state, false);

        public RWS<MonoidW, R, W, S, Unit> Put(S state) => (env, _) =>
            (unit, default(MonoidW).Empty(), state, false);

        public RWS<MonoidW, R, W, S, Unit> Tell(W what) => (env, state) =>
            (unit, what, state, false);

        public RWS<MonoidW, R, W, S, (A, B)> Listen<B>(RWS<MonoidW, R, W, S, A> ma, Func<W, B> f) => (env, state) =>
        {
            var (a, output, nextState, isFaulted) = ma(env, state);
            return isFaulted
                ? (default((A, B)), default(MonoidW).Empty(), nextState, true)
                : ((a, f(output)), output, nextState, false);
        };
    }
}