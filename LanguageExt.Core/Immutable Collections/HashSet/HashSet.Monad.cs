using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class HashSet : Monad<HashSet>, MonoidK<HashSet>, Traversable<HashSet>
{
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

    static K<HashSet, B> Applicative<HashSet>.Action<A, B>(K<HashSet, A> ma, K<HashSet, B> mb) => 
        mb;

    static K<HashSet, A> MonoidK<HashSet>.Empty<A>() =>
        HashSet<A>.Empty;

    static K<HashSet, A> SemigroupK<HashSet>.Combine<A>(K<HashSet, A> ma, K<HashSet, A> mb) =>
        ma.As() + mb.As();
    
    static bool Foldable<HashSet>.Contains<EqA, A>(A value, K<HashSet, A> ta) =>
        ta.As().Contains(value);

    static int Foldable<HashSet>.Count<A>(K<HashSet, A> ta) =>
        ta.As().Count;

    static bool Foldable<HashSet>.IsEmpty<A>(K<HashSet, A> ta) =>
        ta.As().IsEmpty;

    static S Foldable<HashSet>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<HashSet, A> ta)
    {
        foreach (var x in ta.As())
        {
            if (!predicate((state, x))) return state;
            state = f(x)(state);
        }
        return state;
    }
    
    static S Foldable<HashSet>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state, 
        K<HashSet, A> ta)
    {
        foreach (var x in ta.As().Reverse())
        {
            if (!predicate((state, x))) return state;
            state = f(state)(x);
        }
        return state;
    }    

    static K<F, K<HashSet, B>> Traversable<HashSet>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<HashSet, A> ta) 
    {
        return F.Map<HashSet<B>, K<HashSet, B>>(
            ks => ks,
            Foldable.fold(acc, F.Pure(empty<B>()), ta));

        K<F, HashSet<B>> acc(K<F, HashSet<B>> ys, A x) =>
            Applicative.lift((b, bs) => bs.Add(b), f(x), ys);
    }
}
