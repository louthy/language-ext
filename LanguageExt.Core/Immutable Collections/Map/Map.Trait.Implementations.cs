using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Map<Key> : Foldable<Map<Key>>, Functor<Map<Key>>, MonoidK<Map<Key>>
{
    static K<Map<Key>, B> Functor<Map<Key>>.Map<A, B>(Func<A, B> f, K<Map<Key>, A> ma)
    {
        return new Map<Key, B>(Go());
        IEnumerable<(Key, B)> Go()
        {
            foreach (var x in ma.As())
            {
                yield return (x.Key, f(x.Value));
            }
        }
    }
    
    static S Foldable<Map<Key>>.FoldWhile<A, S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S state, K<Map<Key>, A> ta)
    {
        foreach (var x in ta.As())
        {
            if (!predicate((state, x.Value))) return state;
            state = f(x.Value)(state);
        }
        return state;
    }
    
    static S Foldable<Map<Key>>.FoldBackWhile<A, S>(Func<S, Func<A, S>> f, Func<(S State, A Value), bool> predicate, S state, K<Map<Key>, A> ta)
    {
        foreach (var x in ta.As().Value.Reverse())
        {
            if (!predicate((state, x.Value))) return state;
            state = f(state)(x.Value);
        }
        return state;
    }
    
        
    static int Foldable<Map<Key>>.Count<A>(K<Map<Key>, A> ta) =>
        ta.As().Count;

    static bool Foldable<Map<Key>>.IsEmpty<A>(K<Map<Key>, A> ta) =>
        ta.As().IsEmpty;

    static Option<A> Foldable<Map<Key>>.Head<A>(K<Map<Key>, A> ta) =>
        ta.As().Min().Map(kv => kv.Value);

    static Option<A> Foldable<Map<Key>>.Last<A>(K<Map<Key>, A> ta) =>
        ta.As().Max().Map(kv => kv.Value);

    static Option<A> Foldable<Map<Key>>.Min<A>(K<Map<Key>, A> ta) =>
        ta.As().Min().Map(kv => kv.Value);

    static Option<A> Foldable<Map<Key>>.Max<A>(K<Map<Key>, A> ta) =>
        ta.As().Max().Map(kv => kv.Value);

    static K<Map<Key>, A> SemigroupK<Map<Key>>.Combine<A>(K<Map<Key>, A> lhs, K<Map<Key>, A> rhs) =>
        lhs.As() + rhs.As();

    static K<Map<Key>, A> MonoidK<Map<Key>>.Empty<A>() =>
        Map<Key, A>.Empty;
}
