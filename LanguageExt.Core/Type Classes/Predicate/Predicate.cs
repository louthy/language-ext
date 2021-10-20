using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Predicate type-class
    /// </summary>
    /// <typeparam name="A">Type of value to run the predication operation against</typeparam>
    [Typeclass("Pred*")]
    public interface Pred<A> : Typeclass
    {
        /// <summary>
        /// The predicate operation.  Returns true if the source value
        /// fits the predicate.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        bool True(A value);
    }
}
