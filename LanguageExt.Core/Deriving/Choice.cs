using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// A semigroup on applicative functors
    /// </summary>
    public interface Choice<Supertype, Subtype> :
        Choice<Supertype>,
        Traits.Natural<Supertype, Subtype>,
        Traits.CoNatural<Supertype, Subtype>
        where Supertype : Choice<Supertype, Subtype>
        where Subtype : Choice<Subtype>
    {
        /// <summary>
        /// Where `Supertype` defines some notion of failure or choice, this function picks
        /// the first argument that succeeds.  So, if `fa` succeeds, then `fa` is returned;
        /// if it fails, then `fb` is returned.
        /// </summary>
        /// <param name="fa">First structure to test</param>
        /// <param name="fb">Second structure to return if the first one fails</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>First argument to succeed</returns>
        static K<Supertype, A> Choice<Supertype>.Choose<A>(K<Supertype, A> fa, K<Supertype, A> fb) =>
            Supertype.CoTransform(Subtype.Choose(Supertype.Transform(fa), Supertype.Transform(fb)));

        /// <summary>
        /// Where `Supertype` defines some notion of failure or choice, this function picks
        /// the first argument that succeeds.  So, if `fa` succeeds, then `fa` is returned;
        /// if it fails, then `fb` is returned.
        /// </summary>
        /// <param name="fa">First structure to test</param>
        /// <param name="fb">Second structure to return if the first one fails</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>First argument to succeed</returns>
        static Memo<Supertype, A> Choice<Supertype>.Choose<A>(K<Supertype, A> fa, Memo<Supertype, A> fb) =>
            Memo.cotransform<Supertype, Subtype, A>(
                Subtype.Choose(Supertype.Transform(fa), Memo.transform<Supertype, Subtype, A>(fb)));
    }
}
