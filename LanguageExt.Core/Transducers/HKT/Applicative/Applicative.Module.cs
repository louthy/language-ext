using System;

namespace LanguageExt.HKT;

/// <summary>
/// Monad module
/// </summary>
public static class Applicative
{
    public static Applicative<F, A> pure<F, A>(A value) 
        where F : Applicative<F> =>
        F.Pure(value);

    public static Applicative<F, B> apply<F, A, B>(Applicative<F, Transducer<A, B>> mf, Applicative<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(mf, ma);

    public static Applicative<F, B> apply<F, A, B>(Applicative<F, Func<A, B>> mf, Applicative<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(mf, ma);
    
    public static Applicative<F, B> action<F, A, B>(Applicative<F, A> ma, Applicative<F, B> mb)
        where F : Applicative<F> =>
        F.Action(ma, mb);
}
