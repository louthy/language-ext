using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Tuple foldable instance
    /// Supports tuples up to 7 elements where each element is of the same type A
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    public struct FoldTuple<A> :
        Foldable<Tuple<A>, A>,
        Foldable<Tuple<A, A>, A>,
        Foldable<Tuple<A, A, A>, A>,
        Foldable<Tuple<A, A, A, A>, A>,
        Foldable<Tuple<A, A, A, A, A>, A>,
        Foldable<Tuple<A, A, A, A, A, A>, A>,
        Foldable<Tuple<A, A, A, A, A, A, A>, A>,
        Foldable<ValueTuple<A>, A>,
        Foldable<ValueTuple<A, A>, A>,
        Foldable<ValueTuple<A, A, A>, A>,
        Foldable<ValueTuple<A, A, A, A>, A>,
        Foldable<ValueTuple<A, A, A, A, A>, A>,
        Foldable<ValueTuple<A, A, A, A, A, A>, A>,
        Foldable<ValueTuple<A, A, A, A, A, A, A>, A>
    {
        public static readonly FoldTuple<A> Inst = default(FoldTuple<A>);

        [Pure]
        public Func<Unit, int> Count(Tuple<A> fa) => _ =>
            1;

        [Pure]
        public Func<Unit, S> Fold<S>(Tuple<A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Tuple<A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(ValueTuple<A> fa) => _ =>
           1;

        [Pure]
        public Func<Unit, S> Fold<S>(ValueTuple<A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(ValueTuple<A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(Tuple<A, A> fa) => _ =>
           2;

        [Pure]
        public Func<Unit, S> Fold<S>(Tuple<A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Tuple<A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(ValueTuple<A, A> fa) => _ =>
           2;

        [Pure]
        public Func<Unit, S> Fold<S>(ValueTuple<A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(ValueTuple<A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(Tuple<A, A, A> fa) => _ =>
           3;

        [Pure]
        public Func<Unit, S> Fold<S>(Tuple<A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Tuple<A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(ValueTuple<A, A, A> fa) => _ =>
            3;

        [Pure]
        public Func<Unit, S> Fold<S>(ValueTuple<A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(ValueTuple<A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, S> Fold<S>(Tuple<A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            state = f(state, fa.Item4);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Tuple<A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item4);
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(Tuple<A, A, A, A> fa) => _ =>
            4;

        [Pure]
        public Func<Unit, S> Fold<S>(ValueTuple<A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            state = f(state, fa.Item4);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(ValueTuple<A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item4);
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(ValueTuple<A, A, A, A> fa) => _ =>
           4;

        [Pure]
        public Func<Unit, S> Fold<S>(Tuple<A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            state = f(state, fa.Item4);
            state = f(state, fa.Item5);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Tuple<A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item5);
            state = f(state, fa.Item4);
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(Tuple<A, A, A, A, A> fa) => _ =>
           5;

        [Pure]
        public Func<Unit, S> Fold<S>(ValueTuple<A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            state = f(state, fa.Item4);
            state = f(state, fa.Item5);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(ValueTuple<A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item5);
            state = f(state, fa.Item4);
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(ValueTuple<A, A, A, A, A> fa) => _ =>
           5;

        [Pure]
        public Func<Unit, S> Fold<S>(Tuple<A, A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            state = f(state, fa.Item4);
            state = f(state, fa.Item5);
            state = f(state, fa.Item6);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Tuple<A, A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item6);
            state = f(state, fa.Item5);
            state = f(state, fa.Item4);
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(Tuple<A, A, A, A, A, A> fa) => _ =>
           6;

        [Pure]
        public Func<Unit, S> Fold<S>(ValueTuple<A, A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            state = f(state, fa.Item4);
            state = f(state, fa.Item5);
            state = f(state, fa.Item6);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(ValueTuple<A, A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item6);
            state = f(state, fa.Item5);
            state = f(state, fa.Item4);
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(ValueTuple<A, A, A, A, A, A> fa) => _ =>
           6;

        [Pure]
        public Func<Unit, S> Fold<S>(Tuple<A, A, A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            state = f(state, fa.Item4);
            state = f(state, fa.Item5);
            state = f(state, fa.Item6);
            state = f(state, fa.Item7);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Tuple<A, A, A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item7);
            state = f(state, fa.Item6);
            state = f(state, fa.Item5);
            state = f(state, fa.Item4);
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(Tuple<A, A, A, A, A, A, A> fa) => _ =>
           7;

        [Pure]
        public Func<Unit, S> Fold<S>(ValueTuple<A, A, A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            state = f(state, fa.Item4);
            state = f(state, fa.Item5);
            state = f(state, fa.Item6);
            state = f(state, fa.Item7);
            return state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(ValueTuple<A, A, A, A, A, A, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            state = f(state, fa.Item7);
            state = f(state, fa.Item6);
            state = f(state, fa.Item5);
            state = f(state, fa.Item4);
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        };

        [Pure]
        public Func<Unit, int> Count(ValueTuple<A, A, A, A, A, A, A> fa) => _ =>
            7;
    }
}
