using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace LanguageExt
{
    public static class TryAsyncRxExtensions
    {
        public static IObservable<A> ToObservable<A>(this TryAsync<A> ma) =>
            from ra in ma.Try().ToObservable()
            where ra.IsSuccess
            select ra.IfFail(default(A));

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="B">Returned observable bound type</typeparam>
        /// <param name="ma">This</param>
        /// <param name="Succ">Function to call when the operation succeeds</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        public static IObservable<B> MatchObservable<A, B>(this TryAsync<A> ma, Func<A, IObservable<B>> Succ, Func<Exception, B> Fail) =>
            from ra in ma.Try().ToObservable()
            from rb in ra.Match(Succ, e => Observable.Return(Fail(e)))
            select rb;

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="R">Returned observable bound type</typeparam>
        /// <param name="ma">This</param>
        /// <param name="Succ">Function to call when the operation succeeds</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        public static IObservable<R> MatchObservable<A, R>(this TryAsync<A> ma, Func<A, IObservable<R>> Succ, Func<Exception, IObservable<R>> Fail) =>
            from ra in ma.Try().ToObservable()
            from rb in ra.Match(Succ, Fail)
            select rb;

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="R">Returned observable bound type</typeparam>
        /// <param name="ma">This</param>
        /// <param name="Succ">Function to call when the operation succeeds</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        public static IObservable<R> MatchObservable<A, R>(this TryAsync<A> ma, Func<A, R> Succ, Func<Exception, IObservable<R>> Fail) =>
            from ra in ma.Try().ToObservable()
            from rb in ra.Match(x => Observable.Return(Succ(x)), Fail)
            select rb;
    }
}
