using LanguageExt.Attributes;
using LanguageExt.Common;

namespace LanguageExt.Effects.Traits;

/// <summary>
/// Type-class giving a struct the trait of supporting conversion of the
/// `Error` type to a type of `E` (user specified error type)
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
/// <typeparam name="E">User specified error type</typeparam>
[Trait("*")]
public interface HasFromError<out RT, out E>
    where RT : HasFromError<RT, E>
{
    /// <summary>
    /// Allows conversion from an `Error` type to `E`
    /// </summary>
    /// <param name="error">Error value to convert</param>
    /// <returns>A value of type `E`, usually an error representation</returns>
    public static abstract E FromError(Error error);
}
