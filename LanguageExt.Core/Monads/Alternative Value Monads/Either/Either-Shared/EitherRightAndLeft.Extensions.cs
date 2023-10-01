using LanguageExt;
using System;

public static class EitherRightLeftExt
{
    public static Either<L, C> SelectMany<L, R, B, C>(this Either<L, R> ma, Func<R, EitherRight<B>> bind, Func<R, B, C> project) =>
        ma.Match(
            r => Prelude.Right<L, C>(project(r, bind(r).Value)),
            l => Prelude.Left<L, C>(l));

    public static Either<L, C> SelectMany<L, R, C>(this Either<L, R> ma, Func<R, EitherRight<L>> bind, Func<R, Unit, C> project) =>
        ma.Match(
            r => Prelude.Left<L, C>(bind(r).Value),
            l => Prelude.Left<L, C>(l));

    public static EitherRight<B> Apply<A, B>(this EitherRight<Func<A, B>> mf, EitherRight<A> ma) =>
        mf.Bind(f => ma.Map(a => f(a)));

    public static EitherRight<Func<B, C>> Apply<A, B, C>(this EitherRight<Func<A, B, C>> mf, EitherRight<A> ma) =>
        mf.Bind(f => ma.Map<Func<B, C>>(a => b => f(a, b)));

    public static EitherRight<C> Apply<A, B, C>(this EitherRight<Func<A, B, C>> mf, EitherRight<A> ma, EitherRight<B> mb) =>
        mf.Bind(f => ma.Bind(a => mb.Map(b => f(a, b))));

    public static Either<L, B> Apply<L, A, B>(this EitherRight<Func<A, B>> mf, Either<L, A> ma) =>
        mf.Bind(f => ma.Map(a => f(a)));

    public static Either<L, Func<B, C>> Apply<L, A, B, C>(this EitherRight<Func<A, B, C>> mf, Either<L, A> ma) =>
        mf.Bind(f => ma.Map<Func<B, C>>(a => b => f(a, b)));

    public static Either<L, C> Apply<L, A, B, C>(this EitherRight<Func<A, B, C>> mf, Either<L, A> ma, Either<L, B> mb) =>
        mf.Bind(f => ma.Bind(a => mb.Map(b => f(a, b))));
}
