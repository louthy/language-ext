using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace LanguageExt
{
    public class EitherUnsafeRxExtensions
    {
        public static IObservable<R> ToObservable<L, R>(this EitherUnsafe<L, R> ma) =>
            ma.Match(Observable.Return, _ => Observable.Empty<R>());

        /// <summary>
        /// Match the two states of the Either and return a stream of non-null R2s.
        /// </summary>
        [Pure]
        public static IObservable<R2> MatchObservable<L, R, R2>(this EitherUnsafe<L, IObservable<R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
            self.IsRight
                ? self.CastRight().Select(Right).Select(Check.NullReturn)
                : Observable.Return(Check.NullReturn(Left(self.CastLeft())));

        /// <summary>
        /// Match the two states of the IObservable Either and return a stream of non-null R2s.
        /// </summary>
        [Pure]
        public static IObservable<R2> MatchObservable<L, R, R2>(this IObservable<EitherUnsafe<L, R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
            self.Select(either => matchUnsafe(either, Right, Left));

        /// <summary>
        /// Match the two states of the Either and return an observable stream of non-null R2s.
        /// </summary>
        [Pure]
        public static IObservable<R2> MatchObservable<L, R, R2>(this EitherUnsafe<L, R> ma, Func<R, IObservable<R2>> Right, Func<L, R2> Left) =>
            ma.Match(Right, l => Observable.Return(Left(l)));

        /// <summary>
        /// Match the two states of the Either and return an observable stream of non-null R2s.
        /// </summary>
        [Pure]
        public static IObservable<R2> MatchObservable<L, R, R2>(this EitherUnsafe<L, R> ma, Func<R, IObservable<R2>> Right, Func<L, IObservable<R2>> Left) =>
            ma.Match(Right, Left);
    }
}
