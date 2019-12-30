using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    [Typeclass("Choice*UnsafeAsync")]
    public interface ChoiceUnsafeAsync<CH, L, R> : Typeclass
    {
        /// <summary>
        /// Is the choice in the first state
        /// </summary>
        [Pure]
        Task<bool> IsLeft(CH choice);

        /// <summary>
        /// Is the choice in the second state
        /// </summary>
        [Pure]
        Task<bool> IsRight(CH choice);

        /// <summary>
        /// Is the choice in the bottom
        /// </summary>
        [Pure]
        Task<bool> IsBottom(CH choice);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        [Pure]
        Task<C> MatchUnsafe<C>(CH choice, Func<L, C> Left, Func<R, C> Right, Func<C> Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        [Pure]
        Task<C> MatchUnsafeAsync<C>(CH choice, Func<L, Task<C>> LeftAsync, Func<R, C> Right, Func<C> Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        [Pure]
        Task<C> MatchUnsafeAsync<C>(CH choice, Func<L, C> Left, Func<R, Task<C>> RightAsync, Func<C> Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        [Pure]
        Task<C> MatchUnsafeAsync<C>(CH choice, Func<L, Task<C>> LeftAsync, Func<R, Task<C>> RightAsync, Func<C> Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        Task<Unit> MatchUnsafe(CH choice, Action<L> Left, Action<R> Right, Action Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        Task<Unit> MatchUnsafeAsync(CH choice, Func<L, Task> LeftAsync, Action<R> Right, Action Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        Task<Unit> MatchUnsafeAsync(CH choice, Action<L> Left, Func<R, Task> RightAsync, Action Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        Task<Unit> MatchUnsafeAsync(CH choice, Func<L, Task> LeftAsync, Func<R, Task> RightAsync, Action Bottom = null);
    }
}
