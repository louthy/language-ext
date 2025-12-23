using System;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Simple data type that helps indicate whether a recursive function should loop or not.
/// </summary>
/// <remarks>
/// This was created to pair with the `Monad` trait `Tail` function and is extremely lightweight.
/// </remarks>
/// <remarks>
/// It's designed to cause no additional allocations when used in a tail-recursive manner.  That does mean there
/// are some footguns in here if you're not careful.  So, make sure that before you access `Loop` or `Done`
/// that you've confirmed the state of the structure by using `IsLoop` or `IsDone`.
///
/// You can then use C#'s pattern-matching to extract the value from the structure:
///
///     var result = next switch
///     {
///         { IsLoop: true, Loop: var value } => ...,
///         { IsDone: true, Done: var value } => ...,
///         _                                 => throw new Exception("Invalid state")   
///     };
/// </remarks>
/// <remarks>
/// If we ever get real struct discriminated unions, then this can be replaced with that.
/// </remarks>
/// <typeparam name="A">Loop value type</typeparam>
/// <typeparam name="B">Done value type</typeparam>
public readonly struct Next<A, B>
{
    readonly int state;
    readonly A left;
    readonly B right;

    internal Next(int state, A left, B right)
    {
        this.state = state;
        this.left = left;
        this.right = right;
    }

    /// <summary>
    /// Pattern match
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public C Match<C>(Func<A, C> Loop, Func<B, C> Done) =>
        IsLoop 
            ? Loop(left) 
            : IsDone
                ? Done(right)
                : throw new BottomException();

    public Next<A, C> Map<C>(Func<B, C> f) =>
        state switch
        {
            1 => new Next<A, C>(state, left, default!),
            2 => new Next<A, C>(state, left, f(right)),
            _ => throw new BottomException()
        };
    
    public Either<A, B> ToEither() =>
        state switch
        {
            1 => Left(left),
            2 => Right(right),
            _ => throw new BottomException()
        };
    
    /// <summary>
    /// Returns true if we should loop
    /// </summary>
    public bool IsLoop
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => state == 1;
    }

    /// <summary>
    /// Returns true if we should complete and return
    /// </summary>
    public bool IsDone
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => state == 2;
    }

    /// <summary>
    /// Try to access the loop-value type
    /// </summary>
    /// <returns>Either a valid `A` value or will throw an exception</returns>
    /// <exception cref="InvalidCastException">Throws if the structure is in a done state</exception>
    public A Loop
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => state == 1
                   ? left
                   : throw new InvalidCastException();
    }

    /// <summary>
    /// Try to access the done-value type
    /// </summary>
    /// <returns>Either a valid `B` value or will throw an exception</returns>
    /// <exception cref="InvalidCastException">Throws if the structure is in a done state</exception>
    public B Done
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => state == 2
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
        next.Loop;

    /// <summary>
    /// Explicit cast to the complete value type
    /// </summary>
    /// <param name="next">Next structure</param>
    /// <returns>Either a valid `A` value or will throw an exception</returns>
    /// <exception cref="InvalidCastException">Throws if the structure is in a done state</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator B(Next<A, B> next) =>
        next.Done;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Next<A, B>(Loop<A> value) =>
        Next.Loop<A, B>(value.Value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Next<A, B>(Pure<B> value) =>
        Next.Done<A, B>(value.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Next<A, B>(Either<A, B> value) =>
        value switch
        {
            Either<A, B>.Left(var l)  => Next.Loop<A, B>(l),
            Either<A, B>.Right(var r) => Next.Done<A, B>(r),
            _                         => throw new BottomException()
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Either<A, B>(Next<A, B> value) =>
        value.ToEither();
    
}
