using System;
using System.Runtime.Serialization;

namespace LanguageExt
{
    /// <summary>
    /// Supports the building of record types (classes with sets of readonly field values)
    /// Provides structural equality testing, ordering, and hashing
    /// </summary>
    /// <typeparam name="A">Type to provide methods for</typeparam>
    public static class RecordTypeIgnoreBase<A>
    {
        static RecordTypeIgnoreBase()
        {
            Hash = IL.GetHashCode<A>(false);
            Equality = IL.Equals<A>(false);
            EqualityTyped = IL.EqualsTyped<A>(false);
            Compare = IL.Compare<A>(false);
            ToString = IL.ToString<A>(false);
            SetObjectData = IL.SetObjectData<A>(false);
            GetObjectData = IL.GetObjectData<A>(false);
        }

        /// <summary>
        /// General hash code function for record types
        /// </summary>
        public static readonly Func<A, int> Hash;

        /// <summary>
        /// General equality function for record types
        /// </summary>
        public static readonly Func<A, object, bool> Equality;

        /// <summary>
        /// General typed equality function for record types
        /// </summary>
        public static readonly Func<A, A, bool> EqualityTyped;

        /// <summary>
        /// General typed comparison function for record types
        /// </summary>
        public static readonly Func<A, A, int> Compare;

        /// <summary>
        /// General ToString function
        /// </summary>
        public new static readonly Func<A, string> ToString;

        /// <summary>
        /// De-serialise an A
        /// </summary>
        public static readonly Action<A, SerializationInfo> SetObjectData;

        /// <summary>
        /// Serialise an A
        /// </summary>
        public static readonly Action<A, SerializationInfo> GetObjectData;

        [Obsolete("Don't use Equals - use either RecordType<A>.Equality or RecordType<A>.EqualityTyped")]
        public new static bool Equals(object objA, object objB)
        {
            throw new InvalidOperationException("Don't use Equals - use either RecordType<A>.Equality or RecordType<A>.EqualityTyped");
        }
    }
}
