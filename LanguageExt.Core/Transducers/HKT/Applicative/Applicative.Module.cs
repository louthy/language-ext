using System;

namespace LanguageExt.HKT;

/// <summary>
/// Monad module
/// </summary>
public static class Applicative
{
    public static K<F, A> pure<F, A>(A value) 
        where F : Applicative<F> =>
        F.Pure(value);

    public static K<F, B> apply<F, A, B>(K<F, Func<A, B>> mf, K<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(mf, ma);

    public static K<F, Func<B, C>> apply<F, A, B, C>(K<F, Func<A, B, C>> mf, K<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(F.Map(Prelude.curry, mf), ma);

    public static K<F, C> apply<F, A, B, C>(K<F, Func<A, B, C>> mf, K<F, A> ma, K<F, B> mb)
        where F : Applicative<F> =>
        F.Apply(F.Apply(F.Map(Prelude.curry, mf), ma), mb);
    
    public static K<F, B> action<F, A, B>(K<F, A> ma, K<F, B> mb)
        where F : Applicative<F> =>
        F.Action(ma, mb);
}
