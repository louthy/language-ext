using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

public class AtomHashMap<Key> : Foldable<AtomHashMap<Key>, TrieMap.FoldState>
{
    static Iterator<A> IterableK<AtomHashMap<Key>>.ForwardIterator<A>(K<AtomHashMap<Key>, A> fa) =>
        new Iterator.IterHashMapValue<EqDefault<Key>, Key, A>(
            TrieMap.IteratorState<EqDefault<Key>, Key, A>.Setup(fa.As().ToHashMap().Value.Root));

    static TrieMap.FoldState IterableK<AtomHashMap<Key>, TrieMap.FoldState>.StepSetup<A>(K<AtomHashMap<Key>, A> ta) => 
        TrieMap.FoldState.Setup(ta.As().ToHashMap().Value.Root);

    static bool IterableK<AtomHashMap<Key>, TrieMap.FoldState>.Step<A>(K<AtomHashMap<Key>, A> ta, ref TrieMap.FoldState refState, out A value) 
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

    static TrieMap.FoldState IterableK<AtomHashMapEq<EqKey, Key>, TrieMap.FoldState>.StepSetup<A>(K<AtomHashMapEq<EqKey, Key>, A> ta) => 
        TrieMap.FoldState.Setup(ta.As().ToHashMap().Value.Root);

    static bool IterableK<AtomHashMapEq<EqKey, Key>, TrieMap.FoldState>.Step<A>(K<AtomHashMapEq<EqKey, Key>, A> ta, ref TrieMap.FoldState refState, out A value) 
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
