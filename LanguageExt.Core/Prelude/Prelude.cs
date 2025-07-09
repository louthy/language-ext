using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Identity function
    /// </summary>
    [Pure]
    public static A identity<A>(A x) => 
        x;

    /// <summary>
    /// Constant function
    /// Always returns the first argument
    /// </summary>
    [Pure]
    public static Func<B, A> constant<A, B>(A x) =>
        _ => x;

    /// <summary>
    /// Constant function
    /// Always returns the first argument
    /// </summary>
    [Pure]
    public static A constant<A, B>(A x, B _) =>
        x;

    /// <summary>
    /// Constant function
    /// Always returns the first argument
    /// </summary>
    [Pure]
    public static Func<A, A> constantA<A>(A x) =>
        _ => x;

    /// <summary>
    /// Constant function
    /// Always returns the first argument
    /// </summary>
    [Pure]
    public static A constantA<A>(A x, A _) =>
        x;

    /// <summary>
    /// Raises a lazy Exception with the message provided
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <returns>Action that when executed throws</returns>
    public static Action failwith(string message) =>
        () => throw new Exception(message);

    /// <summary>
    /// Raises an Exception with the message provided
    /// </summary>
    /// <typeparam name="R">The return type of the expression this function is being used in.
    /// This allows exceptions to be thrown in ternary operators, or LINQ expressions for
    /// example</typeparam>
    /// <param name="message">Exception message</param>
    /// <returns>Throws an exception</returns>
    public static R failwith<R>(string message) =>
        throw new Exception(message);

    /// <summary>
    /// Raises an ApplicationException with the message provided
    /// </summary>
    /// <typeparam name="R">The return type of the expression this function is being used in.
    /// This allows exceptions to be thrown in ternary operators, or LINQ expressions for
    /// example</typeparam>
    /// <param name="message">ApplicationException message</param>
    /// <returns>Throws an ApplicationException</returns>
    public static R raiseapp<R>(string message) =>
        throw new ApplicationException(message);

    /// <summary>
    /// Raise an exception
    /// </summary>
    /// <typeparam name="R">The return type of the expression this function is being used in.
    /// This allows exceptions to be thrown in ternary operators, or LINQ expressions for
    /// example</typeparam>
    /// <param name="ex">Exception to throw</param>
    /// <returns>Throws an exception</returns>
    public static R raise<R>(Exception ex) =>
        ex.Rethrow<R>();

    /// <summary>
    /// Identifies an exception as being of type E
    /// </summary>
    /// <typeparam name="E">Type to match</typeparam>
    /// <param name="e">Exception to test</param>
    /// <returns>True if e is of type E</returns>
    [Pure]
    public static bool exceptionIs<E>(Exception e)
    {
        if (e is E) return true;
        if (e.InnerException == null) return false;
        return exceptionIs<E>(e.InnerException);
    }

    /// <summary>
    /// Not function, for prettifying code and removing the need to 
    /// use the ! operator.
    /// </summary>
    /// <param name="f">Predicate function to perform the not operation on</param>
    /// <returns>!f</returns>
    public static Func<A, bool> not<A>(Func<A, bool> f) => 
        x => !f(x);

    /// <summary>
    /// Not function, for prettifying code and removing the need to 
    /// use the ! operator.
    /// </summary>
    /// <param name="f">Predicate function to perform the not operation on</param>
    /// <returns>!f</returns>
    public static Func<A, B, bool> not<A, B>(Func<A, B, bool> f) => 
        (x, y) => !f(x, y);

    /// <summary>
    /// Not function, for prettifying code and removing the need to 
    /// use the ! operator.
    /// </summary>
    /// <param name="f">Predicate function to perform the not operation on</param>
    /// <returns>!f</returns>
    public static Func<A, B, C, bool> not<A, B, C>(Func<A, B, C, bool> f) => 
        (x, y, z) => !f(x, y, z);

    /// <summary>
    /// Not function, for prettifying code and removing the need to 
    /// use the ! operator.
    /// </summary>
    /// <param name="value">Value to perform the not operation on</param>
    /// <returns>!value</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool not(bool value) =>
        !value;

    /// <summary>
    /// Returns true if the value is equal to this type's
    /// default value.
    /// </summary>
    /// <example>
    ///     isDefault(0)  // true
    ///     isDefault(1)  // false
    /// </example>
    /// <returns>True if the value is equal to this type's
    /// default value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    #nullable disable
    public static bool isDefault<A>(A value) =>
        Check<A>.IsDefault(value);
    #nullable restore

    /// <summary>
    /// Returns true if the value is not equal to this type's
    /// default value.
    /// </summary>
    /// <example>
    ///     notDefault(0)  // false
    ///     notDefault(1)  // true
    /// </example>
    /// <returns>True if the value is not equal to this type's
    /// default value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    #nullable disable
    public static bool notDefault<A>(A value) =>
        !Check<A>.IsDefault(value);
    #nullable restore

    /// <summary>
    /// Returns true if the value is null, and does so without
    /// boxing of any value-types.  Value-types will always
    /// return false.
    /// </summary>
    /// <example>
    ///     int x = 0;
    ///     string y = null;
    ///     
    ///     isnull(x)  // false
    ///     isnull(y)  // true
    /// </example>
    /// <returns>True if the value is null, and does so without
    /// boxing of any value-types.  Value-types will always
    /// return false.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    #nullable disable
    public static bool isnull<A>(A value) =>
        Check<A>.IsNull(value);
    #nullable restore

    /// <summary>
    /// Returns true if the value is not null, and does so without
    /// boxing of any value-types.  Value-types will always return 
    /// true.
    /// </summary>
    /// <example>
    ///     int x = 0;
    ///     string y = null;
    ///     string z = "Hello";
    ///     
    ///     notnull(x)  // true
    ///     notnull(y)  // false
    ///     notnull(z)  // true
    /// </example>
    /// <returns>True if the value is null, and does so without
    /// boxing of any value-types.  Value-types will always
    /// return false.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    #nullable disable
    public static bool notnull<A>(A value) =>
        !Check<A>.IsNull(value);
    #nullable restore

    /// <summary>
    /// Convert a value to string
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string toString<A>(A value) =>
        value?.ToString() ?? "";

    /// <summary>
    /// Returns true if the string is not null, nor empty, nor a whitespace
    /// </summary>
    /// <param name="value">String to test</param>
    /// <returns>True if the string is null, empty, or whitespace</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool notEmpty(string? value) =>
        !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Returns true if the string is null, empty, or whitespace
    /// </summary>
    /// <param name="value">String to test</param>
    /// <returns>True if the string is null, empty, or whitespace</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool isEmpty([NotNullWhen(false)] string? value) =>
        string.IsNullOrWhiteSpace(value);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (A1, B) mapFirst<A, B, A1>(Func<A, A1> f, (A, B) pair) =>
        (f(pair.Item1), pair.Item2);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (A, B1) mapSecond<A, B, B1>(Func<B, B1> f, (A, B) pair) =>
        (pair.Item1, f(pair.Item2));

}
