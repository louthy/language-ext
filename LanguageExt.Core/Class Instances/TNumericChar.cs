using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Integer number 
/// </summary>
public struct TNumericChar : Ord<char>, Monoid<char>, Arithmetic<char>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(char x, char y) =>
        x == y;

    /// <summary>
    /// Compare two values
    /// </summary>
    /// <param name="x">Left hand side of the compare operation</param>
    /// <param name="y">Right hand side of the compare operation</param>
    /// <returns>
    /// if x greater than y : 1
    /// 
    /// if x less than y    : -1
    /// 
    /// if x equals y       : 0
    /// </returns>
    [Pure]
    public static int Compare(char x, char y) =>
        CharToInt(x).CompareTo(CharToInt(y));

    /// <summary>
    /// Monoid empty value (0)
    /// </summary>
    /// <returns>0</returns>
    [Pure]
    public static char Empty() => (char)0;

    /// <summary>
    /// Semigroup append (sum)
    /// </summary>
    /// <param name="x">left hand side of the append operation</param>
    /// <param name="y">right hand side of the append operation</param>
    /// <returns>x + y</returns>
    [Pure]
    public static char Append(char x, char y) => 
        (char)(CharToInt(x) + CharToInt(y));

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(char x) =>
        x.GetHashCode();

    [Pure]
    public static char Plus(char x, char y) =>
        (char)(CharToInt(x) + CharToInt(y));

    [Pure]
    public static char Subtract(char x, char y) =>
        (char)(CharToInt(x) - CharToInt(y));

    [Pure]
    public static char Product(char x, char y) =>
        (char)(CharToInt(x) * CharToInt(y));

    [Pure]
    public static char Negate(char x) =>
        (char)(-CharToInt(x));

    static int CharToInt(int x) =>
        x > 32768
            ? -(65536 - x)
            : x;
}
