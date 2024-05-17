using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Floating point equality
/// </summary>
public struct EqDecimal : Eq<decimal>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(decimal a, decimal b) =>
        a == b;

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(decimal x) =>
        HashableDecimal.GetHashCode(x);

    [Pure]
    public static Task<bool> EqualsAsync(decimal x, decimal y) =>
        Equals(x, y).AsTask();

    [Pure]
    public static Task<int> GetHashCodeAsync(decimal x) => 
        GetHashCode(x).AsTask();    
}
