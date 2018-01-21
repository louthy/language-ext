﻿using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface Optional<OA, A> : Typeclass
    {
        /// <summary>
        /// True if the optional type allows nulls
        /// </summary>
        [Pure]
        bool IsUnsafe(OA opt);

        /// <summary>
        /// Is the option in a Some state
        /// </summary>
        [Pure]
        bool IsSome(OA opt);

        /// <summary>
        /// Is the option in a None state
        /// </summary>
        [Pure]
        bool IsNone(OA opt);

        /// <summary>
        /// Match the two states of the Option and return a non-null B.
        /// </summary>
        [Pure]
        B Match<B>(OA opt, Func<A, B> Some, Func<B> None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        [Pure]
        B MatchUnsafe<B>(OA opt, Func<A, B> Some, Func<B> None);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        Unit Match(OA opt, Action<A> Some, Action None);

        [Pure]
        OA None { get; }

        [Pure]
        OA Some(A value);

        [Pure]
        OA Optional(A value);
    }
}
