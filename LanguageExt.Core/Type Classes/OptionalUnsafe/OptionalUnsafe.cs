using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

[Trait("Opt*Unsafe")]
public interface OptionalUnsafe<OA, A> : Trait
{
    /// <summary>
    /// Is the option in a Some state
    /// </summary>
    [Pure]
    public static abstract bool IsSome(OA opt);

    /// <summary>
    /// Is the option in a None state
    /// </summary>
    [Pure]
    public static abstract bool IsNone(OA opt);

    /// <summary>
    /// Match the two states of the Option and return a B, which can be null.
    /// </summary>
    [Pure]
    public static abstract B? MatchUnsafe<B>(OA opt, Func<A?, B?> Some, Func<B?> None);

    /// <summary>
    /// Match the two states of the Option and return a B, which can be null.
    /// </summary>
    [Pure]
    public static abstract B? MatchUnsafe<B>(OA opt, Func<A?, B?> Some, B? None);

    /// <summary>
    /// Match the two states of the Option A
    /// </summary>
    /// <param name="Some">Some match operation</param>
    /// <param name="None">None match operation</param>
    public static abstract Unit Match(OA opt, Action<A?> Some, Action None);

    [Pure]
    public static abstract OA None { get; }

    [Pure]
    public static abstract OA Some(A? value);
}
