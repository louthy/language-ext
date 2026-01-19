using System;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class HashMap<Key> : 
    Foldable<HashMap<Key>, TrieMap.FoldState>, 
    Functor<HashMap<Key>>, 
    MonoidK<HashMap<Key>>
{
    static K<HashMap<Key>, B> Functor<HashMap<Key>>.Map<A, B>(Func<A, B> f, K<HashMap<Key>, A> ma) =>
        ma.As().Map(f);
    
    static long Foldable<HashMap<Key>>.Count<A>(K<HashMap<Key>, A> ta) =>
        ta.As().Count;

    static TrieMap.FoldState IterableK<HashMap<Key>, TrieMap.FoldState>.StepSetup<A>(K<HashMap<Key>, A> ta) =>
        TrieMap.FoldState.Setup(ta.As().Value.Root);

    static bool IterableK<HashMap<Key>, TrieMap.FoldState>.Step<A>(
        K<HashMap<Key>, A> ta, ref TrieMap.FoldState refState, 
        out A value)
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

    static bool Foldable<HashMap<Key>>.IsEmpty<A>(K<HashMap<Key>, A> ta) =>
        ta.As().IsEmpty;

    static K<HashMap<Key>, A> SemigroupK<HashMap<Key>>.Combine<A>(K<HashMap<Key>, A> lhs, K<HashMap<Key>, A> rhs) =>
        lhs.As() + rhs.As();

    static K<HashMap<Key>, A> MonoidK<HashMap<Key>>.Empty<A>() =>
        HashMap<Key, A>.Empty;

    static Iterator<A> IterableK<HashMap<Key>>.ForwardIterator<A>(K<HashMap<Key>, A> fa) =>
        new Iterator.IterHashMapValue<EqDefault<Key>, Key, A>(
            TrieMap.IteratorState<EqDefault<Key>, Key, A>.Setup(fa.As().Value.Root));
}
