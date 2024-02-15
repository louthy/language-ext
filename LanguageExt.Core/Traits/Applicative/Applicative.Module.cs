﻿using System;

namespace LanguageExt.Traits;

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

    public static K<F, D> apply<F, A, B, C, D>(K<F, Func<A, B, C, D>> mf, K<F, A> ma, K<F, B> mb, K<F, C> mc)
        where F : Applicative<F> =>
        F.Apply(F.Apply(F.Map(Prelude.curry, mf), ma, mb), mc);

    public static K<F, Func<C, D>> apply<F, A, B, C, D>(K<F, Func<A, B, C, D>> mf, K<F, A> ma, K<F, B> mb)
        where F : Applicative<F> =>
        F.Apply(F.Map(Prelude.curry, mf), ma, mb);

    public static K<F, Func<B,Func<C, D>>> apply<F, A, B, C, D>(K<F, Func<A, B, C, D>> mf, K<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(F.Map(Prelude.curry, mf), ma);
    
    public static K<F, B> action<F, A, B>(K<F, A> ma, K<F, B> mb)
        where F : Applicative<F> =>
        F.Action(ma, mb);

    public static K<F, Unit> when<F>(bool flag, K<F, Unit> fx)
        where F : Applicative<F> =>
        flag ? fx : F.Pure<Unit>(default);

    public static K<F, Unit> unless<F>(bool flag, K<F, Unit> fx)
        where F : Applicative<F> =>
        when(!flag, fx);

    public static K<F, B> lift<F, A, B>(Func<A, B> f, K<F, A> fa)
        where F : Applicative<F> =>
        apply(F.Pure(f), fa);

    public static K<F, C> lift<F, A, B, C>(Func<A, B, C> f, K<F, A> fa, K<F, B> fb)
        where F : Applicative<F> =>
        apply(F.Pure(f), fa, fb);

    public static K<F, C> lift<F, A, B, C>(Func<A, Func<B, C>> f, K<F, A> fa, K<F, B> fb)
        where F : Applicative<F> =>
        apply(apply(F.Pure(f), fa), fb);

    public static K<F, D> lift<F, A, B, C, D>(Func<A, B, C, D> f, K<F, A> fa, K<F, B> fb, K<F, C> fc)
        where F : Applicative<F> =>
        apply(F.Pure(f), fa, fb, fc);

    public static K<F, D> lift<F, A, B, C, D>(Func<A, Func<B, Func<C, D>>> f, K<F, A> fa, K<F, B> fb, K<F, C> fc)
        where F : Applicative<F> =>
        apply(apply(apply(F.Pure(f), fa), fb), fc);
}