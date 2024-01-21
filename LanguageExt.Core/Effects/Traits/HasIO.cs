#nullable  enable
using LanguageExt.Attributes;
using LanguageExt.Common;

namespace LanguageExt.Effects.Traits;

/// <summary>
/// Minimal collection of traits for IO operations
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
/// <typeparam name="E">User specified error type</typeparam>
[Typeclass("*")]
public interface HasIO<out RT> : HasCancel<RT>, HasSyncContext<RT>
    where RT : struct,
    HasCancel<RT>,
    HasSyncContext<RT>;

/// <summary>
/// Minimal collection of traits for IO operations
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
/// <typeparam name="E">User specified error type</typeparam>
[Typeclass("*")]
public interface HasIO<out RT, out E> : HasIO<RT>, HasFromError<RT, E>
    where RT : struct,
    HasCancel<RT>,
    HasSyncContext<RT>,
    HasFromError<RT, E>;
