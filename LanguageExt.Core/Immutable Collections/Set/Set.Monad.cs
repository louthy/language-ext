using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Set : Monad<Set>, Alternative<Set>, Traversable<Set>
{
    static K<Set, B> Monad<Set>.Bind<A, B>(K<Set, A> ma, Func<A, K<Set, B>> f) =>
        ma.As().Bind(f);

    static K<Set, B> Functor<Set>.Map<A, B>(Func<A, B> f, K<Set, A> ma) => 
        ma.As().Map(f);

    static K<Set, A> Applicative<Set>.Pure<A>(A value) =>
        singleton(value);

    static K<Set, B> Applicative<Set>.Apply<A, B>(K<Set, Func<A, B>> mf, K<Set, A> ma) => 
        mf.As().Apply(ma.As());

    static K<Set, B> Applicative<Set>.Action<A, B>(K<Set, A> ma, K<Set, B> mb) => 
        ma.As().Action(mb.As());

    static K<Set, A> Alternative<Set>.Empty<A>() =>
        Set<A>.Empty;

    static K<Set, A> SemiAlternative<Set>.Or<A>(K<Set, A> ma, K<Set, A> mb) => 
        ma.As().IsEmpty ? mb : ma;

    static S Foldable<Set>.Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<Set, A> ta) => 
        ta.As().Fold(initialState, (a, s) => f(s)(a));

    static S Foldable<Set>.FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<Set, A> ta) => 
        ta.As().FoldBack(initialState, (s, a) => f(s)(a));

    static K<F, K<Set, B>> Traversable<Set>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Set, A> ta) 
    {
        return F.Map<Set<B>, K<Set, B>>(
            ks => ks, 
            Foldable.fold(a => s => cons(a, s), F.Pure(empty<B>()), ta));

        K<F, Set<B>> cons(A x, K<F, Set<B>> ys) =>
            Applicative.lift((b, bs) => bs.Add(b), f(x), ys);
    }
}
