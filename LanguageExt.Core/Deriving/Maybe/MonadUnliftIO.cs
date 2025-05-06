/*
using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public static partial class Maybe
    {
        /// <summary>
        /// Derived `MonadIO` implementation
        /// </summary>
        /// <typeparam name="Supertype">Super-type wrapper around the subtype</typeparam>
        /// <typeparam name="Subtype">The subtype that the supertype type 'wraps'</typeparam>
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public interface MonadUnliftIO<Supertype, Subtype> :
            MonadIO<Supertype, Subtype>,
            MonadUnliftIO<Supertype>
            where Subtype : Traits.Maybe.MonadUnliftIO<Subtype>, MonadIO<Subtype>
            where Supertype : MonadUnliftIO<Supertype, Subtype>, 
                              MonadIO<Supertype>
        {
            /// <summary>
            /// Extract the inner `IO` monad from the `Self` structure provided
            /// </summary>
            /// <param name="ma">`Self` structure to extract the `IO` monad from</param>
            /// <typeparam name="A">Bound value type</typeparam>
            /// <returns>`Self` structure with the `IO` structure as the bound value</returns>
            static K<Supertype, IO<A>> Traits.Maybe.MonadUnliftIO<Supertype>.ToIO<A>(K<Supertype, A> ma) =>
                Supertype.CoTransform(Subtype.ToIO(Supertype.Transform(ma)));

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
            static K<Supertype, B> Traits.Maybe.MonadUnliftIO<Supertype>.MapIO<A, B>(K<Supertype, A> ma, Func<IO<A>, IO<B>> f) =>
                Supertype.CoTransform(Subtype.MapIO(Supertype.Transform(ma), f));
        }
    }
}
*/
