using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.TypeClass;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Liftable type-class
    /// <para>
    /// Can lift a value A into a 
    /// </para>
    /// </summary>
    /// <typeparam name="A">The type being described as a monoid</typeparam>
    [Typeclass]
    public interface Liftable<A>
    {
        /// <summary>
        /// Lift value A into a Liftable<A>
        /// <summary>
        Liftable<A> Lift(A x, params A[] xs);

        /// <summary>
        /// Lift value A into a Liftable<A>
        /// <summary>
        Liftable<A> Lift(IEnumerable<A> value);
    }
}
