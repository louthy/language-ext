using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derives `finally` in a `try/finally` operation
    /// </summary>
    public interface Final<Supertype, Subtype> : 
        Final<Supertype>,
        Natural<Supertype, Subtype>,
        CoNatural<Supertype, Subtype>
        where Supertype : Final<Supertype>, Final<Supertype, Subtype>
        where Subtype : Final<Subtype>
    {
        /// <summary>
        /// Run a `finally` operation after the `fa` operation regardless of whether `fa` succeeds or not.
        /// </summary>
        /// <param name="fa">Primary operation</param>
        /// <param name="finally">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        static K<Supertype, A> Final<Supertype>.Finally<X, A>(K<Supertype, A> fa, K<Supertype, X> @finally) =>
            Supertype.CoTransform(Subtype.Finally(Supertype.Transform(fa), Supertype.Transform(@finally)));
    }
}
