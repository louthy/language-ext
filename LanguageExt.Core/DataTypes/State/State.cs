using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    /// <summary>
    /// State monad
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    public class State<S, A> : NewType<State<S, A>, (A, S, bool)>
    {
        /// <summary>
        /// Evaluate the state monad
        /// </summary>
        public readonly Func<S, (A Value, S State, bool IsBottom)> Eval;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value"></param>
        internal State((A, S, bool) value) : base(value) =>
            Eval = state => value;

        internal State(Func<S, (A, S, bool)> f) : base(default((A, S, bool))) =>
            Eval = f ?? (s => (default(A), s, true));

        [Pure]
        internal static State<S, A> From(Func<S, (A, S, bool)> f) =>
            new State<S, A>(f);

        [Pure]
        public State<S, IEnumerable<A>> AsEnumerable() =>
            Select(x => (new A[1] { x }).AsEnumerable());

        [Pure]
        public IEnumerable<A> AsEnumerable(S state)
        {
            var (x, s, b) = Eval(state);
            if (!b)
            {
                yield return x;
            }
        }

        public State<S, Unit> Iter(Action<A> action) =>
            default(MState<S, Unit>).Return(state =>
            {
                var (x, s, b) = Eval(state);
                if (!b) action(x);
                return (unit, b ? state : s, false);
            });

        [Pure]
        public new State<S, int> Count() =>
            default(MState<S, int>).Return(state =>
            {
                var (x, s, b) = Eval(state);
                return b
                    ? (0, state, false)
                    : (1, s, false);
            });

        [Pure]
        public State<S, bool> ForAll(Func<A, bool> pred) =>
            default(MState<S, bool>).Return(state =>
            {
                var (x, s, b) = Eval(state);
                return b
                    ? (false, state, false)
                    : (pred(x), s, false);
            });

        [Pure]
        public State<S, bool> Exists(Func<A, bool> pred) =>
            default(MState<S, bool>).Return(state =>
            {
                var (x, s, b) = Eval(state);
                return b
                    ? (false, state, false)
                    : (pred(x), s, false);
            });

        [Pure]
        public State<S, FState> Fold<FState>(FState initialState, Func<FState, A, FState> f) =>
            default(MState<S, FState>).Return(state =>
            {
                var (x, s, b) = Eval(state);
                return b
                    ? (default(FState), state, true)
                    : (f(initialState, x), s, false);
            });

        [Pure]
        public State<S, S> Fold(Func<S, A, S> f) =>
            default(MState<S, S>).Return(state =>
            {
                var (x, s, b) = Eval(state);
                return b
                    ? (default(S), state, true)
                    : (f(s, x), s, false);
            });

        [Pure]
        public State<S, B> Map<B>(Func<A, B> f) =>
            Select(f);

        /// <summary>
        /// modify::MonadState s m => (s -> s) -> m()
        /// 
        /// Monadic state transformer.
        /// 
        /// Maps an old state to a new state inside a state monad.The old state is thrown away.
        /// </summary>
        [Pure]
        public State<S, Unit> Modify(Func<S, S> f) =>
            default(MState<S, Unit>).State(s => (unit, f(s), false));

        [Pure]
        public State<S, B> Bind<B>(Func<A, State<S, B>> f) =>
            default(MState<S, A>).Bind<MState<S, B>, State<S, B>, B>(this, f);

        [Pure]
        public State<S, B> Select<B>(Func<A, B> f) =>
            default(MState<S, A>).Bind<MState<S, B>, State<S, B>, B>(this, a =>
            default(MState<S, B>).Return(f(a)));

        [Pure]
        public State<S, C> SelectMany<B, C>(
            Func<A, State<S, B>> bind,
            Func<A, B, C> project) =>
                default(MState<S, A>).Bind<MState<S, C>, State<S, C>, C>(this,    a =>
                default(MState<S, B>).Bind<MState<S, C>, State<S, C>, C>(bind(a), b =>
                default(MState<S, C>).Return(project(a, b))));

              [Pure]
        public State<S, A> Filter(Func<A, bool> pred) =>
            Where(pred);

        [Pure]
        public State<S, A> Where(Func<A, bool> pred) =>
            default(MState<S, A>).Return(state => {
                var (x, s, b) = Eval(state);
                if (b || !pred(x)) return (default(A), state, true);
                return (x, s, b);
            });
    }
}