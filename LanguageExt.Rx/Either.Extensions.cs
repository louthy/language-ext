using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class EitherRxExtensions
{
    public static IObservable<R> ToObservable<L, R>(this Either<L, R> ma) =>
        ma.Match(_ => Observable.Empty<R>(), Observable.Return);

    /// <summary>
    /// Match the two states of the Either and return a stream of non-null R2s.
    /// </summary>
    [Pure]
    public static IObservable<R2> MatchObservable<L, R, R2>(this Either<L, IObservable<R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        self.IsRight
            ? self.CastRight().Select(Right).Select(Check.NullReturn)
            : Observable.Return(Check.NullReturn(Left(self.CastLeft())));

    /// <summary>
    /// Match the two states of the IObservable Either and return a stream of non-null R2s.
    /// </summary>
    [Pure]
    public static IObservable<R2> MatchObservable<L, R, R2>(this IObservable<Either<L, R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        self.Select(either => either.Match(Left, Right));


    /// <summary>
    /// Match the two states of the Either and return an observable stream of non-null R2s.
    /// </summary>
    [Pure]
    public static IObservable<R2> MatchObservable<L, R, R2>(this Either<L, R> ma, Func<R, IObservable<R2>> Right, Func<L, R2> Left) =>
        ma.Match(l => Observable.Return(Left(l)), Right);

    /// <summary>
    /// Match the two states of the Either and return an observable stream of non-null R2s.
    /// </summary>
    [Pure]
    public static IObservable<R2> MatchObservable<L, R, R2>(this Either<L, R> ma, Func<R, IObservable<R2>> Right, Func<L, IObservable<R2>> Left) =>
        ma.Match(Left, Right);
}
