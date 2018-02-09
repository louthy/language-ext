﻿using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface OptionalAsync<OA, A> : Typeclass
    {
        /// <summary>
        /// True if the optional type allows nulls
        /// </summary>
        [Pure]
        Task<bool> IsUnsafe(OA opt);

        /// <summary>
        /// Is the option in a Some state
        /// </summary>
        [Pure]
        Task<bool> IsSome(OA opt);

        /// <summary>
        /// Is the option in a None state
        /// </summary>
        [Pure]
        Task<bool> IsNone(OA opt);

        /// <summary>
        /// Match the two states of the Option and return a non-null B.
        /// </summary>
        [Pure]
        Task<B> Match<B>(OA opt, Func<A, B> Some, Func<B> None);

        /// <summary>
        /// Match the two states of the Option and return a non-null B.
        /// </summary>
        [Pure]
        Task<B> MatchAsync<B>(OA opt, Func<A, Task<B>> Some, Func<B> None);

        /// <summary>
        /// Match the two states of the Option and return a non-null B.
        /// </summary>
        [Pure]
        Task<B> MatchAsync<B>(OA opt, Func<A, B> Some, Func<Task<B>> None);

        /// <summary>
        /// Match the two states of the Option and return a non-null B.
        /// </summary>
        [Pure]
        Task<B> MatchAsync<B>(OA opt, Func<A, Task<B>> Some, Func<Task<B>> None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        [Pure]
        Task<B> MatchUnsafe<B>(OA opt, Func<A, B> Some, Func<B> None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        [Pure]
        Task<B> MatchUnsafeAsync<B>(OA opt, Func<A, Task<B>> Some, Func<B> None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        [Pure]
        Task<B> MatchUnsafeAsync<B>(OA opt, Func<A, B> Some, Func<Task<B>> None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        [Pure]
        Task<B> MatchUnsafeAsync<B>(OA opt, Func<A, Task<B>> Some, Func<Task<B>> None);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        Task<Unit> Match(OA opt, Action<A> Some, Action None);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        Task<Unit> MatchAsync(OA opt, Func<A, Task> Some, Action None);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        Task<Unit> MatchAsync(OA opt, Func<A, Task> Some, Func<Task> None);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        Task<Unit> MatchAsync(OA opt, Action<A> Some, Func<Task> None);

        [Pure]
        OA None { get; }

        [Pure]
        OA Some(A value);

        [Pure]
        OA SomeAsync(Task<A> value);

        [Pure]
        OA Optional(A value);

        [Pure]
        OA OptionalAsync(Task<A> value);
    }
}
