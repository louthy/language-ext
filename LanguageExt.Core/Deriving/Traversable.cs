using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Functors representing data structures that can be transformed to structures of the same
    /// shape by performing an `Applicative` (or, therefore, `Monad`) action on each element from
    /// left to right.
    ///
    /// A more detailed description of what same shape means, the various methods, how traversals
    /// are constructed, and example advanced use-cases can be found in the Overview section of Data.Traversable.
    /// </summary>
    public interface Traversable<Supertype, Subtype> : 
        Functor<Supertype, Subtype>, 
        Foldable<Supertype, Subtype>,
        Traversable<Supertype>
        where Supertype : 
            Traversable<Supertype, Subtype>, 
            Traversable<Supertype>
        where Subtype :
            Traversable<Subtype>
    {
        static K<F, K<Supertype, B>> Traversable<Supertype>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Supertype, A> ta) => 
            Subtype.Traverse(f, Supertype.Transform(ta)).Map(Supertype.CoTransform);

        static K<M, K<Supertype, B>> Traversable<Supertype>.TraverseM<M, A, B>(Func<A, K<M, B>> f, K<Supertype, A> ta) => 
            Subtype.TraverseM(f, Supertype.Transform(ta)).Map(Supertype.CoTransform);

        static K<F, K<Supertype, A>> Traversable<Supertype>.Sequence<F, A>(K<Supertype, K<F, A>> ta) => 
            Subtype.Sequence(Supertype.Transform(ta)).Map(Supertype.CoTransform);

        static K<F, K<Supertype, A>> Traversable<Supertype>.SequenceM<F, A>(K<Supertype, K<F, A>> ta) => 
            Subtype.SequenceM(Supertype.Transform(ta)).Map(Supertype.CoTransform);

        static K<F, K<Supertype, B>> Traversable<Supertype>.TraverseDefault<F, A, B>(Func<A, K<F, B>> f, K<Supertype, A> ta) => 
            Subtype.TraverseDefault(f, Supertype.Transform(ta)).Map(Supertype.CoTransform);
    }
}
