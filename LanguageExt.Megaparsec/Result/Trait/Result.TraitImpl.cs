using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
/// Result trait implementation
/// </summary>
/// <typeparam name="T">Token type</typeparam>
/// <typeparam name="E">Error type</typeparam>
public class Result<T, E> : Functor<Result<T, E>>
{
    static K<Result<T, E>, B> Functor<Result<T, E>>.Map<A, B>(Func<A, B> f, K<Result<T, E>, A> ma) =>
        ma switch
        {
            Result<T, E, A>.OK (var hints, var value) => Result.OK<T, E, B>(hints, f(value)),
            Result<T, E, A>.Error (var e)             => Result.Error<T, E, B>(e),
            _                                         => throw new NotSupportedException()
        };
}
