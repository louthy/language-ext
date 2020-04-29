using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    [Typeclass("Choice*Unsafe")]
    public interface ChoiceUnsafe<CH, L, R> : Typeclass
    {
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
        C MatchUnsafe<C>(CH choice, Func<L, C> Left, Func<R, C> Right, Func<C> Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        Unit Match(CH choice, Action<L> Left, Action<R> Right, Action Bottom = null);
    }
}
