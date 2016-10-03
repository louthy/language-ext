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
        /// Match the two states of the Option and return a non-null B.
        /// </summary>
        [Pure]
        B Match<B>(Optional<A> a, Func<A, B> Some, Func<B> None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        [Pure]
        B MatchUnsafe<B>(Optional<A> a, Func<A, B> Some, Func<B> None);
    }
}
