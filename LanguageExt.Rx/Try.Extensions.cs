using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace LanguageExt
{
    public static class TryRxExtensions
    {
        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="R">Returned observable bound type</typeparam>
        /// <param name="self">This</param>
        /// <param name="Succ">Function to call when the operation succeeds</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        [Pure]
        public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, IObservable<R>> Succ, Func<Exception, R> Fail) =>
            self.ToAsync().MatchObservable(Succ, Fail);

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="R">Returned observable bound type</typeparam>
        /// <param name="self">This</param>
        /// <param name="Succ">Function to call when the operation succeeds</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        [Pure]
        public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, IObservable<R>> Succ, Func<Exception, IObservable<R>> Fail) =>
            self.ToAsync().MatchObservable(Succ, Fail);

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="R">Returned observable bound type</typeparam>
        /// <param name="self">This</param>
        /// <param name="Succ">Function to call when the operation succeeds</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        [Pure]
        public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, IObservable<R>> Fail) =>
            self.ToAsync().MatchObservable(Succ, Fail);


        public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, R> Succ, Func<Exception, R> Fail) =>
            self.Select(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(Succ, Fail);
            });

        public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, IObservable<R>> Succ, Func<Exception, R> Fail) =>
            from tt in self.Select(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(Succ, x => Observable.Return(Fail(x)));
            })
            from t in tt
            select t;

        public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, IObservable<R>> Succ, Func<Exception, IObservable<R>> Fail) =>
            from tt in self.Select(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(Succ, Fail);
            })
            from t in tt
            select t;

        public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, R> Succ, Func<Exception, IObservable<R>> Fail) =>
            from tt in self.Select(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(x => Observable.Return(Succ(x)), Fail);
            })
            from t in tt
            select t;
    }
}
