using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Range : Foldable<Range>
{
    static Fold<A, S> Foldable<Range>.FoldStep<A, S>(K<Range, A> ta, in S initialState) 
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        var iter = ta.As().GetEnumerator();
        return go(initialState);
        Fold<A, S> go(S state) =>
            iter.MoveNext()
                ? Fold.Loop(state, iter.Current, go)
                : Fold.Done<A, S>(state);
    }

    static Fold<A, S> Foldable<Range>.FoldStepBack<A, S>(K<Range, A> ta, in S initialState) 
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        var iter = ta.As().GetEnumerator();
        return go(initialState);
        Fold<A, S> go(S state) =>
            iter.MoveNext()
                ? Fold.Loop(state, iter.Current, go)
                : Fold.Done<A, S>(state);
    }
}
