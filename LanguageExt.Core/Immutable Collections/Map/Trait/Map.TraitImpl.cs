using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Map<Key> : Foldable<Map<Key>, Map.FoldState>, Functor<Map<Key>>, MonoidK<Map<Key>>
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
        
    static int Foldable<Map<Key>>.Count<A>(K<Map<Key>, A> ta) =>
        ta.As().Count;

    static bool Foldable<Map<Key>>.IsEmpty<A>(K<Map<Key>, A> ta) =>
        ta.As().IsEmpty;

    static Option<A> Foldable<Map<Key>>.Head<A>(K<Map<Key>, A> ta) =>
        ta.As().Min.Map(kv => kv.Value);

    static Option<A> Foldable<Map<Key>>.Last<A>(K<Map<Key>, A> ta) =>
        ta.As().Max.Map(kv => kv.Value);

    static Option<A> Foldable<Map<Key>>.Min<A>(K<Map<Key>, A> ta) =>
        ta.As().Min.Map(kv => kv.Value);

    static Option<A> Foldable<Map<Key>>.Max<A>(K<Map<Key>, A> ta) =>
        ta.As().Max.Map(kv => kv.Value);

    static K<Map<Key>, A> SemigroupK<Map<Key>>.Combine<A>(K<Map<Key>, A> lhs, K<Map<Key>, A> rhs) =>
        lhs.As() + rhs.As();

    static K<Map<Key>, A> MonoidK<Map<Key>>.Empty<A>() =>
        Map<Key, A>.Empty;
    
    static Fold<A, S> Foldable<Map<Key>>.FoldStep<A, S>(K<Map<Key>, A> ta, in S initialState)
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
    
    static Fold<A, S> Foldable<Map<Key>>.FoldStepBack<A, S>(K<Map<Key>, A> ta, in S initialState) 
    {
        var items = ta.As();
        return go(items.Values.Reverse().GetIterator())(initialState);

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

    static void Foldable<Map<Key>, Map.FoldState>.FoldStepSetup<A>(
        K<Map<Key>, A> ta,
        ref Map.FoldState refState) =>
        Map.FoldState.Setup(ref refState, ta.As().Value.Root);

    static void Foldable<Map<Key>, Map.FoldState>.FoldStepBackSetup<A>(
        K<Map<Key>, A> ta, 
        ref Map.FoldState refState) =>
        Map.FoldState.Setup(ref refState, ta.As().Value.Root);

    static bool Foldable<Map<Key>, Map.FoldState>.FoldStep<A>(
        K<Map<Key>, A> ta, 
        ref Map.FoldState refState, 
        out A value) 
    {
        if (Map.FoldState.Step<Key, A>(ref refState, out var node))
        {
            value = node.KeyValue.Value;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    static bool Foldable<Map<Key>, Map.FoldState>.FoldStepBack<A>(
        K<Map<Key>, A> ta, 
        ref Map.FoldState refState, 
        out A value)  
    {
        if (Map.FoldState.StepBack<Key, A>(ref refState, out var node))
        {
            value = node.KeyValue.Value;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

}
