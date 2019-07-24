using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class TaskTryRxExtensions
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
        public static IObservable<R> MatchObservable<A, R>(this Task<Try<A>> self, Func<A, IObservable<R>> Succ, Func<Exception, R> Fail) =>
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
        public static IObservable<R> MatchObservable<A, R>(this Task<Try<A>> self, Func<A, IObservable<R>> Succ, Func<Exception, IObservable<R>> Fail) =>
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
        public static IObservable<R> MatchObservable<A, R>(this Task<Try<A>> self, Func<A, R> Succ, Func<Exception, IObservable<R>> Fail) =>
            self.ToAsync().MatchObservable(Succ, Fail);
    }
}
