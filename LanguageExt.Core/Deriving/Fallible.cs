using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Trait for higher-kinded structures that have a failure state `E`
    /// </summary>
    public interface Fallible<E, Supertype, Subtype> : 
        Traits.Fallible<E, Supertype>
        where Supertype :
            Fallible<E, Supertype, Subtype>, 
            Traits.Fallible<E, Supertype>,
            Natural<Supertype, Subtype>,
            CoNatural<Supertype, Subtype>
        where Subtype : Traits.Fallible<E, Subtype>
    {
        /// <summary>
        /// Raise a failure state in the `Fallible` structure `F`
        /// </summary>
        /// <param name="error">Error value</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns></returns>
        static K<Supertype, A> Traits.Fallible<E, Supertype>.Fail<A>(E error) =>
            Supertype.CoTransform(Subtype.Fail<A>(error));

        /// <summary>
        /// Run the `Fallible` structure.  If in a failed state, test the failure value
        /// against the predicate.  If, it returns `true`, run the `Fail` function with
        /// the failure value.
        /// </summary>
        /// <param name="fa">`Fallible` structure</param>
        /// <param name="Predicate">Predicate to test any failure values</param>
        /// <param name="Fail">Handler when in failed state</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
        /// predicate returns true for the failure value</returns>
        static K<Supertype, A> Traits.Fallible<E, Supertype>.Catch<A>(
            K<Supertype, A> fa,
            Func<E, bool> Predicate,
            Func<E, K<Supertype, A>> Fail) =>
            Supertype.CoTransform(Supertype.Transform(fa).Catch(Predicate, e => Supertype.Transform(Fail(e))));
    }

    /// <summary>
    /// Trait for higher-kinded structures that have a failure state `E`
    /// </summary>
    public interface Fallible<Supertype, Subtype> : 
        Fallible<Error, Supertype, Subtype> 
        where Supertype : 
            Fallible<Error, Supertype, Subtype>, 
            Natural<Supertype, Subtype>, 
            CoNatural<Supertype, Subtype>
        where Subtype : Traits.Fallible<Error, Subtype>;
}
