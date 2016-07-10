using System;
using System.Linq;
using System.Reactive.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Extension methods for Option
    /// By using extension methods we can check for null references in 'this'
    /// </summary>
    internal static class OptionVExtensions
    {
        /// <summary>
        /// Test the option state
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">OptionV</param>
        /// <returns>True if the Option is in a None state</returns>
        [Pure]
        internal static bool IsNone<A>(this OptionV<A> ma) =>
            isnull(ma) || ma is None<A>;

        /// <summary>
        /// Test the option state
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">OptionV</param>
        /// <returns>True if the Option is in a Some state</returns>
        [Pure]
        internal static bool IsSome<A>(this OptionV<A> ma) =>
            !IsNone(ma);

        /// <summary>
        /// Get the value from the Some state
        /// </summary>
        [Pure]
        internal static A Value<A>(this OptionV<A> self) =>
            (self as SomeValue<A>).Value;
    }
}