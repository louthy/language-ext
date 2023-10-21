using System;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Represents a successful operation
        /// </summary>
        /// <typeparam name="ERROR">Error type</typeparam>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Validation applicative</returns>
        public static Validation<ERROR, A> Success<ERROR, A>(A value) =>
            Validation<ERROR, A>.Success(value);

        /// <summary>
        /// Represents a failed operation
        /// </summary>
        /// <typeparam name="ERROR">Error type</typeparam>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="value">Error value</param>
        /// <returns>Validation applicative</returns>
        public static Validation<ERROR, A> Fail<ERROR, A>(ERROR value) =>
            Validation<ERROR, A>.Fail(Seq1(value));

        /// <summary>
        /// Represents a failed operation
        /// </summary>
        /// <typeparam name="ERROR">Error type</typeparam>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="value">Error value</param>
        /// <returns>Validation applicative</returns>
        public static Validation<ERROR, A> Fail<ERROR, A>(Seq<ERROR> values) =>
            Validation<ERROR, A>.Fail(values);
    }
}
