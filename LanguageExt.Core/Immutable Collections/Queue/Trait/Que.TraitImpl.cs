using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class Que : Foldable<Que>
{
    public static Fold<A, S> FoldStep<A, S>(K<Que, A> ta, in S initialState)
    {
        var items = ta.As();
        return go(items.GetIterator())(initialState);

        static Func<S, Fold<A, S>> go(Iterator<A> iter) =>
            state =>
            {
                if (iter.IsEmpty)
                {
                    return Fold.Done<A, S>(state);
                }
                else
                {
                    return Fold.Loop(state, iter.Head, go(iter.Tail.Clone()));
                }
            };
    }

    public static Fold<A, S> FoldStepBack<A, S>(K<Que, A> ta, in S initialState)
    {
        // Order is undefined in a HashSet, so reversing the order makes no sense,
        // so let's take the most efficient option:
        var items = ta.As();
        return go(items.Reverse().GetIterator())(initialState);

        static Func<S, Fold<A, S>> go(Iterator<A> iter) =>
            state =>
            {
                if (iter.IsEmpty)
                {
                    return Fold.Done<A, S>(state);
                }
                else
                {
                    return Fold.Loop(state, iter.Head, go(iter.Tail.Clone()));
                }
            };
    }
}
