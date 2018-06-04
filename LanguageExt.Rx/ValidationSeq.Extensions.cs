using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace LanguageExt
{
    public static class ValidationSeqRxExtensions
    {
        /// <summary>
        /// Match the two states of the Validation and return an observable stream of non-null R2s.
        /// </summary>
        [Pure]
        public static IObservable<R2> MatchObservable<FAIL, SUCCESS, R2>(this Validation<FAIL, SUCCESS> ma, Func<SUCCESS, IObservable<R2>> Succ, Func<Seq<FAIL>, R2> Fail) =>
            ma.Match(Succ: Succ, Fail: x => Observable.Return(Fail(x)));

        /// <summary>
        /// Match the two states of the Validation and return an observable stream of non-null R2s.
        /// </summary>
        [Pure]
        public static IObservable<R2> MatchObservable<FAIL, SUCCESS, R2>(this Validation<FAIL, SUCCESS> ma, Func<SUCCESS, IObservable<R2>> Succ, Func<Seq<FAIL>, IObservable<R2>> Fail) =>
            ma.Match(Succ: Succ, Fail: Fail);
    }
}
