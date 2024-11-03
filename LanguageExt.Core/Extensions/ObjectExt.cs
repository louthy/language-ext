using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public static class ObjectExt
{
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
    public static bool IsNull<A>([System.Diagnostics.CodeAnalysis.NotNullWhen(false)]this A? value) =>
        value is null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsDefault<A>(A value) =>
        EqDefault<A>.Equals(value, default!);
}