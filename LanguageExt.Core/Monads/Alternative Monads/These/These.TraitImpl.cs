using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class These<A> : 
    Monad<These<A>>, 
    Traversable<These<A>> 
    where A : Semigroup<A>
{
    public static K<These<A>, C> Map<B, C>(Func<B, C> f, K<These<A>, B> ma) =>
        ma.As().Map(f);

    public static K<These<A>, B> Pure<B>(B value) => 
        These.That<A, B>(value);

    public static K<These<A>, C> Apply<B, C>(K<These<A>, Func<B, C>> mf, K<These<A>, B> ma) =>
        (mf, ma) switch
        {
            (This<A, Func<B, C>> (var a), _)                                 => These.This<A, C>(a),
            (That<A, Func<B, C>>, This<A, B> (var a))                        => These.This<A, C>(a),
            (That<A, Func<B, C>> (var f), That<A, B> (var b))                => These.That<A, C>(f(b)),
            (That<A, Func<B, C>> (var f), Pair<A, B> (var a, var b))         => These.Pair(a, f(b)),
            (Pair<A, Func<B, C>> (var a1, _), This<A, B> (var a2))           => These.This<A, C>(a1 + a2),
            (Pair<A, Func<B, C>> (var a1, var f), That<A, B> (var b))        => These.Pair(a1, f(b)),
            (Pair<A, Func<B, C>> (var a1, var f), Pair<A, B> (var a, var b)) => These.Pair(a1 + a, f(b)),
            _                                                                => throw new NotSupportedException()
        };

    public static K<These<A>, C> Bind<B, C>(K<These<A>, B> ma, Func<B, K<These<A>, C>> f) =>
        ma.As().Bind(f);

    public static S FoldWhile<B, S>(
        Func<B, Func<S, S>> f,
        Func<(S State, B Value), bool> predicate,
        S state,
        K<These<A>, B> ta) =>
        ta switch
        {
            This<A, B>            => state,
            That<A, B> (var b)    => predicate((state, b)) ? f(b)(state) : state,
            Pair<A, B> (_, var b) => predicate((state, b)) ? f(b)(state) : state,
            _                     => throw new NotSupportedException()
        };

    public static S FoldBackWhile<B, S>(
        Func<S, Func<B, S>> f, 
        Func<(S State, B Value), bool> predicate, 
        S state, 
        K<These<A>, B> ta) => 
        ta switch
        {
            This<A, B>            => state,
            That<A, B> (var b)    => predicate((state, b)) ? f(state)(b) : state,
            Pair<A, B> (_, var b) => predicate((state, b)) ? f(state)(b) : state,
            _                     => throw new NotSupportedException()
        };

    public static K<F, K<These<A>, C>> Traverse<F, B, C>(Func<B, K<F, C>> f, K<These<A>, B> ta)
        where F : Applicative<F> =>
        ta switch
        {
            This<A, B> (var a)        => F.Pure(These.This<A, C>(a).Kind()),
            That<A, B> (var b)        => F.Map(x => These.That<A, C>(x).Kind(), f(b)),
            Pair<A, B> (var a, var b) => F.Map(x => These.Pair(a, x).Kind(), f(b)),
            _                         => throw new NotSupportedException()
        };
}
