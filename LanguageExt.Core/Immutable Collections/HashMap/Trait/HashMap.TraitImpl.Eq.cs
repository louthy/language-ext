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

    static void Foldable<HashMapEq<EqKey, Key>, TrieMap.FoldState>.FoldStepSetup<A>(K<HashMapEq<EqKey, Key>, A> ta, ref TrieMap.FoldState refState) => 
        TrieMap.FoldState.Setup(ref refState, ta.As().Value.Root);

    static bool Foldable<HashMapEq<EqKey, Key>, TrieMap.FoldState>.FoldStep<A>(K<HashMapEq<EqKey, Key>, A> ta, ref TrieMap.FoldState refState, out A value) 
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

    static void Foldable<HashMapEq<EqKey, Key>, TrieMap.FoldState>.FoldStepBackSetup<A>(K<HashMapEq<EqKey, Key>, A> ta, ref TrieMap.FoldState refState) => 
        TrieMap.FoldState.Setup(ref refState, ta.As().Value.Root);

    static bool Foldable<HashMapEq<EqKey, Key>, TrieMap.FoldState>.FoldStepBack<A>(K<HashMapEq<EqKey, Key>, A> ta, ref TrieMap.FoldState refState, out A value) 
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
    
    static Fold<A, S> Foldable<HashMapEq<EqKey, Key>>.FoldStep<A, S>(K<HashMapEq<EqKey, Key>, A> ta, in S initialState)
    {
        var items = ta.As();
        return go(items.Values.GetIterator())(initialState);

        static Func<S, Fold<A, S>> go(Iterator<A> iter) =>
            state =>
            {
                if (iter.IsEmpty)
                {
                    return Fold.Done<A, S>(state);
                }
                else
                {
                    return Fold.Loop(state, iter.Head, go(iter.Tail.Clone()));
                }
            };
    }

    static Fold<A, S> Foldable<HashMapEq<EqKey, Key>>.FoldStepBack<A, S>(K<HashMapEq<EqKey, Key>, A> ta, in S initialState)
    {
        // Order is undefined in a HashMap, so reversing the order makes no sense,
        // so let's take the most efficient option:
        var items = ta.As();
        return go(items.Values.GetIterator())(initialState);

        static Func<S, Fold<A, S>> go(Iterator<A> iter) =>
            state =>
            {
                if (iter.IsEmpty)
                {
                    return Fold.Done<A, S>(state);
                }
                else
                {
                    return Fold.Loop(state, iter.Head, go(iter.Tail.Clone()));
                }
            };
    }


}
