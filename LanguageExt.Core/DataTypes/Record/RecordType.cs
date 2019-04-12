using System;
using System.Runtime.Serialization;
using static LanguageExt.Prelude;

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
        public static Func<A, int> Hash => RecordTypeHash<A>.Hash;

        /// <summary>
        /// General equality function for record types
        /// </summary>
        public static Func<A, object, bool> Equality => RecordTypeEquality<A>.Equality;

        /// <summary>
        /// General typed equality function for record types
        /// </summary>
        public static Func<A, A, bool> EqualityTyped => RecordTypeEqualityTyped<A>.EqualityTyped;

        /// <summary>
        /// General typed comparison function for record types
        /// </summary>
        public static Func<A, A, int> Compare => RecordTypeCompare<A>.Compare;

        /// <summary>
        /// General ToString function
        /// </summary>
        public static new Func<A, string> ToString => RecordTypeToString<A>.ToString;

        /// <summary>
        /// De-serialise an A
        /// </summary>
        public static Action<A, SerializationInfo> SetObjectData => RecordTypeSetObjectData<A>.SetObjectData;

        /// <summary>
        /// Serialise an A
        /// </summary>
        public static Action<A, SerializationInfo> GetObjectData => RecordTypeGetObjectData<A>.GetObjectData;

        [Obsolete("Don't use Equals - use either RecordType<A>.Equality or RecordType<A>.EqualityTyped")]
        public static new bool Equals(object objA, object objB)
        {
            throw new InvalidOperationException("Don't use Equals - use either RecordType<A>.Equality or RecordType<A>.EqualityTyped");
        }
    }

    internal static class RecordTypeIncludeBase<A>
    {
        static RecordTypeIncludeBase()
        {
            IncludeBase = typeof(A).CustomAttributes
                                   .Exists(a => a.AttributeType.Name == nameof(IgnoreBaseAttribute))
                                   .Apply(not);
        }
        internal static readonly bool IncludeBase;
    }

    internal static class RecordTypeHash<A>
    {
        static RecordTypeHash()
        {
            Hash = IL.GetHashCode<A>(RecordTypeIncludeBase<A>.IncludeBase);
        }
        internal static readonly Func<A, int> Hash;
    }

    internal static class RecordTypeEquality<A>
    {
        static RecordTypeEquality()
        {
            Equality = IL.Equals<A>(RecordTypeIncludeBase<A>.IncludeBase);
        }
        internal static readonly Func<A, object, bool> Equality;
    }

    internal static class RecordTypeEqualityTyped<A>
    {
        static RecordTypeEqualityTyped()
        {
            EqualityTyped = IL.EqualsTyped<A>(RecordTypeIncludeBase<A>.IncludeBase);
        }
        internal static readonly Func<A, A, bool> EqualityTyped;
    }

    internal static class RecordTypeCompare<A>
    {
        static RecordTypeCompare()
        {
            Compare = IL.Compare<A>(RecordTypeIncludeBase<A>.IncludeBase);
        }
        internal static readonly Func<A, A, int> Compare;
    }

    internal static class RecordTypeToString<A>
    {
        static RecordTypeToString()
        {
            ToString = IL.ToString<A>(RecordTypeIncludeBase<A>.IncludeBase);
        }
        internal static new readonly Func<A, string> ToString;
    }

    internal static class RecordTypeSetObjectData<A>
    {
        static RecordTypeSetObjectData()
        {
            SetObjectData = IL.SetObjectData<A>(RecordTypeIncludeBase<A>.IncludeBase);
        }
        internal static readonly Action<A, SerializationInfo> SetObjectData;
    }

    internal static class RecordTypeGetObjectData<A>
    {
        static RecordTypeGetObjectData()
        {
            GetObjectData = IL.GetObjectData<A>(RecordTypeIncludeBase<A>.IncludeBase);
        }
        internal static readonly Action<A, SerializationInfo> GetObjectData;
    }
}
