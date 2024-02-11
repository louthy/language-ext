using System;
using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// Monad trait implementation for `Either<L, R>`
/// </summary>
/// <typeparam name="L">Left type parameter</typeparam>
public class Either<L> : Monad<Either<L>>, Traversable<Either<L>>
{
    public static Applicative<Either<L>, B> Apply<A, B>(
        Applicative<Either<L>, Transducer<A, B>> mf, 
        Applicative<Either<L>, A> ma) =>
        from t in mf.As()
        from x in ma.As()
        from r in t.Invoke(x)
        select r;

    public static Applicative<Either<L>, B> Action<A, B>(
        Applicative<Either<L>, A> ma, 
        Applicative<Either<L>, B> mb) => 
        from _ in ma.As()
        from b in mb.As()
        select b;

    public static Monad<Either<L>, B> Bind<A, B>(
        Monad<Either<L>, A> ma,
        Transducer<A, Monad<Either<L>, B>> f) =>
        ma.As().Bind(f);

    public static Applicative<Either<L>, A> Pure<A>(A value) => 
        Either<L, A>.Right(value);

    public static Applicative<F, Traversable<Either<L>, B>> Traverse<F, A, B>(
        Func<A, Applicative<F, B>> f,
        Traversable<Either<L>, A> ta)
        where F : Applicative<F> =>
        ta.As()
          .Match(Right: r => F.Map(x => Either<L, B>.Right(x).AsTraversable(), f(r)).AsApplicative(),
                 Left:  l => F.Pure(Either<L, B>.Left(l).AsTraversable()));

    public static S Fold<A, S>(Func<A, S, S> f, S initialState, Foldable<Either<L>, A> ta) => 
        ta.As().Match(Right: r => f(r, initialState), Left: _ => initialState);

    public static B FoldBack<A, B>(Func<B, A, B> f, B initialState, Foldable<Either<L>, A> ta) => 
        ta.As().Match(Right: r => f(initialState, r), Left: _ => initialState);
}
