using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public static partial class ModuleT<MP, E, S, T, M>
    where MP : MonadParsecT<MP, E, S, T, M>
    where S : TokenStream<S, T>
    where M : Monad<M>
{
    /// <summary>
    /// The parser `token(test, expected)` accepts tokens for which the
    /// matching function `test` returns `Some` result. If `None` is
    /// returned, the `expected` set is used to report the items that were
    /// expected.
    /// </summary>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <param name="test">Token predicate test function</param>
    /// <param name="expected">Expected items</param>
    /// <returns>Token parser</returns>
    public static K<MP, A> token<A>(Func<T, Option<A>> test, in Set<ErrorItem<T>> expected) => 
        MP.Token(test, expected);
    
    /// <summary>
    /// The parser `tokens(test, chunk)` parses a chunk of input and returns it.
    /// The supplied predicate `test` is used to check equality of given and parsed
    /// chunks after a candidate chunk of correct length is fetched from the stream.
    /// 
    /// This can be used, for example, to write 'chunk':
    /// 
    ///     chunk = tokens (==)
    /// 
    /// Note that this is an auto-backtracking primitive, which means that if it
    /// fails, it never consumes any input. This is done to make its consumption
    /// model match how error messages for this primitive are reported (which
    /// becomes an important thing as user gets more control with primitives like
    /// 'withRecovery'):
    /// 
    ///     parseTest((string("abc")), "abd")
    /// 
    ///     unexpected "abd"
    ///     expecting "abc"
    /// 
    /// This means that it's not necessary to use `@try` with `tokens`-based parsers,
    /// such as `string` and. 
    /// </summary>
    /// <param name="test">Predicate test function. The first argument is the chunk to
    /// test, the second argument is the reference chunk.</param>
    /// <param name="chunk">Reference chunk</param>
    /// <returns>Parsed chunk</returns>
    public static K<MP, S> tokens(Func<S, S, bool> test, S chunk) =>
        MP.Tokens(test, chunk);
    
    /// <summary>
    /// Parse zero or more tokens for which the supplied predicate holds.
    /// Try to use this as much as possible because, for many streams, this
    /// combinator is much faster than parsers built with `many` and `satisfy`.
    /// 
    ///     takeWhile((Some "foo"), f) = many (satisfy(f) | "foo")
    ///     takeWhile(None,         f) = many (satisfy(f))
    /// 
    /// The combinator never fails, although it may parse the empty chunk.
    /// </summary>
    /// <param name="test">Predicate to use to test tokens</param>
    /// <param name="name">Name for a single token in the row</param>
    /// <returns>A chunk of matching tokens</returns>
    public static K<MP, S> takeWhile(Func<T, bool> test, Option<string> name = default) =>
        MP.TakeWhile(test, name);
    
    /// <summary>
    /// Parse one or more tokens for which the supplied predicate holds.
    /// Try to use this as much as possible because, for many streams, this
    /// combinator is much faster than parsers built with `many` and `satisfy`.
    /// 
    ///     takeWhile((Some "foo"), f) = many (satisfy(f) | "foo")
    ///     takeWhile(None,         f) = many (satisfy(f))
    /// 
    /// The combinator never fails, although it may parse the empty chunk.
    /// </summary>
    /// <param name="test">Predicate to use to test tokens</param>
    /// <param name="name">Name for a single token in the row</param>
    /// <returns>A chunk of matching tokens</returns>
    public static K<MP, S> takeWhile1(Func<T, bool> test, Option<string> name = default) =>
        MP.TakeWhile1(test, name);

    /// <summary>
    /// Extract the specified number of tokens from the input stream and
    /// return them packed as a chunk of stream. If there are not enough tokens
    /// in the stream, a parse error will be signalled. It's guaranteed that if
    /// the parser succeeds, the requested number of tokens will be returned.
    /// 
    /// The parser is roughly equivalent to:
    /// 
    ///     take((Just "foo"), n) = count(n, (anySingle | "foo"))
    ///     take(Nothing,      n) = count(n, anySingle)
    /// 
    /// Note that if the combinator fails due to an insufficient number of tokens
    /// in the input stream, it backtracks automatically. No `@try` is necessary
    /// with `take`.
    /// </summary>
    /// <param name="n">How many tokens to extract</param>
    /// <param name="name">Name for a single token in the row</param>
    /// <returns>A chunk of matching tokens</returns>
    public static K<MP, S> take(int n, Option<string> name = default) => 
        MP.Take(n, name);
}
