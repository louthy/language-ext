using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Range : Functor<Range>, Foldable<Range>
{
    static K<Range, B> Functor<Range>.Map<A, B>(Func<A, B> f, K<Range, A> ma)
    {
        var r = ma.As();
        return new Range<B>(f(r.From), f(r.To), f(r.Step), r.runRange.Select(f));
    }

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
}
