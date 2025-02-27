using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class Optional
{
    internal static readonly Action noneIgnore = () => { };
    internal static readonly Func<Unit> noneIgnoreF = () => unit;

    /// <summary>
    /// Invokes the `f` action if the optional is in a `Some` state, otherwise nothing happens.
    /// </summary>
    public static Unit ifSome<F, A>(K<F, A> fa, Action<A> f)
        where F : Optional<F> =>
        F.Match(fa, f, noneIgnore);

    /// <summary>
    /// Invokes the `f` function if the optional is in the `Some` state, otherwise nothing
    /// happens.
    /// </summary>
    public static Unit ifSome<F, A>(K<F, A> fa, Func<A, Unit> f)
        where F : Optional<F> =>
        F.Match(fa, f, noneIgnoreF);

    /// <summary>
    /// Returns the result of invoking the `None` operation if the optional 
    /// is in a `None` state, otherwise the bound `Some(x)` value is returned.
    /// </summary>
    /// <param name="None">Operation to invoke if the structure is in a None state</param>
    /// <returns>Result of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
    [Pure]
    public static A ifNone<F, A>(K<F, A> fa, Func<A> None)
        where F : Optional<F> =>
        F.Match(fa, identity, None);

    /// <summary>
    /// Returns the `noneValue` if the optional is in a `None` state, otherwise
    /// the bound `Some(x)` value is returned.
    /// </summary>
    /// <param name="noneValue">Value to return if in a None state</param>
    /// <returns>noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned</returns>
    [Pure]
    public static A ifNone<F, A>(K<F, A> fa, A noneValue)
        where F : Optional<F> =>
        F.Match(fa, identity, () => noneValue);

    /// <summary>
    /// Match operation with an untyped value for Some. This can be
    /// useful for serialisation and dealing with the IOptional interface
    /// </summary>
    /// <typeparam name="R">The return type</typeparam>
    /// <param name="Some">Operation to perform if the option is in a Some state</param>
    /// <param name="None">Operation to perform if the option is in a None state</param>
    /// <returns>The result of the match operation</returns>
    [Pure]
    public static R matchUntyped<F, A, R>(K<F, A> fa, Func<object?, R> Some, Func<R> None)
        where F : Optional<F> =>
        F.Match(fa, Some: x => Some(x), None: None);

    /// <summary>
    /// Convert the optional to an `Arr` of zero or one items
    /// </summary>
    /// <param name="fa">Option</param>
    /// <returns>An `Arr` of zero or one items</returns>
    [Pure]
    public static Arr<A> toArray<F, A>(K<F, A> fa)
        where F : Optional<F> =>
        F.Match(fa,
                Some: x => [x],
                None: Arr<A>.Empty);

    /// <summary>
    /// Convert the optional to an immutable list of zero or one items
    /// </summary>
    /// <param name="ma">Option</param>
    /// <returns>An immutable list of zero or one items</returns>
    [Pure]
    public static Lst<A> toLst<F, A>(K<F, A> fa)
        where F : Optional<F> =>
        F.Match(fa,
                Some: x => [x],
                None: Lst<A>.Empty);

    /// <summary>
    /// Convert the optional to an enumerable of zero or one items
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">optional</param>
    /// <returns>An enumerable of zero or one items</returns>
    [Pure]
    public static IEnumerable<A> asEnumerable<F, A>(K<F, A> fa)
        where F : Optional<F> =>
        toArray(fa);

    /// <summary>
    /// Convert the structure to an `Either`
    /// </summary>
    [Pure]
    public static Either<L, A> toEither<F, L, A>(K<F, A> fa, L defaultLeftValue)
        where F : Optional<F> =>
        F.Match(fa,
                Some: Right<L, A>,
                None: () => Left<L, A>(defaultLeftValue));

    /// <summary>
    /// Convert the structure to an `Either`
    /// </summary>
    [Pure]
    public static Either<L, A> toEither<F, L, A>(K<F, A> ma, Func<L> Left)
        where F : Optional<F> =>
        F.Match(ma,
                Some: Right<L, A>,
                None: () => Left<L, A>(Left()));

    /// <summary>
    /// Convert the structure to an `Option`
    /// </summary>
    [Pure]
    public static Option<A> toOption<F, A>(K<F, A> ma)
        where F : Optional<F> =>
        F.Match(ma,
                Some: Some,
                None: () => Option<A>.None);
}
