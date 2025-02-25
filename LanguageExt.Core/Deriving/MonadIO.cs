using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derived `MonadIO` implementation
    /// </summary>
    /// <typeparam name="Supertype">Super-type wrapper around the subtype</typeparam>
    /// <typeparam name="Subtype">The subtype that the supertype type 'wraps'</typeparam>
    public interface MonadIO<Supertype, Subtype> :
        MonadIO<Supertype>,
        Natural<Supertype, Subtype>,
        CoNatural<Supertype, Subtype>
        where Subtype : MonadIO<Subtype>, Monad<Subtype>
        where Supertype : MonadIO<Supertype, Subtype>, Monad<Supertype>
    {
        /// <summary>
        /// Lift an IO operation into the `Self` monad
        /// </summary>
        /// <param name="ma">IO structure to lift</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Monad with an `IO` structure lifted into it</returns>
        /// <exception cref="ExceptionalException">If this method isn't overloaded in
        /// the inner monad or any monad in the stack on the way to the inner monad
        /// then it will throw an exception.</exception>
        static K<Supertype, A> MonadIO<Supertype>.LiftIO<A>(K<IO, A> ma) => 
            Supertype.CoTransform(Subtype.LiftIO(ma));

        /// <summary>
        /// Lift an IO operation into the `Self` monad
        /// </summary>
        /// <param name="ma">IO structure to lift</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Monad with an `IO` structure lifted into it</returns>
        /// <exception cref="ExceptionalException">If this method isn't overloaded in
        /// the inner monad or any monad in the stack on the way to the inner monad
        /// then it will throw an exception.</exception>
        static K<Supertype, A> MonadIO<Supertype>.LiftIO<A>(IO<A> ma) =>
            Supertype.CoTransform(Subtype.LiftIO(ma));

        /// <summary>
        /// Extract the inner `IO` monad from the `Self` structure provided
        /// </summary>
        /// <param name="ma">`Self` structure to extract the `IO` monad from</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Self` structure with the `IO` structure as the bound value</returns>
        static K<Supertype, IO<A>> MonadIO<Supertype>.ToIO<A>(K<Supertype, A> ma) => 
            Supertype.CoTransform(Supertype.Transform(ma).ToIO());

        /// <summary>
        /// Map the inner `IO` monad within the `Self` structure provided
        /// </summary>
        /// <param name="ma">`Self` structure to extract the `IO` monad from</param>
        /// <param name="f">`IO` structure mapping function</param>
        /// <typeparam name="A">Input bound value type</typeparam>
        /// <typeparam name="B">Output bound value type</typeparam>
        /// <returns>`Self` structure that has had its inner `IO` monad mapped</returns>
        /// <exception cref="ExceptionalException">If this method isn't overloaded in
        /// the inner monad or any monad in the stack on the way to the inner monad
        /// then it will throw an exception.</exception>
        static K<Supertype, B> MonadIO<Supertype>.MapIO<A, B>(K<Supertype, A> ma, Func<IO<A>, IO<B>> f) => 
            Supertype.CoTransform(Supertype.Transform(ma).MapIO(f));
    }
}
