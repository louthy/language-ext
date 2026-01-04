using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public class AtomSeq : Foldable<AtomSeq>
{
    static Fold<A, S> Foldable<AtomSeq>.FoldStep<A, S>(K<AtomSeq, A> ta, in S initialState)
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

    
    static Fold<A, S> Foldable<AtomSeq>.FoldStepBack<A, S>(K<AtomSeq, A> ta, in S initialState)
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
