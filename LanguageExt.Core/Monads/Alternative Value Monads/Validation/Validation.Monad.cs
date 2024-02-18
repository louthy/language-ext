using System;
using LanguageExt.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `Validation` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class Validation<L> : 
    Monad<Validation<L>>, 
    Alternative<Validation<L>>
    where L : Monoid<L>
{
    static K<Validation<L>, B> Monad<Validation<L>>.Bind<A, B>(
        K<Validation<L>, A> ma, 
        Func<A, K<Validation<L>, B>> f) => 
        ma.As().Bind(f);

    static K<Validation<L>, B> Functor<Validation<L>>.Map<A, B>(
        Func<A, B> f, 
        K<Validation<L>, A> ma) => 
        ma.As().Map(f);

    static K<Validation<L>, A> Applicative<Validation<L>>.Pure<A>(A value) => 
        Validation<L, A>.Success(value);

    static K<Validation<L>, B> Applicative<Validation<L>>.Apply<A, B>(
        K<Validation<L>, Func<A, B>> mf,
        K<Validation<L>, A> ma) =>
        mf.As().Apply(ma.As());

    static K<Validation<L>, B> Applicative<Validation<L>>.Action<A, B>(
        K<Validation<L>, A> ma, 
        K<Validation<L>, B> mb) =>
        Prelude.fun((A _, B b) => b).Map(ma).Apply(mb).As();

    static K<Validation<L>, A> Alternative<Validation<L>>.Empty<A>() =>
        Validation<L, A>.Fail(L.Empty);

    static K<Validation<L>, A> Alternative<Validation<L>>.Or<A>(
        K<Validation<L>, A> ma,
        K<Validation<L>, A> mb) =>
        ma.As() | mb.As();
}
