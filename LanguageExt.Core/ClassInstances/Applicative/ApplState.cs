using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplState<S, A, B, C> :
        Applicative<State<S, Func<A, Func<B, C>>>, State<S, Func<B, C>>, State<S,A>, State<S, B>, State<S,C>, A, B, C>
    {
        public static readonly ApplState<S, A, B, C> Inst = default;

        [Pure]
        public State<S, Func<B, C>> Apply(State<S, Func<A, Func<B, C>>> fabc, State<S, A> fa) =>
            ApplState<S, A, Func<B, C>>.Inst.Apply(fabc, fa);

        [Pure]
        public State<S, C> Apply(State<S, Func<A, Func<B, C>>> fabc, State<S, A> fa, State<S, B> fb) =>
            ApplState<S, B, C>.Inst.Apply(Apply(fabc, fa), fb);

        [Pure]
        public State<S, A> Pure(A x) =>
            MState<S, A>.Inst.Return(_ => x);
    }

    public struct ApplState<S, A, B> :
        Functor<State<S, A>, State<S, B>, A, B>,
        Applicative<State<S, Func<A, B>>, State<S, A>, State<S, B>, A, B>
    {
        public static readonly ApplState<S, A, B> Inst = default;

        [Pure]
        public State<S, B> Action(State<S, A> fa, State<S, B> fb) =>
            MState<S, A>.Inst.Bind<MState<S, B>, State<S, B>, B>(fa, _ => fb);

        [Pure]
        public State<S, B> Apply(State<S, Func<A, B>> fab, State<S, A> fa) =>
            MState<S, Func<A, B>>.Inst.Bind<MState<S, B>, State<S, B>, B>(fab, f =>
                MState<S, A>.Inst.Bind<MState<S, B>, State<S, B>, B>(fa, a =>
                    MState<S, B>.Inst.Return(_ => f(a))));
        [Pure]
        public State<S, B> Map(State<S, A> ma, Func<A, B> f) =>
            FState<S, A, B>.Inst.Map(ma, f);

        [Pure]
        public State<S, A> Pure(A x) =>
            MState<S, A>.Inst.Return(_ => x);
    }

}
