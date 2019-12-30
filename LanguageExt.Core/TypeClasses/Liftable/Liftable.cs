using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Liftable type-class
    /// <para>
    /// Can lift a value A into a 
    /// </para>
    /// </summary>
    /// <typeparam name="A">The type to be lifted</typeparam>
    [Typeclass("Lift*")]
    public interface Liftable<LA, A> : Typeclass
    {
        /// <summary>
        /// Lift value A into a Liftable<A>
        /// <summary>
        [Pure]
        LA Lift(A x);
    }
}
