using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Identifiable module
/// </summary>
public static partial class IdentifiableExtensions
{
    /// <typeparam name="F">Structure</typeparam>
    /// <typeparam name="L">Identifier type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    extension<F, L, A>(K<F, A> self)
        where F : Identifiable<F, L>
    {
        /// <summary>
        /// Identify the structure
        /// </summary>
        /// <param name="fa">Structure to label</param>
        /// <param name="label">Label to apply</param>
        /// <returns>Identified structure</returns>
        public static K<F, A> operator |(K<F, A> fa, Label<L> label) =>
            F.Identify(fa, label);        
    }
}
