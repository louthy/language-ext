#nullable enable
using System;

namespace LanguageExt;

public static class Sum
{
    public static readonly Sum<Unit, Unit> RightUnit = new SumRight<Unit, Unit>(default);
    public static readonly Sum<Unit, Unit> LeftUnit = new SumLeft<Unit, Unit>(default);
}

/// <summary>
/// Sum-type.  Represents either a value of type `A` or a value of type `X`.
/// </summary>
/// <remarks>Isomorphic to `Either<L, R>`</remarks>
/// <typeparam name="X">Alternative value type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public abstract record Sum<X, A>
{
    /// <summary>
    /// Returns `true` if the sum-value is in a `Left` state
    /// </summary>
    public abstract bool IsLeft { get; }

    /// <summary>
    /// Returns `true` if the sum-value is in a `Right` state
    /// </summary>
    public abstract bool IsRight { get; }

    /// <summary>
    /// Returns the value of the sum-type is in a `Right` state, otherwise throws
    /// </summary>
    internal abstract A RightUnsafe { get; }

    /// <summary>
    /// Returns the value of the sum-type is in a `Left` state, otherwise throws
    /// </summary>
    internal abstract X LeftUnsafe { get; }

    /// <summary>
    /// Constructor of `SumLeft<X, A>`
    /// </summary>
    /// <param name="value"></param>
    /// <returns>`Sum<X, A>`</returns>
    public static Sum<X, A> Left(X value) => new SumLeft<X, A>(value);
    
    /// <summary>
    /// Constructor of `SumRight<X, A>`
    /// </summary>
    /// <param name="value"></param>
    /// <returns>`Sum<X, A>`</returns>
    public static Sum<X, A> Right(A value) => new SumRight<X, A>(value);

    /// <summary>
    /// Functor map
    /// </summary>
    /// <param name="f">Function that maps the bound value</param>
    /// <typeparam name="B">Type of the bound value post-mapping</typeparam>
    /// <returns>Mapped functor</returns>
    public abstract Sum<X, B> Map<B>(Func<A, B> f);

    /// <summary>
    /// Functor bi-map
    /// </summary>
    public abstract Sum<Y, B> BiMap<B, Y>(Func<X, Y> Left, Func<A, B> Right);

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Function that maps the bound value</param>
    /// <typeparam name="B">Type of the bound value post-mapping</typeparam>
    /// <returns>Mapped monad</returns>
    public abstract Sum<X, B> Bind<B>(Func<A, Sum<X, B>> f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Function that maps the bound value</param>
    /// <typeparam name="B">Type of the bound value post-mapping</typeparam>
    /// <returns>Mapped monad</returns>
    public virtual Sum<X, B> SelectMany<B>(Func<A, Sum<X, B>> f) =>
        Bind(f);

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="f">Function that maps the bound value</param>
    /// <typeparam name="B">Type of the bound value post-mapping</typeparam>
    /// <returns>Mapped monad</returns>
    public virtual Sum<X, C> SelectMany<B, C>(Func<A, Sum<X, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Casts the sum-type's bound value-type.  If the sum-type is in a `Right` state then this will throw
    /// </summary>
    public virtual Sum<X, B> Cast<B>() =>
        Map<B>(static _ => throw new InvalidCastException());
}

/// <summary>
/// Left (alternative) case of the `SumType` union
/// </summary>
/// <param name="Value">Value of the case</param>
/// <typeparam name="X">Alternative value type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public record SumLeft<X, A>(X Value) : Sum<X, A>
{
    /// <summary>
    /// Returns `true` if the sum-value is in a `Left` state
    /// </summary>
    public override bool IsLeft => 
        true;

    /// <summary>
    /// Returns `true` if the sum-value is in a `Right` state
    /// </summary>
    public override bool IsRight => 
        false;

    /// <summary>
    /// Returns the value of the sum-type is in a `Right` state, otherwise throws
    /// </summary>
    internal override A RightUnsafe =>
        throw new InvalidOperationException();

    /// <summary>
    /// Returns the value of the sum-type is in a `Left` state, otherwise throws
    /// </summary>
    internal override X LeftUnsafe =>
        Value;

    /// <summary>
    /// Functor map
    /// </summary>
    /// <param name="f">Function that maps the bound value</param>
    /// <typeparam name="B">Type of the bound value post-mapping</typeparam>
    /// <returns>Mapped functor</returns>
    public override Sum<X, B> Map<B>(Func<A, B> f) =>
        new SumLeft<X, B>(Value);

    /// <summary>
    /// Functor bi-map
    /// </summary>
    public override Sum<Y, B> BiMap<B, Y>(Func<X, Y> Left, Func<A, B> Right) =>
        new SumLeft<Y, B>(Left(Value));

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Function that maps the bound value</param>
    /// <typeparam name="B">Type of the bound value post-mapping</typeparam>
    /// <returns>Mapped monad</returns>
    public override Sum<X, B> Bind<B>(Func<A, Sum<X, B>> f) =>
        new SumLeft<X, B>(Value);

    public override string ToString() =>
        $"Left({Value})";
}

/// <summary>
/// Right (primary) case of the `SumType` union
/// </summary>
/// <param name="Value">Value of the case</param>
/// <typeparam name="X">Alternative value type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public record SumRight<X, A>(A Value) : Sum<X, A>
{
    /// <summary>
    /// Returns `true` if the sum-value is in a `Left` state
    /// </summary>
    public override bool IsLeft => 
        false;

    /// <summary>
    /// Returns `true` if the sum-value is in a `Right` state
    /// </summary>
    public override bool IsRight => 
        true;

    /// <summary>
    /// Returns the value of the sum-type is in a `Right` state, otherwise throws
    /// </summary>
    internal override A RightUnsafe =>
        Value;

    /// <summary>
    /// Returns the value of the sum-type is in a `Left` state, otherwise throws
    /// </summary>
    internal override X LeftUnsafe =>
        throw new InvalidOperationException();
 
    /// <summary>
    /// Functor map
    /// </summary>
    /// <param name="f">Function that maps the bound value</param>
    /// <typeparam name="B">Type of the bound value post-mapping</typeparam>
    /// <returns>Mapped functor</returns>
    public override Sum<X, B> Map<B>(Func<A, B> f) =>
        new SumRight<X, B>(f(Value));

    /// <summary>
    /// Functor bi-map
    /// </summary>
    public override Sum<Y, B> BiMap<B, Y>(Func<X, Y> Left, Func<A, B> Right) =>
        new SumRight<Y, B>(Right(Value));

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Function that maps the bound value</param>
    /// <typeparam name="B">Type of the bound value post-mapping</typeparam>
    /// <returns>Mapped monad</returns>
    public override Sum<X, B> Bind<B>(Func<A, Sum<X, B>> f) =>
        f(Value);

    public override string ToString() =>
        $"Right({Value})";
}
