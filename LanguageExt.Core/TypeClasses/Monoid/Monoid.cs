using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Monoid type-class
    /// <para>
    /// A monoid is a type with an identity and an associative binary operation.
    /// </para>
    /// </summary>
    /// <typeparam name="A">The type being described as a monoid</typeparam>
    [Typeclass]
    public interface Monoid<A> : Semigroup<A>
    {
        /// <summary>
        /// Identity
        /// <summary>
        [Pure]
        A Empty();

        // Concat is defined in Monoid.Prelude
    }
}
