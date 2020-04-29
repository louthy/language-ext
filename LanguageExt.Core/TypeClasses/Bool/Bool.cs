using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Typeclass for things that have true and false values.
    /// </summary>
    [Typeclass("Bool*")]
    public interface Bool<A> : Typeclass
    {
        /// <summary>
        /// Returns True
        /// </summary>
        /// <returns>True</returns>
        [Pure]
        A True();

        /// <summary>
        /// Returns False
        /// </summary>
        /// <returns>False</returns>
        [Pure]
        A False();

        /// <summary>
        /// Returns the result of the logical AND operation between `a` and `b`
        /// </summary>
        /// <returns>The result of the logical AND operation between `a` and `b`</returns>
        [Pure]
        A And(A a, A b);

        /// <summary>
        /// Returns the result of the logical OR operation between `a` and `b`
        /// </summary>
        /// <returns>The result of the logical OR operation between `a` and `b`</returns>
        [Pure]
        A Or(A a, A b);

        /// <summary>
        /// Returns the result of the logical NOT operation on `a`
        /// </summary>
        /// <returns>The result of the logical NOT operation on `a`</returns>
        [Pure]
        A Not(A a);

        /// <summary>
        /// Returns the result of the logical exclusive-OR operation between `a` and `b`
        /// </summary>
        /// <returns>The result of the logical exclusive-OR operation between `a` and `b`</returns>
        [Pure]
        A XOr(A a, A b);

        /// <summary>
        /// Logical implication
        /// </summary>
        /// <returns>If `a` is true that implies `b`, else `true`</returns>
        [Pure]
        A Implies(A a, A b);

        /// <summary>
        /// Logical bi-conditional.  Both `a` and `b` must be `true`, or both `a` and `b` must
        /// be false.
        /// </summary>
        /// <returns>`true` if `a == b`, `false` otherwise</returns>
        [Pure]
        A BiCondition(A a, A b);
    }
}
