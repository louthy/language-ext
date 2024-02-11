using System;
using System.Linq;
using LanguageExt.HKT;

namespace LanguageExt;

public class Lst : Monad<Lst>, Alternative<Lst>, Traversable<Lst>
{
    static K<Lst, B> Monad<Lst>.Bind<A, B>(K<Lst, A> ma, Func<A, K<Lst, B>> f) =>
        ma.As().Bind(f);

    static K<Lst, B> Functor<Lst>.Map<A, B>(Func<A, B> f, K<Lst, A> ma) => 
        ma.As().Map(f);

    static K<Lst, A> Applicative<Lst>.Pure<A>(A value) =>
        List.singleton(value);

    static K<Lst, B> Applicative<Lst>.Apply<A, B>(K<Lst, Func<A, B>> mf, K<Lst, A> ma) => 
        mf.As().Apply(ma.As()).Freeze();

    static K<Lst, B> Applicative<Lst>.Action<A, B>(K<Lst, A> ma, K<Lst, B> mb) => 
        ma.As().Action(mb.As()).Freeze();

    static K<Lst, A> Alternative<Lst>.Empty<A>() =>
        Lst<A>.Empty;

    static K<Lst, A> Alternative<Lst>.Or<A>(K<Lst, A> ma, K<Lst, A> mb) => 
        ma.As().IsEmpty ? mb : ma;

    static S Foldable<Lst>.Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<Lst, A> ta) => 
        ta.As().Fold(initialState, (a, s) => f(s)(a));

    static S Foldable<Lst>.FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<Lst, A> ta) => 
        ta.As().FoldBack(initialState, (s, a) => f(s)(a));

    static K<F, K<Lst, B>> Traversable<Lst>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Lst, A> ta) 
    {
        return F.Map<Lst<B>, K<Lst, B>>(
            ks => ks.Kind, 
            Foldable.fold(a => s => cons(a, s), F.Pure(List.empty<B>()), ta));

        K<F, Lst<B>> cons(A x, K<F, Lst<B>> ys) =>
            Applicative.lift((b, bs) => b.Cons(bs), f(x), ys);
    }
}
