using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class HashMap<Key> : 
    Foldable<HashMap<Key>>, 
    Functor<HashMap<Key>>, 
    MonoidK<HashMap<Key>>
{
    static K<HashMap<Key>, B> Functor<HashMap<Key>>.Map<A, B>(Func<A, B> f, K<HashMap<Key>, A> ma)
    {
        return new HashMap<Key, B>(Go());
        IEnumerable<(Key, B)> Go()
        {
            foreach (var x in ma.As())
            {
                yield return (x.Key, f(x.Value));
            }
        }
    }
    
    static S Foldable<HashMap<Key>>.FoldWhile<A, S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S state, K<HashMap<Key>, A> ta)
    {
        foreach (var x in ta.As())
        {
            if (!predicate((state, x.Value))) return state;
            state = f(x.Value)(state);
        }
        return state;
    }
    
    static S Foldable<HashMap<Key>>.FoldBackWhile<A, S>(Func<S, Func<A, S>> f, Func<(S State, A Value), bool> predicate, S state, K<HashMap<Key>, A> ta)
    {
        foreach (var x in ta.As().Value.Reverse())
        {
            if (!predicate((state, x.Value))) return state;
            state = f(state)(x.Value);
        }
        return state;
    }
    
    static int Foldable<HashMap<Key>>.Count<A>(K<HashMap<Key>, A> ta) =>
        ta.As().Count;

    static bool Foldable<HashMap<Key>>.IsEmpty<A>(K<HashMap<Key>, A> ta) =>
        ta.As().IsEmpty;

    static K<HashMap<Key>, A> SemigroupK<HashMap<Key>>.Combine<A>(K<HashMap<Key>, A> lhs, K<HashMap<Key>, A> rhs) =>
        lhs.As() + rhs.As();

    static K<HashMap<Key>, A> MonoidK<HashMap<Key>>.Empty<A>() =>
        HashMap<Key, A>.Empty;
}
