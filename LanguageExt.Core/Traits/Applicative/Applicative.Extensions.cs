using System;

namespace LanguageExt.Traits;

/// <summary>
/// Monad module
/// </summary>
public static class ApplicativeExtensions
{
    public static K<F, B> Apply<F, A, B>(this K<F, Func<A, B>> mf, K<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(mf, ma);

    public static K<F, Func<B, C>> Apply<F, A, B, C>(this K<F, Func<A, B, C>> mf, K<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(F.Map(Prelude.curry, mf), ma);

    public static K<F, C> Apply<F, A, B, C>(this K<F, Func<A, B, C>> mf, K<F, A> ma, K<F, B> mb)
        where F : Applicative<F> =>
        F.Apply(F.Apply(F.Map(Prelude.curry, mf), ma), mb);

    public static K<F, D> Apply<F, A, B, C, D>(this K<F, Func<A, B, C, D>> mf, K<F, A> ma, K<F, B> mb, K<F, C> mc)
        where F : Applicative<F> =>
        F.Apply(F.Apply(F.Map(Prelude.curry, mf), ma, mb), mc);

    public static K<F, Func<C, D>> Apply<F, A, B, C, D>(this K<F, Func<A, B, C, D>> mf, K<F, A> ma, K<F, B> mb)
        where F : Applicative<F> =>
        F.Apply(F.Map(Prelude.curry, mf), ma, mb);

    public static K<F, Func<B,Func<C, D>>> Apply<F, A, B, C, D>(this K<F, Func<A, B, C, D>> mf, K<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(F.Map(Prelude.curry, mf), ma);
    
    public static K<F, B> Action<F, A, B>(this K<F, A> ma, K<F, B> mb)
        where F : Applicative<F> =>
        F.Action(ma, mb);

    public static K<F, B> Lift<F, A, B>(this Func<A, B> f, K<F, A> fa)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa);

    public static K<F, C> Lift<F, A, B, C>(this Func<A, B, C> f, K<F, A> fa, K<F, B> fb)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb);

    public static K<F, C> Lift<F, A, B, C>(this Func<A, Func<B, C>> f, K<F, A> fa, K<F, B> fb)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb);

    public static K<F, D> Lift<F, A, B, C, D>(this Func<A, B, C, D> f, K<F, A> fa, K<F, B> fb, K<F, C> fc)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb).Apply(fc);

    public static K<F, D> Lift<F, A, B, C, D>(this Func<A, Func<B, Func<C, D>>> f, K<F, A> fa, K<F, B> fb, K<F, C> fc)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb).Apply(fc);
}
