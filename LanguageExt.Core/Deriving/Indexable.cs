using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public interface Indexable<Supertype, Subtype, in Ix> :
        Indexable<Supertype, Ix>,
        Traits.Natural<Supertype, Subtype>,
        Traits.CoNatural<Supertype, Subtype>
        where Supertype : Indexable<Supertype, Ix>, Indexable<Supertype, Subtype, Ix>
        where Subtype : Indexable<Subtype, Ix>
    {
        /// <summary>
        /// Find the element at the specified index or `None` if out of range
        /// </summary>
        static Option<A> Indexable<Supertype, Ix>.At<A>(Ix index, K<Supertype, A> ta) => 
            Subtype.At(index, Supertype.Transform(ta));
    }
}
