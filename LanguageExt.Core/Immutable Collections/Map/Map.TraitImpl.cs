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
        
    static int Foldable<Map<Key>>.Count<TA, A>(in TA ta) =>
        ta.As().Count;

    static bool Foldable<Map<Key>>.IsEmpty<TA, A>(in TA ta) =>
        ta.As().IsEmpty;

    static Option<A> Foldable<Map<Key>>.Head<TA, A>(in TA ta) =>
        ta.As().Min.Map(kv => kv.Value);

    static Option<A> Foldable<Map<Key>>.Last<TA, A>(in TA ta) =>
        ta.As().Max.Map(kv => kv.Value);

    static Option<A> Foldable<Map<Key>>.Min<TA, A>(in TA ta) =>
        ta.As().Min.Map(kv => kv.Value);

    static Option<A> Foldable<Map<Key>>.Max<TA, A>(in TA ta) =>
        ta.As().Max.Map(kv => kv.Value);

    static K<Map<Key>, A> SemigroupK<Map<Key>>.Combine<A>(K<Map<Key>, A> lhs, K<Map<Key>, A> rhs) =>
        lhs.As() + rhs.As();

    static K<Map<Key>, A> MonoidK<Map<Key>>.Empty<A>() =>
        Map<Key, A>.Empty;
    
    static Fold<A, S> Foldable<Map<Key>>.FoldStep<TA, A, S>(in TA ta, in S initialState) =>
        ta.As().FoldStepValues(initialState);
    
    static Fold<A, S> Foldable<Map<Key>>.FoldStepBack<TA, A, S>(in TA ta, in S initialState) =>
        ta.As().FoldStepBackValues(initialState);

    static void Foldable<Map<Key>, Map.FoldState>.FoldStepInit<TA, A>(
        in TA ta,
        ref Map.FoldState refState) =>
        Map.FoldState.Push(ref refState, ta.As().Value.Root);

    static void Foldable<Map<Key>, Map.FoldState>.FoldStepBackInit<TA, A>(
        in TA ta, 
        ref Map.FoldState refState) =>
        Map.FoldState.Push(ref refState, ta.As().Value.Root);

    static bool Foldable<Map<Key>, Map.FoldState>.FoldStep<TA, A>(
        in TA ta, 
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

    static bool Foldable<Map<Key>, Map.FoldState>.FoldStepBack<TA, A>(
        in TA ta, 
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
