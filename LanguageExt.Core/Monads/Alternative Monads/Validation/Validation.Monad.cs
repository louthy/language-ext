using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `Validation` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class Validation<FAIL> : 
    Monad<Validation<FAIL>>, 
    Alternative<Validation<FAIL>>,
    Traversable<Validation<FAIL>>, 
    Fallible<FAIL, Validation<FAIL>> 
    where FAIL : Monoid<FAIL>
{
    static K<Validation<FAIL>, B> Monad<Validation<FAIL>>.Bind<A, B>(
        K<Validation<FAIL>, A> ma, 
        Func<A, K<Validation<FAIL>, B>> f) => 
        ma switch
        {
            Validation.Success<FAIL, A> (var x) => f(x),
            Validation.Fail<FAIL, A> (var e)    => Validation<FAIL, B>.Fail(e),
            _                                => throw new NotSupportedException()
        }; 
    
    static K<Validation<FAIL>, B> Functor<Validation<FAIL>>.Map<A, B>(
        Func<A, B> f, 
        K<Validation<FAIL>, A> ma) => 
        ma switch
        {
            Validation.Success<FAIL, A> (var x) => Validation<FAIL, B>.Success(f(x)),
            Validation.Fail<FAIL, A> (var e)    => Validation<FAIL, B>.Fail(e),
            _                                => throw new NotSupportedException()
        }; 

    static K<Validation<FAIL>, A> Applicative<Validation<FAIL>>.Pure<A>(A value) => 
        Validation<FAIL, A>.Success(value);

static K<Validation<FAIL>, B> Applicative<Validation<FAIL>>.Apply<A, B>(
    K<Validation<FAIL>, Func<A, B>> mf,
    K<Validation<FAIL>, A> ma) =>
    mf switch
    {
        Validation.Success<FAIL, Func<A, B>> (var f) =>
            ma switch
            {
                Validation.Success<FAIL, A> (var a) =>
                    Validation<FAIL, B>.Success(f(a)),

                Validation.Fail<FAIL, A> (var e) =>
                    Validation<FAIL, B>.Fail(e),

                _ =>
                    Validation<FAIL, B>.Fail(FAIL.Empty)
            },

        Validation.Fail<FAIL, Func<A, B>> (var e1) =>
            ma switch
            {
                Validation.Fail<FAIL, A> (var e2) =>
                    Validation<FAIL, B>.Fail(e1 + e2),

                _ =>
                    Validation<FAIL, B>.Fail(e1)

            },
        _ => Validation<FAIL, B>.Fail(FAIL.Empty)
    };
    
    static K<Validation<FAIL>, B> Applicative<Validation<FAIL>>.Action<A, B>(
        K<Validation<FAIL>, A> ma, 
        K<Validation<FAIL>, B> mb) =>
        ma switch
        {
            Validation.Success<FAIL, A> =>
                mb,

            Validation.Fail<FAIL, B> (var e1) =>
                mb switch
                {
                    Validation.Fail<FAIL, B> (var e2) =>
                        Validation<FAIL, B>.Fail(e1 + e2),

                    _ =>
                        Validation<FAIL, B>.Fail(e1)

                },
            _ => Validation<FAIL, B>.Fail(FAIL.Empty)
        };
    
    static K<Validation<FAIL>, A> MonoidK<Validation<FAIL>>.Empty<A>() =>
        Validation<FAIL, A>.Fail(FAIL.Empty);

    static K<Validation<FAIL>, A> SemigroupK<Validation<FAIL>>.Combine<A>(
        K<Validation<FAIL>, A> ma,
        K<Validation<FAIL>, A> mb) =>
        (ma, mb) switch
        {
            (Validation.Success<FAIL, A> , _) => 
                ma,
            
            (Validation.Fail<FAIL, A> (var e1), Validation.Fail<FAIL, A> (var e2)) => 
                Validation<FAIL, A>.Fail(e1.Combine(e2)),
            
            _ => mb
        };

    static K<Validation<FAIL>, A> Choice<Validation<FAIL>>.Choose<A>(
        K<Validation<FAIL>, A> ma,
        K<Validation<FAIL>, A> mb) =>
        (ma, mb) switch
        {
            (Validation.Success<FAIL, A>, _) => ma,
            (_, Validation.Success<FAIL, A>) => mb,
            _                                => ma
        };

    static S Foldable<Validation<FAIL>>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState,
        K<Validation<FAIL>, A> ta) =>
        ta switch
        {
            Validation.Success<FAIL, A> (var x) =>
                predicate((initialState, x)) 
                    ? f(x)(initialState) 
                    : initialState,

            _ => initialState
        };        

    static S Foldable<Validation<FAIL>>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<Validation<FAIL>, A> ta) => 
        ta switch
        {
            Validation.Success<FAIL, A> (var x) =>
                predicate((initialState, x)) 
                    ? f(initialState)(x) 
                    : initialState,

            _ => initialState
        };        

    static K<F, K<Validation<FAIL>, B>> Traversable<Validation<FAIL>>.Traverse<F, A, B>(
        Func<A, K<F, B>> f, 
        K<Validation<FAIL>, A> ta) => 
        ta switch
        {
            Validation.Success<FAIL, A> (var x) => F.Map(Succ, f(x)),
            Validation.Fail<FAIL, A> (var e)    => F.Pure(Fail<B>(e)),
            _                                => throw new NotSupportedException()
        };         

    static K<Validation<FAIL>, A> Succ<A>(A value) =>
        Validation<FAIL, A>.Success(value);

    static K<Validation<FAIL>, A> Fail<A>(FAIL value) =>
        Validation<FAIL, A>.Fail(value);

    static K<Validation<FAIL>, A> Fallible<FAIL, Validation<FAIL>>.Fail<A>(FAIL error) => 
        Validation<FAIL, A>.Fail(error);

    static K<Validation<FAIL>, A> Fallible<FAIL, Validation<FAIL>>.Catch<A>(
        K<Validation<FAIL>, A> fa,
        Func<FAIL, bool> Predicate,
        Func<FAIL, K<Validation<FAIL>, A>> Fail) =>
        fa.As().BindFail(e => Predicate(e) ? Fail(e).As() : Validation<FAIL, A>.Fail(e));
}
