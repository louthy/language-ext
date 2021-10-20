using System;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MState<S, A> : 
        MonadState<S, A>, 
        Monad<S, (S State, bool IsFaulted), State<S, A>, A>
    {
        [Pure]
        public MB Bind<MONADB, MB, B>(State<S, A> ma, Func<A, MB> f) where MONADB : struct, Monad<S, (S State, bool IsFaulted), MB, B> =>
            default(MONADB).Run(state =>
            {
                var (a, sa, faulted) = ma(state);
                return faulted
                    ? default(MONADB).Fail(default(MState<S, A>).Fail())
                    : default(MONADB).BindReturn((sa, faulted), f(a));
            });

        [Pure]
        public MB BindAsync<MONADB, MB, B>(State<S, A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<S, (S State, bool IsFaulted), MB, B> =>
            default(MONADB).RunAsync(state =>
            {
                var (a, sa, faulted) = ma(state);
                return faulted
                    ? default(MONADB).Fail(default(MState<S, A>).Fail()).AsTask()
                    : default(MONADB).BindReturn((sa, faulted), f(a)).AsTask();
            });

        [Pure]
        public State<S, A> BindReturn((S State, bool IsFaulted) output, State<S, A> mb) => 
            _ => mb(output.State);

        [Pure]
        public State<S, A> Fail(object err = null) =>
            new State<S, A>(state => (default(A), state, true));

        [Pure]
        public State<S, S> Get() => state =>
            (state, state, false);

        [Pure]
        public State<S, Unit> Put(S state) => _ =>
            (unit, state, false);

        [Pure]
        public State<S, A> Return(Func<S, A> f) => state =>
            (f(state), state, false);

        [Pure]
        public State<S, A> Plus(State<S, A> ma, State<S, A> mb) => state =>
        {
            var (a, newstate, faulted) = ma(state);
            return faulted
                ? mb(state)
                : (a, newstate, faulted);
        };

        [Pure]
        public State<S, A> Zero() => state =>
            (default(A), state, true);

        [Pure]
        public Func<S, FoldState> Fold<FoldState>(State<S, A> fa, FoldState initialState, Func<FoldState, A, FoldState> f) =>
            state =>
            {
                var (a, newstate, faulted) = fa(state);
                return faulted
                    ? initialState
                    : f(initialState, a);
            };

        [Pure]
        public Func<S, FoldState> FoldBack<FoldState>(State<S, A> fa, FoldState state, Func<FoldState, A, FoldState> f) =>
            Fold(fa, state, f);

        [Pure]
        public Func<S, int> Count(State<S, A> fa) =>
            Fold(fa, 0, (_, __) => 1);

        [Pure]
        public State<S, A> Run(Func<S, State<S, A>> ma) => 
            state => ma(state)(state);

        [Pure]
        public State<S, A> State(Func<S, A> f) => state =>
            (f(state), state, false);

        [Pure]
        public State<S, A> Apply(Func<A, A, A> f, State<S, A> fa, State<S, A> fb) =>
            from a in fa
            from b in fb
            select f(a, b);
    }
}
