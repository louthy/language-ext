using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ValidationExtensions
{
    public static Validation<F, A> As<F, A>(this K<Validation<F>, A> ma) 
        where F : Monoid<F> =>
        (Validation<F, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Validation<L, A> Flatten<L, A>(this Validation<L, Validation<L, A>> mma)
        where L : Monoid<L> =>
        mma.Bind(x => x);

    /// <summary>
    /// Extract only the successes 
    /// </summary>
    /// <param name="vs">Enumerable of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of successes</returns>
    [Pure]
    public static IEnumerable<S> Successes<F, S>(this IEnumerable<Validation<F, S>> vs)
        where F : Monoid<F>
    {
        foreach (var v in vs)
        {
            if (v.IsSuccess) yield return (S)v;
        }
    }

    /// <summary>
    /// Extract only the failures 
    /// </summary>
    /// <param name="vs">Enumerable of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of failures</returns>
    [Pure]
    public static IEnumerable<F> Fails<F, S>(this IEnumerable<Validation<F, S>> vs)
        where F : Monoid<F>
    {
        foreach (var v in vs)
        {
            if (v.IsFail) yield return (F)v;
        }
    }
    
    /// <summary>
    /// Extract only the successes 
    /// </summary>
    /// <param name="vs">Seq of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of successes</returns>
    [Pure]
    public static Seq<S> Successes<F, S>(this Seq<Validation<F, S>> vs)
        where F : Monoid<F> =>
        toSeq(Successes(vs.AsEnumerable()));
    
    /// <summary>
    /// Extract only the failures 
    /// </summary>
    /// <param name="vs">Seq of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of failures</returns>
    [Pure]
    public static Seq<F> Fails<F, S>(this Seq<Validation<F, S>> vs)
        where F : Monoid<F> => 
        toSeq(Fails(vs.AsEnumerable()));

    /// <summary>
    /// Convert `Validation` type to `Fin` type.
    /// </summary>
    [Pure]
    public static Fin<A> ToFin<A>(this Validation<Error, A> ma) =>
        ma switch
        {
            Validation.Success<Error, A> (var x) => new Fin.Succ<A>(x),
            Validation.Fail<Error, A> (var x)    => new Fin.Fail<A>(x),
            _                                    => throw new NotSupportedException()
        };
}
