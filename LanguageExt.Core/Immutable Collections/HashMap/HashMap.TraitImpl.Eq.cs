using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class HashMapEq<EqKey, Key> : 
    Foldable<HashMapEq<EqKey, Key>>, 
    MonoidK<HashMapEq<EqKey, Key>>,
    Functor<HashMapEq<EqKey, Key>>
    where EqKey : Eq<Key>
{
    static int Foldable<HashMapEq<EqKey, Key>>.Count<A>(K<HashMapEq<EqKey, Key>, A> ta) =>
        ta.As().Count;

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
        var iter = ta.As().Values.GetIterator();
        return go(initialState);
        Fold<A, S> go(S state)
        {
            if (iter.IsEmpty)
            {
                return Fold.Done<A, S>(state);
            }
            else
            {
                iter = iter.Tail.Split();
                return Fold.Loop(state, iter.Head, go);
            }
        }
    }

    static Fold<A, S> Foldable<HashMapEq<EqKey, Key>>.FoldStepBack<A, S>(K<HashMapEq<EqKey, Key>, A> ta, in S initialState) =>
        // Order is undefined in a HashMap, so reversing the order makes no sense,
        // so let's take the most efficient option:
        ta.FoldStep(initialState);

}
