using System.Numerics;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `Num<A>` trait for `INumber<A>` 
/// </summary>
/// <typeparam name="A"></typeparam>
public class Number<A> : Num<A>
    where A : INumber<A>
{
    public static int GetHashCode(A x) =>
        x.GetHashCode();

    public static bool Equals(A x, A y) =>
        x == y;

    public static int Compare(A x, A y) =>
        x.CompareTo(y);

    public static A Add(A x, A y) =>
        x + y;

    public static A Subtract(A x, A y) =>
        x - y;

    public static A Multiply(A x, A y) =>
        x * y;

    public static A Negate(A x) =>
        -x;

    public static A Abs(A x) =>
        x < A.Zero ? -x : x;

    public static A Signum(A x) =>
        x < A.Zero
            ? -A.One
            : x > A.Zero
                ? A.One
                : A.Zero;

    public static A FromInteger(int x) =>
        A.CreateChecked(x);

    public static A FromDecimal(decimal x) => 
        A.CreateChecked(x);

    public static A FromFloat(float x) => 
        A.CreateChecked(x);

    public static A FromDouble(double x) => 
        A.CreateChecked(x);

    public static A Divide(A x, A y) =>
        x / y;
}
