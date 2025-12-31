using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Range : Foldable<Range>
{
    static S Foldable<Range>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Range, A> ta)
    {
        foreach (var x in ta.As().runRange)
        {
            if (!predicate((state, x))) return state;
            state = f(x)(state);
        }
        return state;
    }
    
    static S Foldable<Range>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state, 
        K<Range, A> ta)
    {
        foreach (var x in ta.As().runRange.Reverse())
        {
            if (!predicate((state, x))) return state;
            state = f(state)(x);
        }
        return state;
    }

    static Fold<A, S> Foldable<Range>.FoldStep<A, S>(K<Range, A> ta, S initialState) 
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        var iter = ta.As().GetEnumerator();
        return go(initialState);
        Fold<A, S> go(S state) =>
            iter.MoveNext()
                ? Fold.Loop(state, iter.Current, go)
                : Fold.Done<A, S>(state);
    }

    static Fold<A, S> Foldable<Range>.FoldStepBack<A, S>(K<Range, A> ta, S initialState) 
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
