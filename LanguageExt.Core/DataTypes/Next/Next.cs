using System;
using System.Runtime.CompilerServices;

namespace LanguageExt;

/// <summary>
/// Simple data type that helps indicate whether a recursive function should continue or not.
/// </summary>
/// <remarks>
/// This was created to pair with the `Monad` trait `Tail` function and is extremely lightweight.
/// </remarks>
/// <remarks>
/// It's designed to cause no additional allocations when used in a tail-recursive manner.  That does mean there
/// are some footguns in here if you're not careful.  So, make sure that before you access `ContValue` or `DoneValue`
/// that you've confirmed the state of the structure by using `IsCont` or `IsDone`.
///
/// You can then use C#'s pattern-matching to extract the value from the structure:
///
///     var result = next switch
///     {
///         { IsCont: true, ContValue: var value } => ...,
///         { IsDone: true, DoneValue: var value } => ...,
///         _                                      => throw new Exception("Invalid state")   
///     };
/// </remarks>
/// <remarks>
/// If we ever get real struct discriminated unions, then this can be replaced with that.
/// </remarks>
/// <typeparam name="A">Continue value type</typeparam>
/// <typeparam name="B">Complete value type</typeparam>
public readonly struct Next<A, B>
{
    readonly int cont;
    readonly A left;
    readonly B right;

    internal Next(int cont, A left, B right)
    {
        this.cont = cont;
        this.left = left;
        this.right = right;
    }

    /// <summary>
    /// Returns true if we should continue
    /// </summary>
    public bool IsCont
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => cont == 1;
    }

    /// <summary>
    /// Returns true if we should complete and return
    /// </summary>
    public bool IsDone
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => cont == 2;
    }

    /// <summary>
    /// Try to access the continue-value type
    /// </summary>
    /// <returns>Either a valid `A` value or will throw an exception</returns>
    /// <exception cref="InvalidCastException">Throws if the structure is in a done state</exception>
    public A ContValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => cont == 1
                   ? left
                   : throw new InvalidCastException();
    }

    /// <summary>
    /// Try to access the done-value type
    /// </summary>
    /// <returns>Either a valid `B` value or will throw an exception</returns>
    /// <exception cref="InvalidCastException">Throws if the structure is in a done state</exception>
    public B DoneValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => cont == 2
                   ? right
                   : throw new InvalidCastException();
    }

    /// <summary>
    /// Explicit cast to the complete value type
    /// </summary>
    /// <param name="next">Next structure</param>
    /// <returns>Either a valid `A` value or will throw an exception</returns>
    /// <exception cref="InvalidCastException">Throws if the structure is in a done state</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator A(Next<A, B> next) =>
        next.ContValue;

    /// <summary>
    /// Explicit cast to the complete value type
    /// </summary>
    /// <param name="next">Next structure</param>
    /// <returns>Either a valid `A` value or will throw an exception</returns>
    /// <exception cref="InvalidCastException">Throws if the structure is in a done state</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator B(Next<A, B> next) =>
        next.DoneValue;
}
