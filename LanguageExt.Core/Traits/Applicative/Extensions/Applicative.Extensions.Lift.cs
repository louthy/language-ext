using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ApplicativeExtensions
{
    [Pure]
    public static K<F, B> Lift<F, A, B>(this Func<A, B> f, K<F, A> fa)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa);

    [Pure]
    public static K<F, C> Lift<F, A, B, C>(this Func<A, B, C> f, K<F, A> fa, K<F, B> fb)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb);

    [Pure]
    public static K<F, C> Lift<F, A, B, C>(this Func<A, Func<B, C>> f, K<F, A> fa, K<F, B> fb)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb);

    [Pure]
    public static K<F, D> Lift<F, A, B, C, D>(this Func<A, B, C, D> f, K<F, A> fa, K<F, B> fb, K<F, C> fc)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb).Apply(fc);

    [Pure]
    public static K<F, D> Lift<F, A, B, C, D>(this Func<A, Func<B, Func<C, D>>> f, K<F, A> fa, K<F, B> fb, K<F, C> fc)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb).Apply(fc);
}
