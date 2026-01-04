using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Range : Foldable<Range>
{
    static Fold<A, S> Foldable<Range>.FoldStep<A, S>(K<Range, A> ta, in S initialState) 
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

    static Fold<A, S> Foldable<Range>.FoldStepBack<A, S>(K<Range, A> ta, in S initialState) 
    {
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
