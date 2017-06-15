using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface Choice<CH, L, R>
    {
        /// <summary>
        /// True if the choice type allows nulls
        /// </summary>
        [Pure]
        bool IsUnsafe(CH choice);

        /// <summary>
        /// Is the choice in the first state
        /// </summary>
        [Pure]
        bool IsLeft(CH choice);

        /// <summary>
        /// Is the choice in the second state
        /// </summary>
        [Pure]
        bool IsRight(CH choice);

        /// <summary>
        /// Is the choice in the bottom
        /// </summary>
        [Pure]
        bool IsBottom(CH choice);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        [Pure]
        C Match<C>(CH choice, Func<L, C> Left, Func<R, C> Right, Func<C> Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        Unit Match(CH choice, Action<L> Left, Action<R> Right, Action Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a B, which can be null.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        [Pure]
        C MatchUnsafe<C>(CH choice, Func<L, C> Left, Func<R, C> Right, Func<C> Bottom = null);
    }
}