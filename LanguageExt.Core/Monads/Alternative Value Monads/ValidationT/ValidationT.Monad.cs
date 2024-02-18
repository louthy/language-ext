using System;
using LanguageExt.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `ValidationT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class ValidationT<L, M> : 
    MonadT<ValidationT<L, M>, M>, 
    Alternative<ValidationT<L, M>>
    where M : Monad<M>
    where L : Monoid<L>
{
    static K<ValidationT<L, M>, B> Monad<ValidationT<L, M>>.Bind<A, B>(
        K<ValidationT<L, M>, A> ma, 
        Func<A, K<ValidationT<L, M>, B>> f) => 
        ma.As().Bind(f);

    static K<ValidationT<L, M>, B> Functor<ValidationT<L, M>>.Map<A, B>(
        Func<A, B> f, 
        K<ValidationT<L, M>, A> ma) => 
        ma.As().Map(f);

    static K<ValidationT<L, M>, A> Applicative<ValidationT<L, M>>.Pure<A>(A value) => 
        ValidationT<L, M, A>.Success(value);

    static K<ValidationT<L, M>, B> Applicative<ValidationT<L, M>>.Apply<A, B>(
        K<ValidationT<L, M>, Func<A, B>> mf,
        K<ValidationT<L, M>, A> ma) =>
        mf.As().Apply(ma.As());

    static K<ValidationT<L, M>, B> Applicative<ValidationT<L, M>>.Action<A, B>(
        K<ValidationT<L, M>, A> ma, 
        K<ValidationT<L, M>, B> mb) =>
        Prelude.fun((A _, B b) => b).Map(ma).Apply(mb).As();

    static K<ValidationT<L, M>, A> MonadT<ValidationT<L, M>, M>.Lift<A>(K<M, A> ma) => 
        ValidationT<L, M, A>.Lift(ma);
    
    static K<ValidationT<L, M>, A> Monad<ValidationT<L, M>>.LiftIO<A>(IO<A> ma) => 
        ValidationT<L, M, A>.Lift(M.LiftIO(ma));

    static K<ValidationT<L, M>, A> Alternative<ValidationT<L, M>>.Empty<A>() =>
        ValidationT<L, M, A>.Fail(L.Empty);

    static K<ValidationT<L, M>, A> Alternative<ValidationT<L, M>>.Or<A>(
        K<ValidationT<L, M>, A> ma,
        K<ValidationT<L, M>, A> mb) =>
        ma.As() | mb.As();
}
