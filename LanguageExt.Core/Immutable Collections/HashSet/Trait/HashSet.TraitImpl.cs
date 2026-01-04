using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class HashSet : 
    Monad<HashSet>, 
    MonoidK<HashSet>,
    Alternative<HashSet>, 
    Traversable<HashSet>,
    Foldable<HashSet, TrieSet.FoldState> 
{
    static K<HashSet, B> Monad<HashSet>.Recur<A, B>(A value, Func<A, K<HashSet, Next<A, B>>> f) =>
        createRange(Monad.enumerableRecur(value, x =>f(x).As().AsEnumerable()));
    
    static K<HashSet, B> Monad<HashSet>.Bind<A, B>(K<HashSet, A> ma, Func<A, K<HashSet, B>> f)
    {
        return new HashSet<B>(Go());
        IEnumerable<B> Go()
        {
            foreach (var x in ma.As())
            {
                foreach (var y in f(x).As())
                {
                    yield return y;
                }
            }
        }
    }

    static K<HashSet, B> Functor<HashSet>.Map<A, B>(Func<A, B> f, K<HashSet, A> ma)
    {
        return new HashSet<B>(Go());
        IEnumerable<B> Go()
        {
            foreach (var x in ma.As())
            {
                yield return f(x);
            }
        }
    }

    static K<HashSet, A> Applicative<HashSet>.Pure<A>(A value) =>
        singleton(value);

    static K<HashSet, B> Applicative<HashSet>.Apply<A, B>(K<HashSet, Func<A, B>> mf, K<HashSet, A> ma)
    {
        var ff = mf.As();
        if(ff.IsEmpty) return HashSet<B>.Empty;
        
        return new HashSet<B>(Go());
        IEnumerable<B> Go()
        {
            foreach (var f in mf.As())
            {
                foreach (var a in ma.As())
                {
                    yield return f(a);
                }
            }
        }
    }    

    static K<HashSet, B> Applicative<HashSet>.Apply<A, B>(K<HashSet, Func<A, B>> mf, Memo<HashSet, A> ma)
    {
        var ff = mf.As();
        if(ff.IsEmpty) return HashSet<B>.Empty;
        return new HashSet<B>(Go());
        IEnumerable<B> Go()
        {
            foreach (var f in mf.As())
            {
                foreach (var a in ma.Value.As())
                {
                    yield return f(a);
                }
            }
        }
    }      

    static K<HashSet, A> MonoidK<HashSet>.Empty<A>() =>
        HashSet<A>.Empty;

    static K<HashSet, A> Alternative<HashSet>.Empty<A>() =>
        HashSet<A>.Empty;

    static K<HashSet, A> SemigroupK<HashSet>.Combine<A>(K<HashSet, A> ma, K<HashSet, A> mb) =>
        ma.As() + mb.As();
    
    static K<HashSet, A> Choice<HashSet>.Choose<A>(K<HashSet, A> ma, K<HashSet, A> mb) => 
        ma.IsEmpty ? mb : ma;
    
    static K<HashSet, A> Choice<HashSet>.Choose<A>(K<HashSet, A> ma, Memo<HashSet, A> mb) => 
        ma.IsEmpty ? mb.Value : ma;
    
    static bool Foldable<HashSet>.Contains<EqA, A>(A value, K<HashSet, A> ta) =>
        ta.As().Contains(value);

    static int Foldable<HashSet>.Count<A>(K<HashSet, A> ta) =>
        ta.As().Count;

    static void Foldable<HashSet, TrieSet.FoldState>.FoldStepSetup<A>(K<HashSet, A> ta, ref TrieSet.FoldState refState) => 
        TrieSet.FoldState.Setup(ref refState, ta.As().Value.Root);

    static bool Foldable<HashSet, TrieSet.FoldState>.FoldStep<A>(K<HashSet, A> ta, ref TrieSet.FoldState refState, out A value) =>
        TrieSet.FoldState.Step<EqDefault<A>, A>(ref refState, out value);
    
    static void Foldable<HashSet, TrieSet.FoldState>.FoldStepBackSetup<A>(K<HashSet, A> ta, ref TrieSet.FoldState refState) => 
        TrieSet.FoldState.Setup(ref refState, ta.As().Value.Root);

    static bool Foldable<HashSet, TrieSet.FoldState>.FoldStepBack<A>(K<HashSet, A> ta, ref TrieSet.FoldState refState, out A value) => 
        TrieSet.FoldState.Step<EqDefault<A>, A>(ref refState, out value);

    static bool Foldable<HashSet>.IsEmpty<A>(K<HashSet, A> ta) =>
        ta.As().IsEmpty;

    static K<F, K<HashSet, B>> Traversable<HashSet>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<HashSet, A> ta) 
    {
        return F.Map<HashSet<B>, K<HashSet, B>>(
            ks => ks,
            Foldable.fold(acc, F.Pure(empty<B>()), ta));

        K<F, HashSet<B>> acc(K<F, HashSet<B>> ys, A x) =>
            Applicative.lift((bs, b) => bs.Add(b), ys, f(x));
    }

    static K<F, K<HashSet, B>> Traversable<HashSet>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<HashSet, A> ta) 
    {
        return F.Map<HashSet<B>, K<HashSet, B>>(
            ks => ks,
            Foldable.fold(acc, F.Pure(empty<B>()), ta));

        K<F, HashSet<B>> acc(K<F, HashSet<B>> fys, A x) =>
            fys.Bind(ys => f(x).Map(ys.Add));
    }
    
    static Fold<A, S> Foldable<HashSet>.FoldStep<A, S>(K<HashSet, A> ta, in S initialState)
    {
        var items = ta.As();
        return go(items.GetIterator())(initialState);

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

    static Fold<A, S> Foldable<HashSet>.FoldStepBack<A, S>(K<HashSet, A> ta, in S initialState)
    {
        // Order is undefined in a HashSet, so reversing the order makes no sense,
        // so let's take the most efficient option:
        var items = ta.As();
        return go(items.GetIterator())(initialState);

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
