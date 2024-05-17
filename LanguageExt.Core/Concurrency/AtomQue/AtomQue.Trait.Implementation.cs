using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public class AtomQue : Foldable<AtomQue>
{
    static S Foldable<AtomQue>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state,
        K<AtomQue, A> ta)
    {
        foreach (var item in ta.As())
        {
            if (predicate((state, item))) return state;
            state = f(item)(state);
        }
        return state;
    }
    
    static S Foldable<AtomQue>.FoldBackWhile<A, S>(Func<S, Func<A, S>> f, Func<(S State, A Value), bool> predicate, S state, K<AtomQue, A> ta)
    {
        foreach (var item in ta.As().Reverse())
        {
            if (predicate((state, item))) return state;
            state = f(state)(item);
        }
        return state;
    }
}
