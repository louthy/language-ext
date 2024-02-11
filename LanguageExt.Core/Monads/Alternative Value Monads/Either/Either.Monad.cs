using System;
using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// Monad trait implementation for `Either<L, R>`
/// </summary>
/// <typeparam name="L">Left type parameter</typeparam>
public class Either<L> : Monad<Either<L>>, Traversable<Either<L>>, Alternative<Either<L>>
{
    public static K<Either<L>, B> Apply<A, B>(
        K<Either<L>, Func<A, B>> mf, 
        K<Either<L>, A> ma) =>
        from f in mf.As()
        from x in ma.As()
        select f(x);

    public static K<Either<L>, B> Action<A, B>(
        K<Either<L>, A> ma, 
        K<Either<L>, B> mb) => 
        from _ in ma.As()
        from b in mb.As()
        select b;

    public static K<Either<L>, B> Bind<A, B>(
        K<Either<L>, A> ma,
        Func<A, K<Either<L>, B>> f) =>
        ma.As().Bind(f);

    public static K<Either<L>, A> Pure<A>(A value) => 
        Either<L, A>.Right(value);

    public static K<F, K<Either<L>, B>> Traverse<F, A, B>(Func<A, K<F, B>> f, K<Either<L>, A> ta)
        where F : Applicative<F> =>
        ta.As()
          .Match(Right: r => F.Map(Right, f(r)),
                 Left:  l => F.Pure(Left<B>(l)));

    public static S Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<Either<L>, A> ta) => 
        ta.As().Match(Right: r => f(r)(initialState), Left: _ => initialState);

    public static B FoldBack<A, B>(Func<B, Func<A, B>> f, B initialState, K<Either<L>, A> ta) => 
        ta.As().Match(Right: f(initialState), Left: _ => initialState);

    static K<Either<L>, A> Right<A>(A value) =>
        Either<L, A>.Right(value);

    static K<Either<L>, A> Left<A>(L value) =>
        Either<L, A>.Left(value);

    public static K<Either<L>, B> Map<A, B>(Func<A, B> f, K<Either<L>, A> ma) => 
        ma.As().Map(f);

    public static K<Either<L>, A> Empty<A>() => 
        Either<L, A>.Bottom;

    public static K<Either<L>, A> Or<A>(K<Either<L>, A> ma, K<Either<L>, A> mb) => 
        ma.As().IsRight ? ma : mb;
}
