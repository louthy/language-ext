using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Used by `@catch`, `@exceptional`, `@expected` to represent the catching of errors
/// </summary>
public readonly record struct CatchValue<A>(Func<Error, bool> Match, Func<Error, A> Value)
{
    public CatchValue<Error, A> As() =>
        new (Match, Value);
}

/// <summary>
/// Used by `@catch`, `@exceptional`, `@expected` to represent the catching of errors
/// </summary>
public readonly record struct CatchValue<E, A>(Func<E, bool> Match, Func<E, A> Value);

/// <summary>
/// Used by `@catch`, `@exceptional`, `@expected` to represent the catching of errors
/// </summary>
public readonly record struct CatchError(Func<Error, bool> Match, Func<Error, Error> Value)
{
    public CatchError<Error> As() =>
        new (Match, Value);
}

/// <summary>
/// Used by `@catch`, `@exceptional`, `@expected` to represent the catching of errors
/// </summary>
public readonly record struct CatchError<E>(Func<E, bool> Match, Func<E, E> Value);

/// <summary>
/// Used by `@catch`, `@exceptional`, `@expected` to represent the catching of errors
/// </summary>
public readonly record struct CatchIO<A>(Func<Error, bool> Match, Func<Error, IO<A>> Value)
{
    public CatchIO<Error, A> As() =>
        new (Match, Value);
}

/// <summary>
/// Used by `@catch`, `@exceptional`, `@expected` to represent the catching of errors
/// </summary>
public readonly record struct CatchIO<E, A>(Func<E, bool> Match, Func<E, IO<A>> Value);

/// <summary>
/// Used by `@catch`, `@exceptional`, `@expected` to represent the catching of errors
/// </summary>
public readonly record struct CatchM<M, A>(Func<Error, bool> Match, Func<Error, K<M, A>> Value)
{
    public K<M, A> Run(Error error, K<M, A> otherwise) =>
        Match(error) ? Value(error) : otherwise;
}

/// <summary>
/// Used by `@catch`, `@exceptional`, `@expected` to represent the catching of errors
/// </summary>
public readonly record struct CatchM<M, E, A>(Func<E, bool> Match, Func<E, K<M, A>> Value)
{
    public K<M, A> Run(E error, K<M, A> otherwise) =>
        Match(error) ? Value(error) : otherwise;
}

