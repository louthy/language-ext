using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derived monad-transformer implementation
    /// </summary>
    /// <typeparam name="Supertype">Super-type wrapper around the subtype</typeparam>
    /// <typeparam name="Subtype">The subtype that the supertype type 'wraps'</typeparam>
    public interface MonadT<Supertype, Subtype, out M> :
        Monad<Supertype, Subtype>,
        MonadT<Supertype, M>
        where Subtype : MonadT<Subtype, M>
        where Supertype : MonadT<Supertype, Subtype, M>, MonadT<Supertype, M>
        where M : Monad<M>
    {
        static K<Supertype, A> MonadT<Supertype, M>.Lift<A>(K<M, A> ma) => 
            Supertype.CoTransform(Subtype.Lift(ma));
    }
}
