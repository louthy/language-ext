using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace LanguageExt
{
    public static class NullableRxExtensions
    {
        public static IObservable<A> ToObservable<A>(this A? ma) where A : struct =>
            ma.HasValue
                ? Observable.Return(ma.Value)
                : Observable.Empty<A>();

        /// <summary>
        /// Match the two states of the Nullable and return an observable stream of Rs.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A stream of Rs</returns>
        [Pure]
        public static IObservable<R> MatchObservable<T, R>(this T? self, Func<T, IObservable<R>> Some, Func<R> None) where T : struct =>
            self.HasValue
                ? Some(self.Value)
                : Observable.Return(None());


        /// <summary>
        /// Match the two states of the IObservable&lt;Nullable&lt;T&gt;&gt; and return a stream of non-null Rs.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A stream of Rs</returns>
        [Pure]
        public static IObservable<R> MatchObservable<T, R>(this IObservable<T?> self, Func<T, R> Some, Func<R> None) where T : struct =>
            self.Select(nullable => nullable.Match(Some, None));

        /// <summary>
        /// Match the two states of the Nullable and return an observable stream of Rs.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A stream of Rs</returns>
        [Pure]
        public static IObservable<R> MatchObservable<T, R>(this T? self, Func<T, IObservable<R>> Some, Func<IObservable<R>> None) where T : struct =>
            self.HasValue
                ? Some(self.Value)
                : None();
    }
}
