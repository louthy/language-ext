using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface Choice<CH, A, B>
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
        bool IsChoice1(CH choice);

        /// <summary>
        /// Is the choice in the second state
        /// </summary>
        [Pure]
        bool IsChoice2(CH choice);

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
        C Match<C>(CH choice, Func<A, C> Choice1, Func<B, C> Choice2, Func<C> Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        Unit Match(CH choice, Action<A> Choice1, Action<B> Choice2, Action Bottom = null);

        /// <summary>
        /// Match the two states of the Choice and return a B, which can be null.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        [Pure]
        C MatchUnsafe<C>(CH choice, Func<A, C> Choice1, Func<B, C> Choice2, Func<C> Bottom = null);
    }
}
