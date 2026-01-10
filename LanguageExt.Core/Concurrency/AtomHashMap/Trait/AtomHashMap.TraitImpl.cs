using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

public class AtomHashMap<Key> : Foldable<AtomHashMap<Key>, TrieMap.FoldState>
{
    static Iterator<A> IterableK<AtomHashMap<Key>>.ForwardIterator<A>(K<AtomHashMap<Key>, A> fa) =>
        new Iterator.IterHashMapValue<EqDefault<Key>, Key, A>(
            TrieMap.IteratorState<EqDefault<Key>, Key, A>.Setup(fa.As().ToHashMap().Value.Root));

    static void Foldable<AtomHashMap<Key>, TrieMap.FoldState>.FoldStepSetup<A>(K<AtomHashMap<Key>, A> ta, ref TrieMap.FoldState refState) => 
        TrieMap.FoldState.Setup(ref refState, ta.As().ToHashMap().Value.Root);

    static bool Foldable<AtomHashMap<Key>, TrieMap.FoldState>.FoldStep<A>(K<AtomHashMap<Key>, A> ta, ref TrieMap.FoldState refState, out A value) 
    {
        if (TrieMap.FoldState.Step<EqDefault<Key>, Key, A>(ref refState, out var kv))
        {
            value = kv.Value;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }
}

public class AtomHashMapEq<EqKey, Key> : Foldable<AtomHashMapEq<EqKey, Key>, TrieMap.FoldState>
    where EqKey : Eq<Key>
{
    static Iterator<A> IterableK<AtomHashMapEq<EqKey, Key>>.ForwardIterator<A>(K<AtomHashMapEq<EqKey, Key>, A> fa) => 
        new Iterator.IterHashMapValue<EqKey, Key, A>(
            TrieMap.IteratorState<EqKey, Key, A>.Setup(fa.As().ToHashMap().Value.Root));

    static void Foldable<AtomHashMapEq<EqKey, Key>, TrieMap.FoldState>.FoldStepSetup<A>(K<AtomHashMapEq<EqKey, Key>, A> ta, ref TrieMap.FoldState refState) => 
        TrieMap.FoldState.Setup(ref refState, ta.As().ToHashMap().Value.Root);

    static bool Foldable<AtomHashMapEq<EqKey, Key>, TrieMap.FoldState>.FoldStep<A>(K<AtomHashMapEq<EqKey, Key>, A> ta, ref TrieMap.FoldState refState, out A value) 
    {
        if (TrieMap.FoldState.Step<EqKey, Key, A>(ref refState, out var kv))
        {
            value = kv.Value;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }
}
