using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface Choice<A, B>
    {
        /// <summary>
        /// True if the choice type allows nulls
        /// </summary>
        bool IsUnsafe(Choice<A, B> a);

        /// <summary>
        /// Is the choice in the first state
        /// </summary>
        bool IsChoice1(Choice<A, B> a);

        /// <summary>
        /// Is the choice in the second state
        /// </summary>
        bool IsChoice2(Choice<A, B> a);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        [Pure]
        C Match<C>(Choice<A, B> a, Func<A, C> Choice1, Func<B, C> Choice2);

        /// <summary>
        /// Match the two states of the Choice and return a B, which can be null.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        [Pure]
        C MatchUnsafe<C>(Choice<A, B> a, Func<A, C> Choice1, Func<B, C> Choice2);
    }
}
