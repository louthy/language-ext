using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class Prelude
    {

        /// <summary>
        /// Represents a successful operation
        /// </summary>
        /// <typeparam name="MonoidError">Monoid for collecting the errors</typeparam>
        /// <typeparam name="ERROR">Error type</typeparam>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Validation applicative</returns>
        public static Validation<MonoidError, ERROR, A> Success<MonoidError, ERROR, A>(A value)
            where MonoidError : struct, Monoid<ERROR>, Eq<ERROR> =>
            Validation<MonoidError, ERROR, A>.Success(value);

        /// <summary>
        /// Represents a failed operation
        /// </summary>
        /// <typeparam name="MonoidError">Monoid for collecting the errors</typeparam>
        /// <typeparam name="ERROR">Error type</typeparam>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="value">Error value</param>
        /// <returns>Validation applicative</returns>
        public static Validation<MonoidError, ERROR, A> Fail<MonoidError, ERROR, A>(ERROR value)
            where MonoidError : struct, Monoid<ERROR>, Eq<ERROR> =>
            Validation<MonoidError, ERROR, A>.Fail(value);
    }
}
