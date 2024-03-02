using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Arr : Monad<Arr>, Alternative<Arr>, Traversable<Arr>
{
    static K<Arr, B> Monad<Arr>.Bind<A, B>(K<Arr, A> ma, Func<A, K<Arr, B>> f) =>
        ma.As().Bind(f);

    static K<Arr, B> Functor<Arr>.Map<A, B>(Func<A, B> f, K<Arr, A> ma) => 
        ma.As().Map(f);

    static K<Arr, A> Applicative<Arr>.Pure<A>(A value) =>
        singleton(value);

    static K<Arr, B> Applicative<Arr>.Apply<A, B>(K<Arr, Func<A, B>> mf, K<Arr, A> ma) => 
        mf.As().Apply(ma.As());

    static K<Arr, B> Applicative<Arr>.Action<A, B>(K<Arr, A> ma, K<Arr, B> mb) => 
        ma.As().Action(mb.As());

    static K<Arr, A> Alternative<Arr>.Empty<A>() =>
        Arr<A>.Empty;

    static K<Arr, A> SemiAlternative<Arr>.Or<A>(K<Arr, A> ma, K<Arr, A> mb) => 
        ma.As().IsEmpty ? mb : ma;

    static S Foldable<Arr>.Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<Arr, A> ta) => 
        ta.As().Fold(initialState, (a, s) => f(s)(a));

    static S Foldable<Arr>.FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<Arr, A> ta) => 
        ta.As().FoldBack(initialState, (s, a) => f(s)(a));

    static K<F, K<Arr, B>> Traversable<Arr>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Arr, A> ta) 
    {
        return F.Map<Arr<B>, K<Arr, B>>(
            ks => ks, 
            F.Map(s => s.ToArr(), 
                  Foldable.fold(cons, F.Pure(Seq.empty<B>()), ta)));

        K<F, Seq<B>> cons(A x, K<F, Seq<B>> ys) =>
            Applicative.lift(Prelude.Cons, f(x), ys);
    }
}
