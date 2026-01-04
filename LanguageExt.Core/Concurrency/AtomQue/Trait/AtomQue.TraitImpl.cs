using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public class AtomQue : Foldable<AtomQue>
{
    static Fold<A, S> Foldable<AtomQue>.FoldStep<A, S>(K<AtomQue, A> ta, in S initialState)
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
    
    static Fold<A, S> Foldable<AtomQue>.FoldStepBack<A, S>(K<AtomQue, A> ta, in S initialState) 
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
