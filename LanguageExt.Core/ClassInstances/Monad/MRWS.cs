using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MRWS<MonoidW, R, W, S, A> :

        MonadRWS<SRWS<MonoidW, R, W, S, A>, RWS<MonoidW, R, W, S, A>, MonoidW, R, W, S, A>,
        Monad<(R, W, S), RWS<MonoidW, R, W, S, A>, A>
        where MonoidW : struct, Monoid<W>

    {
        [Pure]
        public SEnv Ask<SReaderEnv, SEnv>() where SReaderEnv : struct, ReaderMonadValue<SEnv, R, R> =>
            default(SReaderEnv).Lift(env =>
                (env, env, false));

        [Pure]
        public MB Bind<MONADB, MB, B>(RWS<MonoidW, R, W, S, A> ma, Func<A, MB> f) where MONADB : struct, Monad<(R, W, S), MB, B> =>
            default(MONADB).Return(env =>
            {
                var (x, (r, w, s), bottom) = default(SRWS<MonoidW, R, W, S, A>).Eval(ma, env);
                if (bottom) return (default(B), env, true);
                return default(MONADB).Eval(f(x), (env.Item1, default(MonoidW).Append(env.Item2, w), s));
            });

        [Pure]
        public (A, (R, W, S), bool) Eval(RWS<MonoidW, R, W, S, A> ma, (R, W, S) state) =>
            default(SRWS<MonoidW, R, W, S, A>).Eval(ma, state);

        [Pure]
        public RWS<MonoidW, R, W, S, A> Fail(Exception err = null) =>
            default(SRWS<MonoidW, R, W, S, A>).Lift(_ => (default(A), (default(R), default(MonoidW).Empty(), default(S)), true));

        [Pure]
        public RWS<MonoidW, R, W, S, A> Fail(object err) =>
            default(SRWS<MonoidW, R, W, S, A>).Lift(_ => (default(A), (default(R), default(MonoidW).Empty(), default(S)), true));

        [Pure]
        public SSS Get<SStateS, SSS>() where SStateS : struct, StateMonadValue<SSS, S, S> =>
            default(SStateS).Lift(state => (state, state, false));

        [Pure]
        public SSAW Listen<SWriterAW, SSAW>(RWS<MonoidW, R, W, S, A> ma) where SWriterAW : struct, WriterMonadValue<SSAW, W, (A, W)> =>
            default(SWriterAW).Lift(written =>
            {
                var (a, w, b) = default(SRWS<MonoidW, R, W, S, A>).Eval(ma, written);
                return b
                    ? (default((A, W)), default(MonoidW).Empty(), true)
                    : ((a, w), default(MonoidW).Append(written, w), false);
            });

        [Pure]
        public RWS<MonoidW, R, W, S, A> Local(Func<R, R> f, RWS<MonoidW, R, W, S, A> ma) =>
            default(SRWS<MonoidW, R, W, S, A>).Lift(env =>
            {
                var e = f(env.Item1);
                var (a, (r, w, s), b) = default(SRWS<MonoidW, R, W, S, A>).Eval(ma, (e, env.Item2, env.Item3));
                if (b) return (a, env, b);
                return (a, (r, default(MonoidW).Append(env.Item2, w), s), b);
            });

        [Pure]
        public SSU Put<SStateU, SSU>(S state) where SStateU : struct, StateMonadValue<SSU, S, Unit> =>
            default(SStateU).Lift(_ => (unit, state, false));

        [Pure]
        public RWS<MonoidW, R, W, S, A> Reader(Func<R, A> f) =>
            default(SRWS<MonoidW, R, W, S, A>).Lift(env => (f(env.Item1), env, false));

        [Pure]
        public RWS<MonoidW, R, W, S, A> Return(A x) =>
            default(SRWS<MonoidW, R, W, S, A>).Lift((x, (default(R), default(MonoidW).Empty(), default(S)), false));

        [Pure]
        public RWS<MonoidW, R, W, S, A> Return(Func<(R, W, S), (A, (R, W, S), bool)> f) =>
            default(SRWS<MonoidW, R, W, S, A>).Lift(f);

        [Pure]
        public RWS<MonoidW, R, W, S, A> State(Func<S, (A, S, bool)> f) =>
            default(SRWS<MonoidW, R, W, S, A>).Lift(rws =>
            {
                var (a, s, b) = f(rws.Item3);
                return (a, (rws.Item1, rws.Item2, s), b);
            });

        [Pure]
        public SSU Tell<SWriterU, SSU>(W what) where SWriterU : struct, WriterMonadValue<SSU, W, Unit> =>
            default(SWriterU).Lift(output =>
                (unit, default(MonoidW).Append(output, what), false));
    }



    public struct MRWS<SRwsA, RwsA, MonoidW, R, W, S, A> : 
        MonadRWS<SRwsA, RwsA, MonoidW, R, W, S, A>,
        Monad<(R, W, S), RwsA, A>
            where SRwsA : struct, 
                MonadValue<RwsA, (R, W, S), A>,
                ReaderMonadValue<RwsA, R, A>,
                WriterMonadValue<RwsA, W, A>,
                StateMonadValue<RwsA, S, A>
            where MonoidW : struct, Monoid<W>
    {
        [Pure]
        public SEnv Ask<SReaderEnv, SEnv>() where SReaderEnv : struct, ReaderMonadValue<SEnv, R, R> =>
            default(SReaderEnv).Lift(env =>
                (env, env, false));

        [Pure]
        public MB Bind<MONADB, MB, B>(RwsA ma, Func<A, MB> f) where MONADB : struct, Monad<(R, W, S), MB, B> =>
            default(MONADB).Return(env =>
            {
                var (x, (r,w,s), bottom) = default(SRwsA).Eval(ma, env);
                if (bottom) return (default(B), env, true);
                return default(MONADB).Eval(f(x), (env.Item1, default(MonoidW).Append(env.Item2, w), s));
            });

        [Pure]
        public (A, (R, W, S), bool) Eval(RwsA ma, (R, W, S) state) =>
            default(SRwsA).Eval(ma, state);

        [Pure]
        public RwsA Fail(Exception err = null) =>
            default(SRwsA).Lift(_ => (default(A), (default(R), default(MonoidW).Empty(), default(S)), true));

        [Pure]
        public RwsA Fail(object err) =>
            default(SRwsA).Lift(_ => (default(A), (default(R), default(MonoidW).Empty(), default(S)), true));

        [Pure]
        public SSS Get<SStateS, SSS>() where SStateS : struct, StateMonadValue<SSS, S, S> =>
            default(SStateS).Lift(state => (state, state, false));

        [Pure]
        public SSAW Listen<SWriterAW, SSAW>(RwsA ma) where SWriterAW : struct, WriterMonadValue<SSAW, W, (A, W)> =>
            default(SWriterAW).Lift(written =>
            {
                var (a, w, b) = default(SRwsA).Eval(ma, written);
                return b
                    ? (default((A, W)), default(MonoidW).Empty(), true)
                    : ((a, w), default(MonoidW).Append(written, w), false);
            });

        [Pure]
        public RwsA Local(Func<R, R> f, RwsA ma) =>
            default(SRwsA).Lift(env =>
            {
                var e = f(env.Item1);
                var (a, (r, w, s), b) = default(SRwsA).Eval(ma, (e, env.Item2, env.Item3));
                if (b) return (a, env, b);
                return (a, (r, default(MonoidW).Append(env.Item2, w), s), b);
            });

        [Pure]
        public SSU Put<SStateU, SSU>(S state) where SStateU : struct, StateMonadValue<SSU, S, Unit> =>
            default(SStateU).Lift(_ => (unit, state, false));

        [Pure]
        public RwsA Reader(Func<R, A> f) =>
            default(SRwsA).Lift(env => (f(env.Item1), env, false));

        [Pure]
        public RwsA Return(A x) =>
            default(SRwsA).Lift((x, (default(R), default(MonoidW).Empty(), default(S)), false));

        [Pure]
        public RwsA Return(Func<(R, W, S), (A, (R, W, S), bool)> f) =>
            default(SRwsA).Lift(f);

        [Pure]
        public RwsA State(Func<S, (A, S, bool)> f) =>
            default(SRwsA).Lift(rws =>
            {
                var (a, s, b) = f(rws.Item3);
                return (a, (rws.Item1, rws.Item2, s), b);
            });

        [Pure]
        public SSU Tell<SWriterU, SSU>(W what) where SWriterU : struct, WriterMonadValue<SSU, W, Unit> =>
            default(SWriterU).Lift(output => 
                (unit, default(MonoidW).Append(output, what), false));
    }
}
