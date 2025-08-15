using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class These<A> : Traversable<These<A>> 
{
    public static K<These<A>, C> Map<B, C>(Func<B, C> f, K<These<A>, B> ma) =>
        ma.As().Map(f);

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
