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
public interface HasIO<out RT, out E> : HasCancel<RT>, HasFromError<RT, E>, HasSyncContext<RT>
    where RT : struct, 
        HasCancel<RT>, 
        HasSyncContext<RT>,
        HasFromError<RT, E>
{
}
