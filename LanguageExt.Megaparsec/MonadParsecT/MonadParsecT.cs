using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
/// Parser combinator monad transformer trait 
/// </summary>
/// <remarks>
/// Type class describing monads that implement the full set of primitive parsers.
/// </remarks>
/// <remarks>
/// **Note** that the following primitives are “fast” and should be taken
/// advantage of as much as possible if your aim is a fast parser: `tokens`,
/// `takeWhileP`, `takeWhile1P`, and `takeP`
/// </remarks>
/// <typeparam name="MP">This type</typeparam>
/// <typeparam name="E">Error type</typeparam>
/// <typeparam name="S">Token-stream type</typeparam>
/// <typeparam name="T">Token type</typeparam>
/// <typeparam name="M">Lifted monad</typeparam>
public interface MonadParsecT<MP, E, S, T, M> : 
    MonadT<MP, M>,
    Alternative<MP>, 
    Identifiable<MP, string>,
    Fallible<ParseError<T, E>, MP>,
    Readable<MP, State<S, T, E>>, 
    Stateful<MP, State<S, T, E>> 
    where MP : MonadParsecT<MP, E, S, T, M>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    /// <summary>
    /// Stop parsing and report the `ParseError`. This is the only way to
    /// control the position of the error without manipulating the parser state
    /// manually.
    /// </summary>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <param name="error">Error</param>
    /// <returns></returns>
    public static abstract K<MP, A> Error<A>(ParseError<T, E> error);

    /// <summary>
    /// The parser `label(name, p)` behaves as parser `p`, but whenever the
    /// parser `p` fails /without consuming any input/, it replaces names of
    /// “expected” tokens with the name `name`.
    /// </summary>
    /// <param name="name">Label name</param>
    /// <param name="p">Parser to label</param>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <returns>Parser</returns>
    public static abstract K<MP, A> Label<A>(string name, K<MP, A> p);

    /// <summary>
    /// `hidden(p)` behaves just like parser `p`, but it doesn't show any
    /// “expected” tokens in the error-message when `p` fails.
    /// </summary>
    /// <param name="p">Parser to hide</param>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <returns>Parser</returns>
    public static virtual K<MP, A> Hidden<A>(K<MP, A> p) =>
        MP.Label("", p);

    /// <summary>
    /// The parser `@try(p)` behaves like the parser `p`, except that it
    /// backtracks the parser state when `p` fails (either consuming input or
    /// not).
    /// </summary>
    /// <remarks>
    /// This combinator is used whenever arbitrary look-ahead is needed. Since
    /// it pretends that it hasn't consumed any input when `p` fails, the
    /// (`|`) combinator will try its second alternative even if the first
    /// parser failed while consuming input.
    /// </remarks>
    /// <example>
    /// For example, here is a parser that is supposed to parse the word “let”
    /// or the word “lexical”:
    ///
    ///     parseTest((string("let") | string("lexical")), "lexical")
    /// 
    ///     unexpected "lex"
    ///     expecting "let"
    ///
    /// What happens here? The first parser consumes “le” and fails (because it
    /// doesn't see a “t”). The second parser, however, isn't tried, since the
    /// first parser has already consumed some input! `Try` fixes this behavior
    /// and allows backtracking to work:
    ///
    ///     parseTest((@try (string("let")) | string("lexical")), "lexical")
    ///     "lexical"
    ///
    /// `Try` also improves error messages in case of overlapping alternatives,
    /// because Megaparsec's hint system can be used:
    ///
    ///     parseTest((@try (string("let")) | string("lexical")), "le")
    /// 
    ///     unexpected "le"
    ///     expecting "let" or "lexical"
    /// </example>
    /// <remarks>
    /// **Note** that the combinator: `string` backtracks automatically (see `tokens`), so it
    /// does not need `@try`. However, the examples above demonstrate the idea behind `@try` so well
    /// that it was decided to keep them. You still need to use `@try` when your
    /// alternatives are complex, composite parsers.
    /// </remarks>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <returns>Parser</returns>
    public static abstract K<MP, A> Try<A>(K<MP, A> p);

    /// <summary>
    /// If `p` in `lookAhead(p)` succeeds (either by consuming input or not),
    /// the whole parser behaves like `p` succeeded without consuming anything
    /// (parser state is also not updated). If `p` fails, `lookAhead` has no
    ///  effect, i.e. it will fail consuming input if `p` fails consuming input.
    ///  Combine with `try` if this is undesirable
    /// </summary>
    /// <param name="p">Parser to look ahead with</param>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <returns>Parser</returns>
    public static abstract K<MP, A> LookAhead<A>(K<MP, A> p);

    /// <summary>
    /// `notFollowedBy(p)` only succeeds when the parser `p` fails. This parser
    /// /never consumes/ any input and /never modifies/ parser state. It can be
    /// used to implement the “longest match” rule.
    /// </summary>
    /// <param name="p">Parser to test</param>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <returns>Unit parser</returns>
    public static abstract K<MP, Unit> NotFollowedBy<A>(K<MP, A> p);

    /// <summary>
    /// `withRecovery(f, p)` allows us to continue parsing even if the parser
    /// `p` fails. In this case `f` is called with the `ParseError` as its
    /// argument. Typical usage is to return a value signifying failure to
    /// parse this particular object and to consume some part of the input up
    /// to the point where the next object starts.
    /// 
    /// Note that if `f` fails, the original error message is reported as if
    /// without `withRecovery`. In no way can the recovering parser `f` influence
    /// error messages.
    /// </summary>
    /// <param name="onError">Delegate to invoke on error</param>
    /// <param name="p">Parser to run</param>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <returns>Parser</returns>
    public static abstract K<MP, A> WithRecovery<A>(Func<ParseError<T, E>, K<MP, A>> onError, K<MP, A> p);

    /// <summary>
    /// This parser only succeeds at the end of input
    /// </summary>
    public static abstract K<MP, Unit> EOF { get; }

    /// <summary>
    /// `observing(p)` allows us to “observe” failure of the `p` parser,
    /// should it happen, without actually ending parsing but instead getting
    /// the `ParseError` in `Left`. On success, the parsed value is returned in
    /// `Right` as usual. Note, this primitive just allows you to observe
    /// parse errors as they happen, it does not backtrack or change how the
    /// `p` parser works in any way.
    /// </summary>
    /// <param name="p">Parser</param>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <returns>Parser</returns>
    public static abstract K<MP, Either<ParseError<T,E>, A>> Observing<A>(K<MP, A> p);
    
    /// <summary>
    /// The parser `token(test, expected)` accepts tokens for which the
    /// matching function `test` returns `Some` result. If `None` is
    /// returned, the `expected` set is used to report the items that were
    /// expected.
    /// </summary>
    /// <param name="test">Token predicate test function</param>
    /// <param name="expected">Expected items</param>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <returns>Token parser</returns>
    public static abstract K<MP, A> Token<A>(Func<T, Option<A>> test, in Set<ErrorItem<T>> expected);

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
    public static abstract K<MP, S> Tokens(Func<S, S, bool> test, in S chunk);

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
    public static abstract K<MP, S> TakeWhile(Func<T, bool> test, in Option<string> name = default);

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
    public static abstract K<MP, S> TakeWhile1(Func<T, bool> test, in Option<string> name = default);

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
    public static abstract K<MP, S> Take(int n, in Option<string> name = default);

    /// <summary>
    /// An escape hatch for defining custom 'MonadParsec' primitives
    /// </summary>
    /// <param name="f">Parsing function to lift</param>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <returns>Parser</returns>
    public static abstract K<MP, A> Lift<A>(Func<State<S, T, E>, Reply<E, S, T, A>> f);
}
