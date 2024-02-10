using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// Monad trait implementation for `Either<L, R>`
/// </summary>
/// <typeparam name="L">Left type parameter</typeparam>
public class Either<L> : Monad<Either<L>>
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

    public static Alternative<Either<L>, A> Empty<A>() => throw new System.NotImplementedException();

    public static Alternative<Either<L>, A> Or<A>(Alternative<Either<L>, A> ma, Alternative<Either<L>, A> mb) => throw new System.NotImplementedException();
}
