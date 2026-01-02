using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public class AtomHashMap<Key> : Foldable<AtomHashMap<Key>>
{
    static Fold<A, S> Foldable<AtomHashMap<Key>>.FoldStep<A, S>(K<AtomHashMap<Key>, A> ta, in S initialState) =>
        ta.As().ToHashMap().FoldStep(initialState);
    
    static Fold<A, S> Foldable<AtomHashMap<Key>>.FoldStepBack<A, S>(K<AtomHashMap<Key>, A> ta, in S initialState) =>
        ta.As().ToHashMap().FoldStepBack(initialState);
}

public class AtomHashMapEq<EqKey, Key> : Foldable<AtomHashMapEq<EqKey, Key>>
    where EqKey : Eq<Key>
{
    static Fold<A, S> Foldable<AtomHashMapEq<EqKey, Key>>.FoldStep<A, S>(K<AtomHashMapEq<EqKey, Key>, A> ta, in S initialState) =>
        ta.As().ToHashMap().FoldStep(initialState);
    
    static Fold<A, S> Foldable<AtomHashMapEq<EqKey, Key>>.FoldStepBack<A, S>(K<AtomHashMapEq<EqKey, Key>, A> ta, in S initialState) =>
        ta.As().ToHashMap().FoldStepBack(initialState);
}
