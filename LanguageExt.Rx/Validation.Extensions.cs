using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static class ValidationRxExtensions
    {
        /// <summary>
        /// Match the two states of the Validation and return an observable stream of non-null R2s.
        /// </summary>
        [Pure]
        public static IObservable<R2> MatchObservable<MonoidFail, FAIL, SUCCESS, R2>(this Validation<MonoidFail, FAIL, SUCCESS> ma, Func<SUCCESS, IObservable<R2>> Succ, Func<FAIL, R2> Fail) where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ma.Match( Succ: Succ, Fail: x => Observable.Return(Fail(x)));

        /// <summary>
        /// Match the two states of the Validation and return an observable stream of non-null R2s.
        /// </summary>
        [Pure]
        public static IObservable<R2> MatchObservable<MonoidFail, FAIL, SUCCESS, R2>(this Validation<MonoidFail, FAIL, SUCCESS> ma, Func<SUCCESS, IObservable<R2>> Succ, Func<FAIL, IObservable<R2>> Fail) where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ma.Match(Succ: Succ, Fail: Fail);
    }
}
