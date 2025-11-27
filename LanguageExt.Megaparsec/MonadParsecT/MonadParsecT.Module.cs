using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
/// MonadParsec module
/// </summary>
public static partial class Module<MP, E, S, M>
    where MP : MonadParsecT<MP, E, S, char, M>
    where S : TokenStream<S, char>
    where M : Monad<M>
{
    /// <summary>
    /// Stop parsing and report the `ParseError`. This is the only way to
    /// control the position of the error without manipulating the parser state
    /// manually.
    /// </summary>
    /// <param name="error">Error</param>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> error<A>(ParseError<char, E> error) =>
        MP.Error<A>(error);

    /// <summary>
    /// The parser `Label(name, p)` behaves as parser `p`, but whenever the
    /// parser `p` fails /without consuming any input/, it replaces names of
    /// “expected” tokens with the name `name`.
    /// </summary>
    /// <param name="name">Label name</param>
    /// <param name="p">Parser to label</param>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> label<A>(string name, K<MP, A> p) =>
        MP.Label(name, p);

    /// <summary>
    /// `hHidden(p)` behaves just like parser `p`, but it doesn't show any
    /// “expected” tokens in the error-message when `p` fails.
    /// </summary>
    /// <param name="p">Parser to hide</param>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> hidden<A>(K<MP, A> p) =>
        MP.Hidden(p);

    /// <summary>
    /// The parser `Try(p)` behaves like the parser `p`, except that it
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
    ///     parseTest((try (string("let")) | string("lexical")), "lexical")
    ///     "lexical"
    ///
    /// `Try` also improves error messages in case of overlapping alternatives,
    /// because Megaparsec's hint system can be used:
    ///
    ///     parseTest((try (string("let")) | string("lexical")), "le")
    /// 
    ///     unexpected "le"
    ///     expecting "let" or "lexical"
    /// </example>
    /// **Note** that the combinator: `string` backtracks automatically (see `tokens`), so it
    /// does not need `Try`. However, the examples above demonstrate the idea behind 'Try' so well
    /// that it was decided to keep them. You still need to use 'Try' when your
    /// alternatives are complex, composite parsers.
    /// <param name="p">Parser to try</param>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> @try<A>(K<MP, A> p) =>
        MP.Try(p);

    /// <summary>
    /// If `p` in `lookAhead(p)` succeeds (either by consuming input or not),
    /// the whole parser behaves like `p` succeeded without consuming anything
    /// (parser state is also not updated). If `p` fails, `lookAhead` has no
    ///  effect, i.e. it will fail consuming input if `p` fails consuming input.
    ///  Combine with `try` if this is undesirable
    /// </summary>
    /// <param name="p">Parser to look ahead with</param>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> lookAhead<A>(K<MP, A> p) =>
        MP.LookAhead(p);

    /// <summary>
    /// `notFollowedBy(p)` only succeeds when the parser `p` fails. This parser
    /// /never consumes/ any input and /never modifies/ parser state. It can be
    /// used to implement the “longest match” rule.
    /// </summary>
    /// <param name="p">Parser to test</param>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, Unit> notFollowedBy<A>(K<MP, A> p) =>
        MP.NotFollowedBy(p);
  
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
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> withRecovery<A>(Func<ParseError<char, E>, K<MP, A>> onError, K<MP, A> p) =>
        MP.WithRecovery(onError, p);    

    /// <summary>
    /// This parser only succeeds at the end of input
    /// </summary>
    /// <returns>Parser</returns>
    public static readonly K<MP, Unit> eof =
        MP.EOF;

    /// <summary>
    /// `observing(p)` allows us to “observe” failure of the `p` parser,
    /// should it happen, without actually ending parsing but instead getting
    /// the `ParseError` in `Left`. On success, the parsed value is returned in
    /// `Right` as usual. Note, this primitive just allows you to observe
    /// parse errors as they happen, it does not backtrack or change how the
    /// `p` parser works in any way.
    /// </summary>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <param name="p">Parser</param>
    /// <returns>Parser</returns>
    public static K<MP, Either<ParseError<char, E>, A>> observing<A>(K<MP, A> p) =>
        MP.Observing(p);
    
    /// <summary>
    /// The parser `token(test, expected)` accepts tokens for which the
    /// matching function `test` returns `Some` result. If `None` is
    /// returned, the `expected` set is used to report the items that were
    /// expected.
    /// </summary>
    /// <param name="test">Token predicate test function</param>
    /// <param name="expected">Expected items</param>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <returns>Token parser</returns>
    public static K<MP, A> token<A>(Func<char, Option<A>> test, in Set<ErrorItem<char>> expected) =>
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
    public static K<MP, S> takeWhile(Func<char, bool> test, Option<string> name = default) =>
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
    public static K<MP, S> takeWhile1(Func<char, bool> test, Option<string> name = default) =>
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

    /// <summary>
    /// Return the full parser state as a `State` record
    /// </summary>
    /// <returns>Parser</returns>
    public static readonly K<MP, State<S, char, E>> getParserState = 
        MP.Ask;

    /// <summary>
    /// Write the full parser state
    /// </summary>
    /// <returns>Parser</returns>
    public static K<MP, Unit> putParserState(State<S, char, E> s) => 
        MP.Put(s);

    /// <summary>
    /// Return the full parser state and then map it to a new value using the supplied function
    /// </summary>
    /// <returns>Parser</returns>
    public static K<MP, A> mapParserState<A>(Func<State<S, char, E>, A> f) => 
        MP.Asks(f);

    /// <summary>
    /// Update the parser state using the supplied function
    /// </summary>
    /// <param name="f">Update function</param>
    /// <returns>Parser</returns>
    public static K<MP, Unit> modifyParserState(Func<State<S, char, E>, State<S, char, E>> f) =>
        MP.Modify(f);

    /// <summary>
    /// An escape hatch for defining custom 'MonadParsec' primitives
    /// </summary>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <param name="f">Parsing function to lift</param>
    /// <returns>Parser</returns>
    public static K<MP, A> lift<A>(Func<State<S, char, E>, Reply<E, S, char, A>> f) =>
        MP.Lift(f);    
}
