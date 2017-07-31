using System;

namespace LanguageExt
{
    /// <summary>
    /// Supports the building of record types (classes with sets of readonly field values)
    /// Provides structural equality testing, ordering, and hashing
    /// </summary>
    /// <typeparam name="A">Type to provide methods for</typeparam>
    public static class RecordType<A>
    {
        /// <summary>
        /// General hash code function for record types
        /// </summary>
        public static readonly Func<A, int> Hash =
            IL.GetHashCode<A>();

        /// <summary>
        /// General equality function for record types
        /// </summary>
        public static readonly Func<A, object, bool> Equality = 
            IL.Equals<A>();

        /// <summary>
        /// General typed equality function for record types
        /// </summary>
        public static readonly Func<A, A, bool> EqualityTyped =
            IL.EqualsTyped<A>();

        /// <summary>
        /// General typed comparison function for record types
        /// </summary>
        public static readonly Func<A, A, int> Compare =
            IL.Compare<A>();

        /// <summary>
        /// General ToString function
        /// </summary>
        public static readonly Func<A, string> AsString =
            IL.ToString<A>();

        [Obsolete("Don't use Equals - use either RecordType<A>.Equality or RecordType<A>.EqualityTyped")]
        public new static bool Equals(object objA, object objB)
        {
            throw new InvalidOperationException("Don't use Equals - use either RecordType<A>.Equality or RecordType<A>.EqualityTyped");
        }
    }
}
