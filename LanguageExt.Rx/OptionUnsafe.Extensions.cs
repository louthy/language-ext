using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace LanguageExt
{
    public static class OptionUnsafeRxExtensions
    {
        public static IObservable<A> ToObservable<A>(this OptionUnsafe<A> ma) =>
            ma.IsSome
                ? Observable.Return(ma.Cast())
                : Observable.Empty<A>();

        /// <summary>
        /// Match the two states of the OptionUnsafe and return an observable stream of non-null Rs.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some handler.  Must not return null.</param>
        /// <param name="None">None handler.  Must not return null.</param>
        /// <returns>A stream of non-null Rs</returns>
        [Pure]
        public static IObservable<B> MatchObservable<A, B>(this OptionUnsafe<A> ma, Func<A, IObservable<B>> Some, Func<B> None) =>
            ma.IsSome
                ? Some(ma.Cast())
                : Observable.Return(None());

        /// <summary>
        /// Match the two states of the OptionUnsafe and return an observable stream of non-null Rs.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some handler.  Must not return null.</param>
        /// <param name="None">None handler.  Must not return null.</param>
        /// <returns>A stream of non-null Rs</returns>
        [Pure]
        public static IObservable<B> MatchObservable<A, B>(this OptionUnsafe<A> ma, Func<A, IObservable<B>> Some, Func<IObservable<B>> None) =>
            ma.IsSome
                ? Some(ma.Cast())
                : None();
    }
}
