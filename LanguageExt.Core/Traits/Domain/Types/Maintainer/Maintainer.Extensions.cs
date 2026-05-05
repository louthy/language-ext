using System;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

/// <summary>
/// Extension methods for maintained domain sets.
/// </summary>
public static class MaintainerExtensions
{
    extension<A>(A)
        where A : Maintainer<A>
    {
        /// <summary>
        /// Finds the first maintained value that satisfies the predicate.
        /// </summary>
        public static Option<A> FindM(Func<A, bool> fa) =>
            Prelude.findM(fa);

        /// <summary>
        /// Finds the first maintained value that satisfies the predicate.
        /// </summary>
        public static A Find(Func<A, bool> fa) =>
            Prelude.find(fa);
    }

    extension<A>(A a)
        where A : Maintainer<A>
    {
        /// <summary>
        /// Checks whether this maintained value is equal to another value.
        /// </summary>
        public bool Is(A b) => a.Equals(b);

        /// <summary>
        /// Checks whether this maintained value is different from another value.
        /// </summary>
        public bool IsNot(A b) => !a.Equals(b);
    }
}
