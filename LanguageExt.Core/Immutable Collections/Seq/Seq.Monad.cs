using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Seq : Monad<Seq>, Alternative<Seq>, Traversable<Seq>
{
    static K<Seq, B> Monad<Seq>.Bind<A, B>(K<Seq, A> ma, Func<A, K<Seq, B>> f) =>
        ma.As().Bind(f);

    static K<Seq, B> Functor<Seq>.Map<A, B>(Func<A, B> f, K<Seq, A> ma) => 
        ma.As().Map(f);

    static K<Seq, A> Applicative<Seq>.Pure<A>(A value) =>
        singleton(value);

    static K<Seq, B> Applicative<Seq>.Apply<A, B>(K<Seq, Func<A, B>> mf, K<Seq, A> ma) => 
        mf.As().Apply(ma.As());

    static K<Seq, B> Applicative<Seq>.Action<A, B>(K<Seq, A> ma, K<Seq, B> mb) => 
        ma.As().Action(mb.As());

    static K<Seq, A> MonoidK<Seq>.Empty<A>() =>
        Seq<A>.Empty;

    static K<Seq, A> SemigroupK<Seq>.Combine<A>(K<Seq, A> ma, K<Seq, A> mb) => 
        ma.As().IsEmpty ? mb : ma;

    static S Foldable<Seq>.Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<Seq, A> ta) => 
        ta.As().Fold(initialState, (a, s) => f(s)(a));

    static S Foldable<Seq>.FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<Seq, A> ta) => 
        ta.As().FoldBack(initialState, (s, a) => f(s)(a));

    static K<F, K<Seq, B>> Traversable<Seq>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Seq, A> ta) 
    {
        return F.Map<Seq<B>, K<Seq, B>>(
            ks => ks, 
            Foldable.foldBack(cons, F.Pure(empty<B>()), ta));

        K<F, Seq<B>> cons(K<F, Seq<B>> ys, A x) =>
            Applicative.lift(Prelude.Cons, f(x), ys);
    }
}
