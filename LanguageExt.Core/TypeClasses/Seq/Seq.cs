using System.Collections.Generic;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Sequence type-class
    /// Any type that can be converted to a stream of values (i.e. List,
    /// Option, etc.)
    /// </summary>
    public interface Seq<A>
    {
        /// <summary>
        /// Convert the structure to an IEnumerable<A>
        /// </summary>
        /// <returns>An IEnumerable that represents the strucure</returns>
        IEnumerable<A> ToSeq(Seq<A> sa);
    }
}
