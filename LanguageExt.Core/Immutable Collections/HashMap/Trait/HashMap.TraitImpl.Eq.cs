using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class HashMapEq<EqKey, Key> : 
    Foldable<HashMapEq<EqKey, Key>, TrieMap.FoldState>, 
    MonoidK<HashMapEq<EqKey, Key>>,
    Functor<HashMapEq<EqKey, Key>>
    where EqKey : Eq<Key>
{
    static int Foldable<HashMapEq<EqKey, Key>>.Count<A>(K<HashMapEq<EqKey, Key>, A> ta) =>
        ta.As().Count;

    static TrieMap.FoldState IterableK<HashMapEq<EqKey, Key>, TrieMap.FoldState>.StepSetup<A>(K<HashMapEq<EqKey, Key>, A> ta) => 
        TrieMap.FoldState.Setup(ta.As().Value.Root);

    static bool IterableK<HashMapEq<EqKey, Key>, TrieMap.FoldState>.Step<A>(K<HashMapEq<EqKey, Key>, A> ta, ref TrieMap.FoldState refState, out A value) 
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

    static bool Foldable<HashMapEq<EqKey, Key>>.IsEmpty<A>(K<HashMapEq<EqKey, Key>, A> ta) =>
        ta.As().IsEmpty;

    static K<HashMapEq<EqKey, Key>, A> SemigroupK<HashMapEq<EqKey, Key>>.Combine<A>(K<HashMapEq<EqKey, Key>, A> lhs, K<HashMapEq<EqKey, Key>, A> rhs) =>
        lhs.As() + rhs.As();

    static K<HashMapEq<EqKey, Key>, A> MonoidK<HashMapEq<EqKey, Key>>.Empty<A>() =>
        HashMap<EqKey, Key, A>.Empty;

    public static K<HashMapEq<EqKey, Key>, B> Map<A, B>(Func<A, B> f, K<HashMapEq<EqKey, Key>, A> ma) =>
        new HashMap<EqKey, Key, B>(ma.As().Value.Select(kv => (kv.Key, f(kv.Value))));

    public static Iterator<A> ForwardIterator<A>(K<HashMapEq<EqKey, Key>, A> fa) => 
        new Iterator.IterHashMapValue<EqKey, Key, A>(
            TrieMap.IteratorState<EqKey, Key, A>.Setup(fa.As().Value.Root));
}
