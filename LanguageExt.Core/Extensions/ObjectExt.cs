using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

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
            EqualityComparer<T>.Default.Equals(value, default(T));

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
            Prelude.isnull(value);
    }
}
