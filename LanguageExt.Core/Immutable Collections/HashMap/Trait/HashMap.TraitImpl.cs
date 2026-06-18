using System;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class HashMap<Key> : 
    Foldable<HashMap<Key>, TrieMap.FoldState>,
    Indexable<HashMap<Key>, Key>,
    Functor<HashMap<Key>>, 
    MonoidK<HashMap<Key>>,
    Countable<HashMap<Key>>
{
    static K<HashMap<Key>, B> Functor<HashMap<Key>>.Map<A, B>(Func<A, B> f, K<HashMap<Key>, A> ma) =>
        ma.As().Map(f);
    
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

    static Option<A> Indexable<HashMap<Key>, Key>.At<A>(Key index, K<HashMap<Key>, A> ta) => 
        ta.As().Value.Find(index);
    
    /// <summary>
    /// Sort the items in the foldable structure in the order dictated by the ordering function
    /// </summary>
    /// <param name="comparer">Ordering function</param>
    /// <param name="ta">Foldable structure</param>
    /// <returns>An array of sorted values</returns>
    static Arr<A> Foldable<HashMap<Key>>.Sort<A>(Comparison<A> comparer, K<HashMap<Key>, A> ta)
    {
        var ln = ta.As().Count;
        if (ln <= 0) return Arr<A>.Empty;
        if (ln >= int.MaxValue) throw new ArgumentException("HashMap: Foldable.Sort: structure too large");
        
        var ys = ArrayWriterRef<A>.Init(ln);
        var fs = IterableK.stepSetup<HashMap<Key>, TrieMap.FoldState, A>(ta);
        while (IterableK.step(ta, ref fs, out var x))
        {
            ys.Add(x);
        }
        ys.MutableView.Sort(comparer);
        return ys.ToArr();
    }
    
    /// <summary>
    /// Sort the items in the foldable structure in the order dictated by the ordering function using the key selector.
    /// </summary>
    /// <param name="key">Key selector function</param>
    /// <param name="comparer">Ordering function</param>
    /// <param name="ta">Foldable structure</param>
    /// <returns>An array of sorted values</returns>
    static Arr<A> Foldable<HashMap<Key>>.Sort<A, K>(Func<A, K> key, Comparison<K> comparer, K<HashMap<Key>, A> ta)
    {
        var ln = ta.As().Count;
        if (ln <= 0) return Arr<A>.Empty;
        if (ln >= int.MaxValue) throw new ArgumentException("HashMap: Foldable.Sort: structure too large");
        
        var ks = ArrayWriterRef<K>.Init(ln);
        var ys = ArrayWriterRef<A>.Init(ln);
        var fs = IterableK.stepSetup<HashMap<Key>, TrieMap.FoldState, A>(ta);
        while (IterableK.step(ta, ref fs, out var x))
        {
            ks.Add(key(x));
            ys.Add(x);
        }
        ks.MutableView.Sort(ys.MutableView, comparer);
        return ys.ToArr();
    }

    static long Countable<HashMap<Key>>.Count<A>(K<HashMap<Key>, A> fa) => 
        fa.As().Count;
}
