using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

public interface Optional<F> : Applicative<F>
    where F : Applicative<F>, Optional<F>
{
    /// <summary>
    /// Is the optional in `Some` state
    /// </summary>
    [Pure]
    public static virtual bool IsSome<A>(K<F, A> fa) =>
        F.Match(fa, Some: true, None: false);

    /// <summary>
    /// Is the optional in `None` state
    /// </summary>
    [Pure]
    public static virtual bool IsNone<A>(K<F, A> fa) =>
        F.Match(fa, Some: false, None: true);

    /// <summary>
    /// Match the two states of the optional and return a non-null B.
    /// </summary>
    [Pure]
    public static abstract B Match<A, B>(K<F, A> fa, Func<A, B> Some, Func<B> None);

    /// <summary>
    /// Match the two states of the Option and return a non-null B.
    /// </summary>
    [Pure]
    public static virtual B Match<A, B>(K<F, A> fa, Func<A, B> Some, B None) =>
        F.Match(fa, Some, () => None);

    /// <summary>
    /// Match the two states of the Option and return a non-null B.
    /// </summary>
    [Pure]
    public static virtual B Match<A, B>(K<F, A> fa, B Some, B None) =>
        F.Match(fa, _ => Some, () => None);

    /// <summary>
    /// Match the two states of the Option A
    /// </summary>
    /// <param name="Some">Some match operation</param>
    /// <param name="None">None match operation</param>
    public static virtual Unit Match<A>(K<F, A> fa, Action<A> Some, Action None) =>
        F.Match<A, Unit>(fa, Some: x => { Some(x); return default; }, None: () => { None(); return default; });

    [Pure] 
    public static abstract K<F, A> None<A>();

    [Pure]
    public static virtual K<F, A> Some<A>(A value) =>
        F.Pure(value);
}
