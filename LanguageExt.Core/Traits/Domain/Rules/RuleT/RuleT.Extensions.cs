using System;
using LanguageExt.Common;

namespace LanguageExt.Traits.Domain;

public static partial class RuleTExtensions
{
    extension<SELF, T, M, A>(SELF)
        where SELF : RuleT<SELF, T, M, A>, new()
        where T : MonadT<T, M>
        where M : Monad<M>
    {
        /// <summary>
        /// Validates the specified value using the rule, providing full access to the rule instance
        /// and the value when constructing the error.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// A function that receives the rule instance and the invalid value to produce an <see cref="Error"/>.
        /// </param>
        /// <returns>
        /// A successful <see cref="FinT{T, A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="FinT{T, A}"/> with the generated error.
        /// </returns>
        public static FinT<T, A> ValidateT(K<M, A> value, Func<SELF, A, K<T, Error>> Fail) =>
            SELF.ValidateT(value, Fail);

        /// <summary>
        /// Validates the specified value using the rule, providing the invalid value
        /// to construct the error.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// A function that receives the invalid value and returns an <see cref="Error"/>.
        /// </param>
        /// <returns>
        /// A successful <see cref="FinT{M, A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="FinT{M, A}"/> with the generated error.
        /// </returns>
        public static FinT<T, A> ValidateT(K<M, A> value, Func<A, K<T, Error>> Fail) =>
            SELF.ValidateT(value, (_, v) => Fail(v));

        /// <summary>
        /// Validates the specified value using the rule, providing a parameterless error factory.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// A function that returns an <see cref="Error"/> when validation fails.
        /// </param>
        /// <returns>
        /// A successful <see cref="FinT{M, A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="FinT{M, A}"/> with the generated error.
        /// </returns>
        public static FinT<T, A> ValidateT(K<M, A> value, Func<K<T, Error>> Fail) =>
            SELF.ValidateT(value, (_, _) => Fail());

        /// <summary>
        /// Validates the specified value using the rule, returning a constant error
        /// when validation fails.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="Fail">
        /// The <see cref="Error"/> to return when validation fails.
        /// </param>
        /// <returns>
        /// A successful <see cref="FinT{M, A}"/> containing the value when valid;
        /// otherwise, a failed <see cref="FinT{M, A}"/> with the provided error.
        /// </returns>
        public static FinT<T, A> ValidateT(K<M, A> value, K<T, Error> Fail) =>
            SELF.ValidateT(value, (_, _) => Fail);

    }

}
