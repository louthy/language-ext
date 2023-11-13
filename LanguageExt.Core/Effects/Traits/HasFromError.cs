#nullable  enable
using LanguageExt.Attributes;
using LanguageExt.Common;

namespace LanguageExt.Effects.Traits;

/// <summary>
/// Type-class giving a struct the trait of supporting conversion of the
/// `Error` type to a type of `E` (user specified error type)
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
/// <typeparam name="E">User specified error type</typeparam>
[Typeclass("*")]
public interface HasFromError<out RT, out E>
    where RT : struct, HasFromError<RT, E>
{
    /// <summary>
    /// Allows conversion from an `Error` type to `E`
    /// </summary>
    /// <param name="error">Error value to convert</param>
    /// <returns>A value of type `E`, usually an error representation</returns>
    E FromError(Error error);
}
