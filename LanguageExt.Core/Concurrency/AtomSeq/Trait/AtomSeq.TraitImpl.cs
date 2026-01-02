using LanguageExt.Traits;

namespace LanguageExt;

public class AtomSeq : Foldable<AtomSeq>
{
    static Fold<A, S> Foldable<AtomSeq>.FoldStep<A, S>(K<AtomSeq, A> ta, in S initialState) =>
        ta.As().ToSeq().FoldStep(initialState);
    
    static Fold<A, S> Foldable<AtomSeq>.FoldStepBack<A, S>(K<AtomSeq, A> ta, in S initialState) => 
        ta.As().ToSeq().FoldStepBack(initialState);
}
