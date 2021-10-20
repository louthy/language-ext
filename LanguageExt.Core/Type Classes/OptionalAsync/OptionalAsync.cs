using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    [Typeclass("Opt*Async")]
    public interface OptionalAsync<OA, A> : Typeclass
    {
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
        Task<B> MatchAsync<B>(OA opt, Func<A, Task<B>> SomeAsync, Func<B> None);

        /// <summary>
        /// Match the two states of the Option and return a non-null B.
        /// </summary>
        [Pure]
        Task<B> MatchAsync<B>(OA opt, Func<A, B> Some, Func<Task<B>> NoneAsync);

        /// <summary>
        /// Match the two states of the Option and return a non-null B.
        /// </summary>
        [Pure]
        Task<B> MatchAsync<B>(OA opt, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        Task<Unit> Match(OA opt, Action<A> Some, Action None);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="SomeAsync">Async Some match operation</param>
        /// <param name="None">None match operation</param>
        Task<Unit> MatchAsync(OA opt, Func<A, Task> SomeAsync, Action None);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="SomeAsync">Async Some match operation</param>
        /// <param name="NoneAsync">Async None match operation</param>
        Task<Unit> MatchAsync(OA opt, Func<A, Task> SomeAsync, Func<Task> NoneAsync);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="NoneAsync">Async None match operation</param>
        Task<Unit> MatchAsync(OA opt, Action<A> Some, Func<Task> NoneAsync);

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
