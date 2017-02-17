using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct SState<S, A> : StateMonadValue<State<S, A>, S, A>
    {
        static readonly State<S, A> bottom = new State<S, A>(_ => (default(A), default(S), true));

        [Pure]
        public (A, S, bool) Eval(State<S, A> r, S state) =>
            r.eval(state);

        [Pure]
        public State<S, A> Lift((A, S, bool) value) =>
            new State<S, A>(_ => value);

        [Pure]
        public State<S, A> Lift(Func<S, (A, S, bool)> f) =>
            new State<S, A>(f);

        [Pure]
        public State<S, A> Bottom => bottom;
    }
}