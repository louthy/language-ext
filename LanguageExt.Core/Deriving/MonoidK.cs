using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// A monoid for higher-kinds
    /// </summary>
    /// <typeparam name="M">Higher kind</typeparam>
    public interface MonoidK<Supertype, Subtype> :
        MonoidK<Supertype>,
        SemigroupK<Supertype, Subtype>
        where Supertype : MonoidK<Supertype, Subtype>
        where Subtype : MonoidK<Subtype>
    {
        /// <summary>
        /// Identity
        /// </summary>
        static K<Supertype, A> MonoidK<Supertype>.Empty<A>() =>
            Supertype.CoTransform(Subtype.Empty<A>());
    }
}

