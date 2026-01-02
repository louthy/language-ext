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
    
    static int Foldable<HashMap<Key>>.Count<A>(K<HashMap<Key>, A> ta) =>
        ta.As().Count;

    static bool Foldable<HashMap<Key>>.IsEmpty<A>(K<HashMap<Key>, A> ta) =>
        ta.As().IsEmpty;

    static K<HashMap<Key>, A> SemigroupK<HashMap<Key>>.Combine<A>(K<HashMap<Key>, A> lhs, K<HashMap<Key>, A> rhs) =>
        lhs.As() + rhs.As();

    static K<HashMap<Key>, A> MonoidK<HashMap<Key>>.Empty<A>() =>
        HashMap<Key, A>.Empty;
    
    static Fold<A, S> Foldable<HashMap<Key>>.FoldStep<A, S>(K<HashMap<Key>, A> ta, in S initialState)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        var iter = ta.As().Values.GetEnumerator();
        return go(initialState);
        Fold<A, S> go(S state) =>
            iter.MoveNext()
                ? Fold.Loop(state, iter.Current, go)
                : Fold.Done<A, S>(state);
    }      
        
    static Fold<A, S> Foldable<HashMap<Key>>.FoldStepBack<A, S>(K<HashMap<Key>, A> ta, in S initialState) =>
        // Order is undefined in a HashMap, so reversing the order makes no sense,
        // so let's take the most efficient option:
        ta.FoldStep(initialState);
}
