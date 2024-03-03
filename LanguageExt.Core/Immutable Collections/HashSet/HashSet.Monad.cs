using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class HashSet : Monad<HashSet>, Alternative<HashSet>, Traversable<HashSet>
{
    static K<HashSet, B> Monad<HashSet>.Bind<A, B>(K<HashSet, A> ma, Func<A, K<HashSet, B>> f) =>
        ma.As().Bind(f);

    static K<HashSet, B> Functor<HashSet>.Map<A, B>(Func<A, B> f, K<HashSet, A> ma) => 
        ma.As().Map(f);

    static K<HashSet, A> Applicative<HashSet>.Pure<A>(A value) =>
        singleton(value);

    static K<HashSet, B> Applicative<HashSet>.Apply<A, B>(K<HashSet, Func<A, B>> mf, K<HashSet, A> ma) => 
        mf.As().Apply(ma.As());

    static K<HashSet, B> Applicative<HashSet>.Action<A, B>(K<HashSet, A> ma, K<HashSet, B> mb) => 
        ma.As().Action(mb.As());

    static K<HashSet, A> Alternative<HashSet>.Empty<A>() =>
        HashSet<A>.Empty;

    static K<HashSet, A> SemiAlternative<HashSet>.Or<A>(K<HashSet, A> ma, K<HashSet, A> mb) => 
        ma.As().IsEmpty ? mb : ma;

    static S Foldable<HashSet>.Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<HashSet, A> ta) => 
        ta.As().Fold(initialState, (a, s) => f(s)(a));

    static S Foldable<HashSet>.FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<HashSet, A> ta) => 
        ta.As().FoldBack(initialState, (s, a) => f(s)(a));

    static K<F, K<HashSet, B>> Traversable<HashSet>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<HashSet, A> ta) 
    {
        return F.Map<HashSet<B>, K<HashSet, B>>(
            ks => ks, 
            Foldable.fold(add, F.Pure(empty<B>()), ta));

        K<F, HashSet<B>> add(A x, K<F, HashSet<B>> ys) =>
            Applicative.lift((b, bs) => bs.Add(b), f(x), ys);
    }
}
