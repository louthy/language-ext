using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Set : Monad<Set>, MonoidK<Set>, Traversable<Set>
{
    static K<Set, B> Monad<Set>.Bind<A, B>(K<Set, A> ma, Func<A, K<Set, B>> f)
    {
        return new Set<B>(Go());
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

    static K<Set, B> Functor<Set>.Map<A, B>(Func<A, B> f, K<Set, A> ma)
    {
        return new Set<B>(Go());
        IEnumerable<B> Go()
        {
            foreach (var x in ma.As())
            {
                yield return f(x);
            }
        }
    }

    static K<Set, A> Applicative<Set>.Pure<A>(A value) =>
        singleton(value);

    static K<Set, B> Applicative<Set>.Apply<A, B>(K<Set, Func<A, B>> mf, K<Set, A> ma)
    {
        return new Set<B>(Go());
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

    static K<Set, B> Applicative<Set>.Action<A, B>(K<Set, A> ma, K<Set, B> mb) => 
        mb;

    static K<Set, A> MonoidK<Set>.Empty<A>() =>
        Set<A>.Empty;

    static K<Set, A> SemigroupK<Set>.Combine<A>(K<Set, A> ma, K<Set, A> mb) =>
        ma.As() + mb.As();
    
    static bool Foldable<Set>.Contains<EqA, A>(A value, K<Set, A> ta) =>
        ta.As().Contains(value);

    static int Foldable<Set>.Count<A>(K<Set, A> ta) =>
        ta.As().Count;

    static bool Foldable<Set>.IsEmpty<A>(K<Set, A> ta) =>
        ta.As().IsEmpty;

    static S Foldable<Set>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Set, A> ta)
    {
        foreach (var x in ta.As())
        {
            if (!predicate((state, x))) return state;
            state = f(x)(state);
        }
        return state;
    }
    
    static S Foldable<Set>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state, 
        K<Set, A> ta)
    {
        foreach (var x in ta.As().Reverse())
        {
            if (!predicate((state, x))) return state;
            state = f(state)(x);
        }
        return state;
    }    

    static K<F, K<Set, B>> Traversable<Set>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Set, A> ta) 
    {
        return F.Map<Set<B>, K<Set, B>>(
            ks => ks,
            Foldable.fold(acc, F.Pure(empty<B>()), ta));

        K<F, Set<B>> acc(K<F, Set<B>> ys, A x) =>
            Applicative.lift((b, bs) => bs.Add(b), f(x), ys);
    }
    
    static Option<A> Foldable<Set>.Head<A>(K<Set, A> ta) =>
        ta.As().Min;

    static Option<A> Foldable<Set>.Last<A>(K<Set, A> ta) =>
        ta.As().Max;

    static Option<A> Foldable<Set>.Min<A>(K<Set, A> ta) =>
        ta.As().Min;

    static Option<A> Foldable<Set>.Max<A>(K<Set, A> ta) =>
        ta.As().Max;
}
