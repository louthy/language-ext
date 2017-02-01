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
        static readonly State<S, A> Bottom = new State<S, A>((default(A), default(S), true));

        [Pure]
        public MB Bind<MONADB, MB, B>(State<S, A> ma, Func<A, MB> f) where MONADB : struct, Monad<S, MB, B> =>
            default(MONADB).Return(s1 =>
            {
                var (x, s2, bottom) = ma.Eval(s1);
                if (bottom) return (default(B), s1, true);
                return default(MONADB).Eval(f(x), s2, false);
            });

        public State<S, A> Fail(object err) =>
            Bottom;

        public State<S, A> Fail(Exception err = null) =>
            Bottom;

        [Pure]
        public State<S, A> Return(Func<S, (A, S, bool)> f) =>
            new State<S, A>(f);

        [Pure]
        public State<S, S> Get() =>
            default(MState<S, S>).State(s => (s, s, false));

        [Pure]
        public State<S, Unit> Put<B>(B state) =>
            default(MState<S, Unit>).State(s => (unit, s, false));

        [Pure]
        public State<S, A> Return(A x) =>
            State(st => (x, st, false));

        [Pure]
        public (A, S, bool) Eval(State<S, A> ma, S state, bool bottom) =>
            bottom
                ? (default(A), state, bottom)
                : ma.Eval(state);

        [Pure]
        public State<S, A> State(Func<S, (A, S, bool)> f) =>
            default(MState<S, S>).Bind<MState<S, A>, State<S, A>, A>(Get(), s =>
            {
                var (a, s1, bottom) = f(s);
                return bottom 
                    ? default(MState<S, A>).Fail()
                    : default(MState<S, Unit>).Bind<MState<S, A>, State<S, A>, A>(
                          default(MState<S, S>).Put(s1), _ =>
                              default(MState<S, A>).Return(a));
            });
    }
}
