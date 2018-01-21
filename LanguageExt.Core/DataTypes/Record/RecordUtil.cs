using System;
using System.Runtime.Serialization;

namespace LanguageExt
{
    /// <summary>
    /// Supports the building of record types (classes with sets of readonly field values)
    /// Provides structural equality testing, ordering, and hashing
    /// </summary>
    /// <typeparam name="A">Type to provide methods for</typeparam>
    public static class RecordType<A>
    {
        static RecordType()
        {
            Hash = IL.GetHashCode<A>();
            Equality = IL.Equals<A>();
            EqualityTyped = IL.EqualsTyped<A>();
            Compare = IL.Compare<A>();
            ToString = IL.ToString<A>();
            SetObjectData = IL.SetObjectData<A>();
            GetObjectData = IL.GetObjectData<A>();
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
