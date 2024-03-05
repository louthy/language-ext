using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Fin : Monad<Fin>, Traversable<Fin>, Alternative<Fin>
{
    static K<Fin, B> Monad<Fin>.Bind<A, B>(K<Fin, A> ma, Func<A, K<Fin, B>> f) =>
        ma.As().Bind(f);

    static K<Fin, B> Functor<Fin>.Map<A, B>(Func<A, B> f, K<Fin, A> ma) =>
        ma.As().Map(f);

    static K<Fin, A> Applicative<Fin>.Pure<A>(A value) =>
        Fin<A>.Succ(value);

    static K<Fin, B> Applicative<Fin>.Apply<A, B>(K<Fin, Func<A, B>> mf, K<Fin, A> ma) =>
        mf.As().Apply(ma.As());

    static K<Fin, B> Applicative<Fin>.Action<A, B>(K<Fin, A> ma, K<Fin, B> mb) =>
        ma.As().Action(mb.As());

    static S Foldable<Fin>.Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<Fin, A> ta) =>
        ta.As().Fold(initialState, (s, a) => f(a)(s));

    static S Foldable<Fin>.FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<Fin, A> ta) =>
        ta.As().Fold(initialState, (s, a) => f(s)(a));

    static K<F, K<Fin, B>> Traversable<Fin>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Fin, A> ta) =>
        ta.As().Match(Succ: x => F.Map(ConsSucc, f(x)),
                      Fail: e => F.Pure(ConsFail<B>(e)));

    static K<Fin, A> MonoidK<Fin>.Empty<A>() =>
        Fin<A>.Fail(Errors.None);

    static K<Fin, A> SemigroupK<Fin>.Combine<A>(K<Fin, A> ma, K<Fin, A> mb) =>
        ma.As().IsSucc ? ma : mb;

    static K<Fin, A> ConsSucc<A>(A value) =>
        Fin<A>.Succ(value);

    static K<Fin, A> ConsFail<A>(Error value) =>
        Fin<A>.Fail(value);
}
