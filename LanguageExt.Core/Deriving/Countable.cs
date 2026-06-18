using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public interface Countable<Supertype, Subtype> :
        Countable<Supertype>,
        Traits.Natural<Supertype, Subtype>,
        Traits.CoNatural<Supertype, Subtype>
        where Supertype : Countable<Supertype>, Countable<Supertype, Subtype>
        where Subtype : Countable<Subtype>
    {

        static K<Sub, A> transform<Super, Sub, A>(K<Super, A> ta) 
            where Super : Traits.Natural<Super, Sub> =>
            Super.Transform(ta);

        /// <summary>
        /// Returns the size/length of a finite structure as an `int`.  The
        /// default implementation just counts elements starting with the leftmost.
        /// 
        /// Instances for structures that can compute the element count faster
        /// than via element-by-element counting, should provide a specialised
        /// implementation.
        /// </summary>
        static long Countable<Supertype>.Count<A>(K<Supertype, A> ta) =>
            Subtype.Count(transform<Supertype, Subtype, A>(ta));
    }
}
