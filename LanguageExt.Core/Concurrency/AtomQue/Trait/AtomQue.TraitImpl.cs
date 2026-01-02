using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public class AtomQue : Foldable<AtomQue>
{
    static Fold<A, S> Foldable<AtomQue>.FoldStep<A, S>(K<AtomQue, A> ta, in S initialState)
    {
        var iter = ta.As().GetEnumerator();
        return go(initialState);
        Fold<A, S> go(S state) =>
            iter.MoveNext()
                ? Fold.Loop(state, iter.Current, go)
                : Fold.Done<A, S>(state);
    }
    
    static Fold<A, S> Foldable<AtomQue>.FoldStepBack<A, S>(K<AtomQue, A> ta, in S initialState) 
    {
        var iter = ta.As().Reverse().GetEnumerator();
        return go(initialState);
        Fold<A, S> go(S state) =>
            iter.MoveNext()
                ? Fold.Loop(state, iter.Current, go)
                : Fold.Done<A, S>(state);
    }    
}
