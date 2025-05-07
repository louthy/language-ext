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
        Monad<Supertype, Subtype>,
        MonadIO<Supertype>
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
        static K<Supertype, A> Traits.MonadIO<Supertype>.LiftIO<A>(K<IO, A> ma) =>
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
        static K<Supertype, A> Traits.MonadIO<Supertype>.LiftIO<A>(IO<A> ma) =>
            Supertype.CoTransform(Subtype.LiftIO(ma));
    }
}
