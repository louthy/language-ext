using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace LanguageExt
{
    public static class TryOptionAsyncRxExtensions
    {
        public static IObservable<A> ToObservable<A>(this TryOptionAsync<A> ma) =>
            from ra in ma.Try().ToObservable()
            where ra.IsSome
            select ra.IfFailOrNone(default(A));

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="B">Returned observable bound type</typeparam>
        /// <param name="ma">This</param>
        /// <param name="Some">Function to call when the operation succeeds</param>
        /// <param name="None">Function to call when the operation succeeds but returns no value</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        public static IObservable<B> MatchObservable<A, B>(this TryOptionAsync<A> ma, Func<A, IObservable<B>> Some, Func<B> None, Func<Exception, B> Fail) =>
            from ra in ma.Try().ToObservable()
            from rb in ra.Match(Some, () => Observable.Return(None()), e => Observable.Return(Fail(e)))
            select rb;

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="B">Returned observable bound type</typeparam>
        /// <param name="ma">This</param>
        /// <param name="Some">Function to call when the operation succeeds</param>
        /// <param name="None">Function to call when the operation succeeds but returns no value</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        public static IObservable<B> MatchObservable<A, B>(this TryOptionAsync<A> ma, Func<A, IObservable<B>> Some, Func<IObservable<B>> None, Func<Exception, B> Fail) =>
            from ra in ma.Try().ToObservable()
            from rb in ra.Match(Some, None, e => Observable.Return(Fail(e)))
            select rb;

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="R">Returned observable bound type</typeparam>
        /// <param name="ma">This</param>
        /// <param name="Some">Function to call when the operation succeeds</param>
        /// <param name="None">Function to call when the operation succeeds but returns no value</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        public static IObservable<R> MatchObservable<A, R>(this TryOptionAsync<A> ma, Func<A, IObservable<R>> Some, Func<R> None, Func<Exception, IObservable<R>> Fail) =>
            from ra in ma.Try().ToObservable()
            from rb in ra.Match(Some, () => Observable.Return(None()), Fail)
            select rb;

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="R">Returned observable bound type</typeparam>
        /// <param name="ma">This</param>
        /// <param name="Some">Function to call when the operation succeeds</param>
        /// <param name="None">Function to call when the operation succeeds but returns no value</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        public static IObservable<R> MatchObservable<A, R>(this TryOptionAsync<A> ma, Func<A, IObservable<R>> Some, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail) =>
            from ra in ma.Try().ToObservable()
            from rb in ra.Match(Some, None, Fail)
            select rb;

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="R">Returned observable bound type</typeparam>
        /// <param name="ma">This</param>
        /// <param name="Some">Function to call when the operation succeeds</param>
        /// <param name="None">Function to call when the operation succeeds but returns no value</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        public static IObservable<R> MatchObservable<A, R>(this TryOptionAsync<A> ma, Func<A, R> Some, Func<R> None, Func<Exception, IObservable<R>> Fail) =>
            from ra in ma.Try().ToObservable()
            from rb in ra.Match(x => Observable.Return(Some(x)), () => Observable.Return(None()), Fail)
            select rb;

        /// <summary>
        /// Turns the computation into an observable stream
        /// </summary>
        /// <typeparam name="A">Bound type</typeparam>
        /// <typeparam name="R">Returned observable bound type</typeparam>
        /// <param name="ma">This</param>
        /// <param name="Some">Function to call when the operation succeeds</param>
        /// <param name="None">Function to call when the operation succeeds but returns no value</param>
        /// <param name="Fail">Function to call when the operation fails</param>
        /// <returns>An observable that represents the result of Succ or Fail</returns>
        public static IObservable<R> MatchObservable<A, R>(this TryOptionAsync<A> ma, Func<A, R> Some, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail) =>
            from ra in ma.Try().ToObservable()
            from rb in ra.Match(x => Observable.Return(Some(x)), None, Fail)
            select rb;
    }
}
