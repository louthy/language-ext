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
    static S Foldable<HashMapEq<EqKey, Key>>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state, 
        K<HashMapEq<EqKey, Key>, A> ta)
    {
        foreach (var x in ta.As())
        {
            if (!predicate((state, x.Value))) return state;
            state = f(x.Value)(state);
        }
        return state;
    }
    
    static S Foldable<HashMapEq<EqKey, Key>>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state, 
        K<HashMapEq<EqKey, Key>, A> ta)
    {
        foreach (var x in ta.As().Value.Reverse())
        {
            if (!predicate((state, x.Value))) return state;
            state = f(state)(x.Value);
        }
        return state;
    }
    
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
}
