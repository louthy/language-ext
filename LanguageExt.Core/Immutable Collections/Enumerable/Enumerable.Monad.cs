using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class EnumerableM : Monad<EnumerableM>, Alternative<EnumerableM>, Traversable<EnumerableM>
{
    static K<EnumerableM, B> Monad<EnumerableM>.Bind<A, B>(K<EnumerableM, A> ma, Func<A, K<EnumerableM, B>> f) =>
        ma.As().Bind(f);

    static K<EnumerableM, B> Functor<EnumerableM>.Map<A, B>(Func<A, B> f, K<EnumerableM, A> ma) => 
        ma.As().Map(f);

    static K<EnumerableM, A> Applicative<EnumerableM>.Pure<A>(A value) =>
        singleton(value);

    static K<EnumerableM, B> Applicative<EnumerableM>.Apply<A, B>(K<EnumerableM, Func<A, B>> mf, K<EnumerableM, A> ma) => 
        new EnumerableM<B>(mf.As().Apply(ma.As()));

    static K<EnumerableM, B> Applicative<EnumerableM>.Action<A, B>(K<EnumerableM, A> ma, K<EnumerableM, B> mb) => 
        new EnumerableM<B>(ma.As().Action(mb.As()));

    static K<EnumerableM, A> Alternative<EnumerableM>.Empty<A>() =>
        EnumerableM<A>.Empty;

    static K<EnumerableM, A> Alternative<EnumerableM>.Or<A>(K<EnumerableM, A> ma, K<EnumerableM, A> mb)
    {
        return new EnumerableM<A>(go());
        IEnumerable<A> go()
        {
            bool found = false;
            foreach (var x in ma.As())
            {
                found = true;
                yield return x;
            }

            if (!found)
            {
                foreach (var x in mb.As())
                {
                    yield return x;
                }
            }
        }
    }

    static S Foldable<EnumerableM>.Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<EnumerableM, A> ta) => 
        ta.As().Fold(initialState, (a, s) => f(s)(a));

    static S Foldable<EnumerableM>.FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<EnumerableM, A> ta) => 
        ta.As().FoldBack(initialState, (s, a) => f(s)(a));

    static K<F, K<EnumerableM, B>> Traversable<EnumerableM>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<EnumerableM, A> ta) 
    {
        return F.Map<EnumerableM<B>, K<EnumerableM, B>>(
            ks => ks, 
            F.Map(s => new EnumerableM<B>(s.AsEnumerable()), 
                  Foldable.fold(a => s => cons(a, s), F.Pure(Seq.empty<B>()), ta)));

        K<F, Seq<B>> cons(A x, K<F, Seq<B>> ys) =>
            Applicative.lift((b, bs) => b.Cons(bs), f(x), ys);
    }
}
