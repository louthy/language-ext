using LanguageExt.TypeClasses;
using System;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Tuple foldable instance
    /// Supports tuples of 2 or 3 elements where each element is of the same type A
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    public struct FoldTuple<A> :
        Foldable<Tuple<A, A>, A>,
        Foldable<ValueTuple<A, A>, A>,
        Foldable<Tuple<A, A, A>, A>,
        Foldable<ValueTuple<A, A, A>, A>
    {
        public int Count(Tuple<A, A> fa) =>
            2;

        public S Fold<S>(Tuple<A, A> fa, S state, Func<S, A, S> f)
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            return state;
        }

        public S FoldBack<S>(Tuple<A, A> fa, S state, Func<S, A, S> f)
        {
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        }

        public int Count(ValueTuple<A, A> fa) =>
            2;

        public S Fold<S>(ValueTuple<A, A> fa, S state, Func<S, A, S> f)
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            return state;
        }

        public S FoldBack<S>(ValueTuple<A, A> fa, S state, Func<S, A, S> f)
        {
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        }

        public int Count(Tuple<A, A, A> fa) =>
            3;

        public S Fold<S>(Tuple<A, A, A> fa, S state, Func<S, A, S> f)
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            return state;
        }

        public S FoldBack<S>(Tuple<A, A, A> fa, S state, Func<S, A, S> f)
        {
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        }

        public int Count(ValueTuple<A, A, A> fa) =>
            3;

        public S Fold<S>(ValueTuple<A, A, A> fa, S state, Func<S, A, S> f)
        {
            state = f(state, fa.Item1);
            state = f(state, fa.Item2);
            state = f(state, fa.Item3);
            return state;
        }

        public S FoldBack<S>(ValueTuple<A, A, A> fa, S state, Func<S, A, S> f)
        {
            state = f(state, fa.Item3);
            state = f(state, fa.Item2);
            state = f(state, fa.Item1);
            return state;
        }
    }
}
