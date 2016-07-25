using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface Optional<A>
    {
        /// <summary>
        /// True if the optional type allows nulls
        /// </summary>
        bool IsUnsafe(Optional<A> a);

        /// <summary>
        /// Is the option in a Some state
        /// </summary>
        bool IsSomeA(Optional<A> a);

        /// <summary>
        /// Is the option in a None state
        /// </summary>
        bool IsNoneA(Optional<A> a);

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="None">None handler.  Must not return null.</param>
        /// <returns>A non-null R</returns>
        [Pure]
        B Match<B>(Optional<A> a, Func<A, B> Some, Func<B> None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some handler.  May return null.</param>
        /// <param name="None">None handler.  May return null.</param>
        /// <returns>R, or null</returns>
        [Pure]
        B MatchUnsafe<B>(Optional<A> a, Func<A, B> Some, Func<B> None);
    }
}
