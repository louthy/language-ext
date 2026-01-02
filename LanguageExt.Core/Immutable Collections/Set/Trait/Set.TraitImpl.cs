using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Set : 
    Monad<Set>,
    MonoidK<Set>,
    Alternative<Set>, 
    Traversable<Set>
{
    static K<Set, B> Monad<Set>.Recur<A, B>(A value, Func<A, K<Set, Next<A, B>>> f) =>
        createRange(Monad.enumerableRecur(value, x =>f(x).As().AsEnumerable()));
    
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

    static K<Set, B> Applicative<Set>.Apply<A, B>(K<Set, Func<A, B>> mf, Memo<Set, A> ma)
    {
        return new Set<B>(Go());
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

    static K<Set, A> MonoidK<Set>.Empty<A>() =>
        Set<A>.Empty;

    static K<Set, A> Alternative<Set>.Empty<A>() =>
        Set<A>.Empty;

    static K<Set, A> SemigroupK<Set>.Combine<A>(K<Set, A> ma, K<Set, A> mb) =>
        ma.As() + mb.As();
    
    static K<Set, A> Choice<Set>.Choose<A>(K<Set, A> ma, K<Set, A> mb) => 
        ma.IsEmpty ? mb : ma;

    static K<Set, A> Choice<Set>.Choose<A>(K<Set, A> ma, Memo<Set, A> mb) => 
        ma.IsEmpty ? mb.Value : ma;

    static bool Foldable<Set>.Contains<EqA, A>(A value, K<Set, A> ta) =>
        ta.As().Contains(value);

    static int Foldable<Set>.Count<A>(K<Set, A> ta) =>
        ta.As().Count;

    static bool Foldable<Set>.IsEmpty<A>(K<Set, A> ta) =>
        ta.As().IsEmpty;

    static K<F, K<Set, B>> Traversable<Set>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Set, A> ta) 
    {
        return F.Map<Set<B>, K<Set, B>>(
            ks => ks,
            Foldable.fold(acc, F.Pure(empty<B>()), ta));

        K<F, Set<B>> acc(K<F, Set<B>> ys, A x) =>
            Applicative.lift((bs, b) => bs.Add(b), ys, f(x));
    }

    static K<F, K<Set, B>> Traversable<Set>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Set, A> ta) 
    {
        return F.Map<Set<B>, K<Set, B>>(
            ks => ks,
            Foldable.fold(acc, F.Pure(empty<B>()), ta));

        K<F, Set<B>> acc(K<F, Set<B>> fys, A x) =>
            fys.Bind(ys => f(x).Map(ys.Add));
    }
    
    static Option<A> Foldable<Set>.Head<A>(K<Set, A> ta) =>
        ta.As().Min;

    static Option<A> Foldable<Set>.Last<A>(K<Set, A> ta) =>
        ta.As().Max;

    static Option<A> Foldable<Set>.Min<A>(K<Set, A> ta) =>
        ta.As().Min;

    static Option<A> Foldable<Set>.Max<A>(K<Set, A> ta) =>
        ta.As().Max;
    
    static Fold<A, S> Foldable<Set>.FoldStep<A, S>(K<Set, A> ta, in S initialState) =>
        ta.As().FoldStep(initialState);
        
    static Fold<A, S> Foldable<Set>.FoldStepBack<A, S>(K<Set, A> ta, in S initialState) =>
        ta.As().FoldStepBack(initialState);
}
