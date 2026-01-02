using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `Validation` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class Validation<FAIL> :
    Monad<Validation<FAIL>>,
    MonoidK<Validation<FAIL>>,
    Alternative<Validation<FAIL>>,
    Traversable<Validation<FAIL>>,
    Fallible<FAIL, Validation<FAIL>>
{
    static K<Validation<FAIL>, B> Monad<Validation<FAIL>>.Bind<A, B>(
        K<Validation<FAIL>, A> ma,
        Func<A, K<Validation<FAIL>, B>> f) =>
        ma switch
        {
            Validation<FAIL, A>.Success (var x) => f(x),
            Validation<FAIL, A>.Fail (var e)    => Validation.FailI<FAIL, B>(e),
            _                                   => throw new NotSupportedException()
        };

    static K<Validation<FAIL>, B> Monad<Validation<FAIL>>.Recur<A, B>(A value, Func<A, K<Validation<FAIL>, Next<A, B>>> f)
    {
        while (true)
        {
            var mr = +f(value);
            if (mr.IsFail) return Validation.FailI<FAIL, B>(mr.FailValue);
            var next = (Next<A, B>)mr;
            if(next.IsDone) return Validation.SuccessI<FAIL, B>(next.Done);
            value = next.Loop;
        }
    }

    static K<Validation<FAIL>, B> Functor<Validation<FAIL>>.Map<A, B>(
        Func<A, B> f,
        K<Validation<FAIL>, A> ma) =>
        ma switch
        {
            Validation<FAIL, A>.Success (var x) => Validation.SuccessI<FAIL, B>(f(x)),
            Validation<FAIL, A>.Fail (var e)    => Validation.FailI<FAIL, B>(e),
            _                                   => throw new NotSupportedException()
        };

    static K<Validation<FAIL>, A> Applicative<Validation<FAIL>>.Pure<A>(A value) =>
        Validation.SuccessI<FAIL, A>(value);

    static K<Validation<FAIL>, B> Applicative<Validation<FAIL>>.Apply<A, B>(
        K<Validation<FAIL>, Func<A, B>> mf,
        K<Validation<FAIL>, A> ma) =>
        mf.ApplyI(ma, SemigroupInstance<FAIL>.Instance);

    static K<Validation<FAIL>, B> Applicative<Validation<FAIL>>.Apply<A, B>(
        K<Validation<FAIL>, Func<A, B>> mf,
        Memo<Validation<FAIL>, A> ma) =>
        mf.ApplyI(ma.Value, SemigroupInstance<FAIL>.Instance);

    static K<Validation<FAIL>, A> Alternative<Validation<FAIL>>.Empty<A>() =>
        MonoidInstance<FAIL>.Instance switch
        {
            { IsSome: true, Value: { } monoid } =>
                Validation.FailI<FAIL, A>(monoid.Empty),

            _ => throw new NotSupportedException($"{typeof(FAIL).Name} must be a Monoid")
        };

    static K<Validation<FAIL>, A> MonoidK<Validation<FAIL>>.Empty<A>() =>
        MonoidInstance<FAIL>.Instance switch
        {
            { IsSome: true, Value: { } monoid } =>
                Validation.FailI<FAIL, A>(monoid.Empty),

            _ => throw new NotSupportedException($"{typeof(FAIL).Name} must be a Monoid")
        };

    static K<Validation<FAIL>, A> SemigroupK<Validation<FAIL>>.Combine<A>(
        K<Validation<FAIL>, A> ma,
        K<Validation<FAIL>, A> mb) =>
        ma.As().CombineFirst(mb.As(), SemigroupInstance<FAIL>.Instance);

    static K<Validation<FAIL>, A> Choice<Validation<FAIL>>.Choose<A>(
        K<Validation<FAIL>, A> ma,
        K<Validation<FAIL>, A> mb) =>
        (ma, mb) switch
        {
            (Validation<FAIL, A>.Success, _) => ma,
            (_, Validation<FAIL, A>.Success) => mb,
            _                                => ma
        };

    static K<Validation<FAIL>, A> Choice<Validation<FAIL>>.Choose<A>(
        K<Validation<FAIL>, A> ma,
        Memo<Validation<FAIL>, A> mb) =>
        ma switch
        {
            Validation<FAIL, A>.Success => ma,
            _                           => mb.Value switch
                                           {
                                               Validation<FAIL, A>.Success b => b,
                                               _ => ma
                                           }
        };

    static K<F, K<Validation<FAIL>, B>> Traversable<Validation<FAIL>>.Traverse<F, A, B>(
        Func<A, K<F, B>> f,
        K<Validation<FAIL>, A> ta) =>
        ta switch
        {
            Validation<FAIL, A>.Success (var x) => F.Map(Succ, f(x)),
            Validation<FAIL, A>.Fail (var e)    => F.Pure(Fail<B>(e)),
            _                                   => throw new NotSupportedException()
        };

    static K<Validation<FAIL>, A> Succ<A>(A value) =>
        Validation.SuccessI<FAIL, A>(value);

    static K<Validation<FAIL>, A> Fail<A>(FAIL value) =>
        Validation.FailI<FAIL, A>(value);

    static K<Validation<FAIL>, A> Fallible<FAIL, Validation<FAIL>>.Fail<A>(FAIL error) =>
        Validation.FailI<FAIL, A>(error);

    static K<Validation<FAIL>, A> Fallible<FAIL, Validation<FAIL>>.Catch<A>(
        K<Validation<FAIL>, A> fa,
        Func<FAIL, bool> Predicate,
        Func<FAIL, K<Validation<FAIL>, A>> Fail) =>
        fa switch
        {
            Validation<FAIL, A>.Success mx => mx,
            Validation<FAIL, A>.Fail (var e) =>
                Predicate(e)
                    ? Fail(e)
                    : Validation.FailI<FAIL, A>(e),
            _ => throw new NotSupportedException()
        };

    static Fold<A, S> Foldable<Validation<FAIL>>.FoldStep<A, S>(K<Validation<FAIL>, A> ta, in S initialState)
    {
        var ma = ta.As();
        return ma.IsSuccess
                   ? Fold.Loop(initialState, ma.SuccessValue, Fold.Done<A, S>)
                   : Fold.Done<A, S>(initialState);
    }
        
    static Fold<A, S> Foldable<Validation<FAIL>>.FoldStepBack<A, S>(K<Validation<FAIL>, A> ta, in S initialState) =>
        ta.FoldStep(initialState);
}
