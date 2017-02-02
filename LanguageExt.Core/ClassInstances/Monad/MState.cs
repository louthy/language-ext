using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MState<S, A> : MonadState<S, A>
    {
        [Pure]
        public MB Bind<MONADB, MB, B>(State<S, A> ma, Func<A, MB> f) where MONADB : struct, Monad<S, MB, B> =>
            default(MONADB).Return(s1 =>
            {
                var (x, s2, bottom) = ma.Eval(s1);
                if (bottom) return (default(B), s1, true);
                return default(MONADB).Eval(f(x), s2, false);
            });

        public State<S, A> Fail(object err) =>
            State<S, A>.Bottom;

        public State<S, A> Fail(Exception err = null) =>
            State<S, A>.Bottom;

        [Pure]
        public State<S, A> Return(Func<S, (A, S, bool)> f) =>
            new State<S, A>(f);

        [Pure]
        public State<S, S> Get() =>
            State(s => (s, s, false));

        [Pure]
        public State<S, Unit> Put(S state) =>
            State(s => (unit, s, false));

        [Pure]
        public State<S, A> Return(A x) =>
            State(s => (x, s, false));

        [Pure]
        public (A, S, bool) Eval(State<S, A> ma, S state, bool bottom) =>
            bottom
                ? (default(A), state, bottom)
                : ma.Eval(state);

        [Pure]
        public State<S, B> State<B>(Func<S, (B, S, bool)> f) =>
            default(MState<S, S>).Bind<MState<S, B>, State<S, B>, B>(
                default(MState<S, A>).Get(), s =>
                {
                    var(a, s1, bottom) = f(s);
                    return bottom
                        ? default(MState<S, B>).Fail()
                        : default(MState<S, Unit>).Bind<MState<S, B>, State<S, B>, B>(
                                default(MState<S, S>).Put(s1), _ =>
                                    default(MState<S, B>).Return(a));
                });
    }
}
