using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    public static class ObjectExt
    {
        /// <summary>
        /// Returns true if the value is equal to this type's
        /// default value.
        /// </summary>
        /// <example>
        ///     0.IsDefault()  // true
        ///     1.IsDefault()  // false
        /// </example>
        /// <returns>True if the value is equal to this type's
        /// default value</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefault<T>(this T value) =>
            Check<T>.IsDefault(value);

        /// <summary>
        /// Returns true if the value is null, and does so without
        /// boxing of any value-types.  Value-types will always
        /// return false.
        /// </summary>
        /// <example>
        ///     int x = 0;
        ///     string y = null;
        ///     
        ///     x.IsNull()  // false
        ///     y.IsNull()  // true
        /// </example>
        /// <returns>True if the value is null, and does so without
        /// boxing of any value-types.  Value-types will always
        /// return false.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this T value) =>
            Check<T>.IsNull(value);

        private static class Check<T>
        {
            static readonly bool IsReferenceType;
            static readonly bool IsNullable;
            static readonly EqualityComparer<T> DefaultEqualityComparer;

            static Check()
            {
                IsNullable = Nullable.GetUnderlyingType(typeof(T)) != null;
                IsReferenceType = !typeof(T).GetTypeInfo().IsValueType;
                DefaultEqualityComparer = EqualityComparer<T>.Default;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static bool IsDefault(T value) =>
                DefaultEqualityComparer.Equals(value, default(T));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static bool IsNull(T value) =>
                IsNullable
                    ? value.Equals(default(T))
                    : IsReferenceType && DefaultEqualityComparer.Equals(value, default(T));
        }
    }
}
