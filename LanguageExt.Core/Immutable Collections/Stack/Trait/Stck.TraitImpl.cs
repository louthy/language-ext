using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Stck :
    Foldable<Stck>,
    MonoidK<Stck>
{
    static Fold<A, S> Foldable<Stck>.FoldStep<A, S>(K<Stck, A> ta, in S initialState)
    {
        return go(ta.As())(initialState);

        Func<S, Fold<A, S>> go(Stck<A> stack) =>
            state =>
                stack switch
                {
                    Stck<A>.Nil               => Fold.Done<A, S>(state),
                    Stck<A>.Top(var t, var r) => Fold.Loop(state, t, go(r)),
                    _                         => throw new InvalidOperationException("Invalid stack state")
                };
    }

    static Fold<A, S> Foldable<Stck>.FoldStepBack<A, S>(K<Stck, A> ta, in S initialState) 
    {
        return go(ta.As().Reverse())(initialState);

        Func<S, Fold<A, S>> go(Stck<A> stack) =>
            state =>
                stack switch
                {
                    Stck<A>.Nil               => Fold.Done<A, S>(state),
                    Stck<A>.Top(var t, var r) => Fold.Loop(state, t, go(r)),
                    _                         => throw new InvalidOperationException("Invalid stack state")
                };
    }


    static K<Stck, A> SemigroupK<Stck>.Combine<A>(K<Stck, A> lhs, K<Stck, A> rhs) => 
        +lhs + +rhs;

    static K<Stck, A> MonoidK<Stck>.Empty<A>() => 
        Stck<A>.Empty;
}
