using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MRWS<R, W, S, A> : MonadRWS<R, W, S, A>
    {
        [Pure]
        public MB Bind<MONADB, MB, B>(RWS<R, W, S, A> ma, Func<A, MB> f) where MONADB : struct, Monad<(R,W,S), MB, B> =>
            default(MONADB).Return( rws =>
            {
                var (x, (r,w,s), bottom) = ma.Eval(rws);
                if (bottom) return (default(B), (r, w, s), true);
                return default(MONADB).Eval(f(x), (r, w, s), false);
            });

        public RWS<R, W, S, A> Fail(object err) =>
            RWS<R, W, S, A>.Bottom;

        public RWS<R, W, S, A> Fail(Exception err = null) =>
            RWS<R, W, S, A>.Bottom;

        [Pure]
        public RWS<R, W, S, A> Return(Func<(R,W,S), (A, (R, W, S), bool)> f) =>
            new RWS<R, W, S, A>(f);

        [Pure]
        public RWS<R, W, S, S> Get =>
            RWSS<R, W, S, A>.Get;

        [Pure]
        public RWS<R, W, S, R> Ask =>
            RWSS<R, W, S, A>.Ask;

        [Pure]
        public RWS<R, W, S, Unit> Put(S state) =>
            new RWS<R, W, S, Unit>(rws => (unit, (rws.Item1, rws.Item2, state), false));

        [Pure]
        public RWS<R, W, S, A> Return(A x) =>
            new RWS<R, W, S, A>(rws => (x, rws, false));

        [Pure]
        public RWS<R, W, S, B> RWS<B>(Func<(R,W,S), B> f) =>
            new RWS<R, W, S, B>(rws => (f(rws), rws, false));

        [Pure]
        public (A, (R, W, S), bool) Eval(RWS<R, W, S, A> ma, (R,W,S) rws, bool bottom) =>
            bottom
                ? (default(A), rws, bottom)
                : ma.Eval(rws);

        [Pure]
        public State<S, B> State<B>(Func<S, (B, S, bool)> f) =>
            default(MState<S, S>).Bind<MState<S, B>, State<S, B>, B>(
                default(MState<S, A>).Get, s =>
                {
                    var(a, s1, bottom) = f(s);
                    return bottom
                        ? default(MState<S, B>).Fail()
                        : default(MState<S, Unit>).Bind<MState<S, B>, State<S, B>, B>(
                                default(MState<S, S>).Put(s1), _ =>
                                    default(MState<S, B>).Return(a));
                });

        [Pure]
        public RWS<R, W, S, A> Local(Func<R, R> f, Reader<R, A> ma) =>
            new RWS<R, W, S, A>(env =>
            {
                var r = f(env.Item1);
                var (a, r2, b) = ma.Eval(r);
                if (b) return (a, (env.Item1, env.Item2, env.Item3), b);
                return (a, (r2, env.Item2, env.Item3), b);
            });
    }

    internal class RWSS<R,W,S,A>
    {
        public static readonly RWS<R, W, S, R> Ask = new RWS<R, W, S, R>(rws => (rws.Item1, (rws.Item1, rws.Item2, rws.Item3), false));
        public static readonly RWS<R, W, S, S> Get = new RWS<R, W, S, S>(rws => (rws.Item3, (rws.Item1, rws.Item2, rws.Item3), false));
    }
}
