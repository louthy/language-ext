using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

public static partial class Applicative
{
    [Pure]
    public static K<F, B> lift<F, A, B>(Func<A, B> f, Memo<F, A> fa)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa);

    [Pure]
    public static K<F, C> lift<F, A, B, C>(Func<A, B, C> f, Memo<F, A> fa, Memo<F, B> fb)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb);

    [Pure]
    public static K<F, C> lift<F, A, B, C>(Func<A, Func<B, C>> f, Memo<F, A> fa, Memo<F, B> fb)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb);

    [Pure]
    public static K<F, D> lift<F, A, B, C, D>(Func<A, B, C, D> f, Memo<F, A> fa, Memo<F, B> fb, Memo<F, C> fc)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb).Apply(fc);

    [Pure]
    public static K<F, D> lift<F, A, B, C, D>(Func<A, Func<B, Func<C, D>>> f, Memo<F, A> fa, Memo<F, B> fb, Memo<F, C> fc)
        where F : Applicative<F> =>
        F.Pure(f).Apply(fa).Apply(fb).Apply(fc);
}
