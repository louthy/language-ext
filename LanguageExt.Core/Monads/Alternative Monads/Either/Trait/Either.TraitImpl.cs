using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Monad trait implementation for `Either〈L, R〉`
/// </summary>
/// <typeparam name="L">Left type parameter</typeparam>
public class Either<L> :
    Either,
    Monad<Either<L>>,
    Fallible<L, Either<L>>,
    Traversable<Either<L>>,
    Natural<Either<L>, Option>,
    Choice<Either<L>>
{
    static K<Either<L>, B> Applicative<Either<L>>.Apply<A, B>(
        K<Either<L>, Func<A, B>> mf,
        K<Either<L>, A> ma) =>
        (mf, ma) switch
        {
            (Either<L, Func<A, B>>.Right (var f), Either<L, A>.Right (var a)) => Right(f(a)),
            (Either<L, Func<A, B>>.Left (var e1), _)                          => Left<B>(e1),
            (_, Either<L, A>.Left (var e2))                                   => Left<B>(e2),
            _                                                                 => throw new NotSupportedException()
        };

    static K<Either<L>, B> Applicative<Either<L>>.Apply<A, B>(
        K<Either<L>, Func<A, B>> mf,
        Memo<Either<L>, A> ma) =>
        mf switch
        {
            Either<L, Func<A, B>>.Right         => mf.Apply(ma.Value),
            Either<L, Func<A, B>>.Left (var e1) => Left<B>(e1),
            _                                   => throw new NotSupportedException()
        };
    
    static K<Either<L>, B> Monad<Either<L>>.Bind<A, B>(K<Either<L>, A> ma, Func<A, K<Either<L>, B>> f) =>
        ma switch
        {
            Either<L, A>.Right (var r) => f(r),
            Either<L, A>.Left (var l)  => Left<B>(l),
            _                          => throw new NotSupportedException()
        };

    static K<Either<L>, B> Monad<Either<L>>.Recur<A, B>(A value, Func<A, K<Either<L>, Next<A, B>>> f) 
    {
        while (true)
        {
            var mr = +f(value);
            if (mr.IsLeft) return Either.Left<L, B>(mr.LeftValue);
            var next = (Next<A, B>)mr;
            if(next.IsDone) return Either.Right<L, B>(next.Done);
            value = next.Loop;
        }
    }

    static K<Either<L>, A> Applicative<Either<L>>.Pure<A>(A value) => 
        new Either<L, A>.Right(value);

    static K<F, K<Either<L>, B>> Traversable<Either<L>>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Either<L>, A> ta) =>
        ta switch
        {
            Either<L, A>.Right (var r) => F.Map(Right, f(r)),
            Either<L, A>.Left (var l)  => F.Pure(Left<B>(l)),
            _                          => throw new NotSupportedException()
        };

    static S Foldable<Either<L>>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Either<L>, A> ta) =>
        ta switch
        {
            Either<L, A>.Right (var r) => predicate((state, r)) ? f(r)(state) : state,
            _                          => state
        };

    static S Foldable<Either<L>>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Either<L>, A> ta) =>
        ta switch
        {
            Either<L, A>.Right (var r) => predicate((state, r)) ? f(state)(r) : state,
            _                          => state
        };

    static K<Either<L>, B> Functor<Either<L>>.Map<A, B>(Func<A, B> f, K<Either<L>, A> ma) =>
        ma switch
        {
            Either<L, A>.Right (var r) => Right(f(r)),
            Either<L, A>.Left (var l)  => Left<B>(l),
            _                          => throw new NotSupportedException()
        };

    static K<Either<L>, A> Right<A>(A value) =>
        new Either<L, A>.Right(value);

    static K<Either<L>, A> Left<A>(L value) =>
        new Either<L, A>.Left(value);

    static K<Either<L>, A> Choice<Either<L>>.Choose<A>(K<Either<L>, A> ma, K<Either<L>, A> mb) =>
        ma is Either<L, A>.Right ? ma : mb;

    static K<Either<L>, A> Choice<Either<L>>.Choose<A>(K<Either<L>, A> ma, Memo<Either<L>, A> mb) => 
        ma is Either<L, A>.Right ? ma : mb.Value;

    static K<Either<L>, A> Fallible<L, Either<L>>.Fail<A>(L error) => 
        new Either<L, A>.Left(error);

    static K<Either<L>, A> Fallible<L, Either<L>>.Catch<A>(
        K<Either<L>, A> fa, Func<L, bool> Predicate,
        Func<L, K<Either<L>, A>> Fail) =>
        fa.As().BindLeft(l => Predicate(l) ? Fail(l).As() : Either.Left<L, A>(l));

    static K<Option, A> Natural<Either<L>, Option>.Transform<A>(K<Either<L>, A> fa) =>
        fa switch
        {
            Either<L, A>.Right (var r) => Option.Some(r),
            _                          => Option<A>.None
        };
    
    static Fold<A, S> Foldable<Either<L>>.FoldStep<A, S>(K<Either<L>, A> ta, S initialState)
    {
        var ma = ta.As();
        return ma.IsRight
                   ? Fold.Loop(initialState, ma.RightValue, Fold.Done<A, S>)
                   : Fold.Done<A, S>(initialState);
    }    
        
    static Fold<A, S> Foldable<Either<L>>.FoldStepBack<A, S>(K<Either<L>, A> ta, S initialState) =>
        ta.FoldStep(initialState);
}
