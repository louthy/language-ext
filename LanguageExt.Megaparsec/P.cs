using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
/// Arrow kind for parsers
/// </summary>
/// <typeparam name="E">Error type</typeparam>
/// <typeparam name="S">Token stream type</typeparam>
/// <typeparam name="T">Token type</typeparam>
/// <typeparam name="M">Self-type</typeparam>
/// <typeparam name="A">Value to parse type</typeparam>
public interface P<E, S, T, in M, A> : K<M, A>
    where M : MonadParsec<E, S, T, M>
    where S : TokenStream<S, T>;
