using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude; 

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
            These.This<A, B>            => state,
            These.That<A, B> (var b)    => predicate((state, b)) ? f(b)(state) : state,
            These.Both<A, B> (_, var b) => predicate((state, b)) ? f(b)(state) : state,
            _                           => throw new NotSupportedException()
        };

    public static S FoldBackWhile<B, S>(
        Func<S, Func<B, S>> f, 
        Func<(S State, B Value), bool> predicate, 
        S state, 
        K<These<A>, B> ta) => 
        ta switch
        {
            These.This<A, B>            => state,
            These.That<A, B> (var b)    => predicate((state, b)) ? f(state)(b) : state,
            These.Both<A, B> (_, var b) => predicate((state, b)) ? f(state)(b) : state,
            _                           => throw new NotSupportedException()
        };

    public static K<F, K<These<A>, C>> Traverse<F, B, C>(Func<B, K<F, C>> f, K<These<A>, B> ta)
        where F : Applicative<F> =>
        ta switch
        {
            These.This<A, B> (var a)        => F.Pure(This<A, C>(a).Kind()),
            These.That<A, B> (var b)        => F.Map(x => That<A, C>(x).Kind(), f(b)),
            These.Both<A, B> (var a, var b) => F.Map(x => Both(a, x).Kind(), f(b)),
            _                               => throw new NotSupportedException()
        };
}
