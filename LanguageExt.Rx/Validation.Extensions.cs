using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using LanguageExt.TypeClasses;

namespace LanguageExt;

public static class ValidationRxExtensions
{
    /// <summary>
    /// Match the two states of the Validation and return an observable stream of non-null R2s.
    /// </summary>
    [Pure]
    public static IObservable<R2> MatchObservable<F, A, R2>(
        this Validation<F, A> ma, 
        Func<A, IObservable<R2>> Succ, 
        Func<F, R2> Fail) 
        where F : Monoid<F>, Eq<F> =>
        ma.Match( Succ: Succ, Fail: x => Observable.Return(Fail(x)));

    /// <summary>
    /// Match the two states of the Validation and return an observable stream of non-null R2s.
    /// </summary>
    [Pure]
    public static IObservable<R2> MatchObservable<F, A, R2>(
        this Validation<F, A> ma, 
        Func<A, IObservable<R2>> Succ, 
        Func<F, IObservable<R2>> Fail) 
        where F : Monoid<F>, Eq<F> =>
        ma.Match(Succ: Succ, Fail: Fail);
}
