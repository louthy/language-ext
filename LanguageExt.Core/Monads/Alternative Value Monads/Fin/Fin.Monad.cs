using System;
using LanguageExt.Common;
using LanguageExt.HKT;

namespace LanguageExt;

public class Fin : Monad<Fin>, Traversable<Fin>, Alternative<Fin>
{
    public static K<Fin, B> Bind<A, B>(K<Fin, A> ma, Func<A, K<Fin, B>> f) =>
        ma.As().Bind(f);

    public static K<Fin, B> Map<A, B>(Func<A, B> f, K<Fin, A> ma) =>
        ma.As().Map(f);

    public static K<Fin, A> Pure<A>(A value) =>
        Fin<A>.Succ(value);

    public static K<Fin, B> Apply<A, B>(K<Fin, Func<A, B>> mf, K<Fin, A> ma) =>
        mf.As().Apply(ma.As());

    public static K<Fin, B> Action<A, B>(K<Fin, A> ma, K<Fin, B> mb) =>
        ma.As().Action(mb.As());

    public static S Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<Fin, A> ta) =>
        ta.As().Fold(initialState, (s, a) => f(a)(s));

    public static S FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<Fin, A> ta) =>
        ta.As().Fold(initialState, (s, a) => f(s)(a));

    public static K<F, K<Fin, B>> Traverse<F, A, B>(Func<A, K<F, B>> f, K<Fin, A> ta)
        where F : Applicative<F> =>
        ta.As().Match(Succ: x => F.Map(Succ, f(x)),
                      Fail: e => F.Pure(Fail<B>(e)));

    public static K<Fin, A> Empty<A>() =>
        Fin<A>.Fail(Errors.None);

    public static K<Fin, A> Or<A>(K<Fin, A> ma, K<Fin, A> mb) =>
        ma.As().IsSucc ? ma : mb;

    static K<Fin, A> Succ<A>(A value) =>
        Fin<A>.Succ(value);

    static K<Fin, A> Fail<A>(Error value) =>
        Fin<A>.Fail(value);
}
