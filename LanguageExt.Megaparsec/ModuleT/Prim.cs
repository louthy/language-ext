using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public static partial class ModuleT<MP, E, S, T, M>
    where MP : MonadParsecT<MP, E, S, T, M>
    where S : TokenStream<S, T>
    where M : Monad<M>
{
    /// <summary>
    /// Match a single token
    /// </summary>
    /// <example>
    ///     semicolon = single(';')
    /// </example>
    /// <param name="token">Token to match</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> single(T token) =>
        single<EqDefault<T>>(token);
    
    /// <summary>
    /// Match a single token
    /// </summary>
    /// <example>
    ///     semicolon = single(';')
    /// </example>
    /// <typeparam name="EqT">Equality trait</typeparam>
    /// <param name="token">Token to match</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> single<EqT>(T token) 
        where EqT : Eq<T> =>
        token<T>(x => EqT.Equals(x, token) ? Some(x) : None,
                 Set.singleton(ErrorItem.Token(token)));

    /// <summary>
    /// The parser `satisfy(f)` succeeds for any token for which the supplied
    /// function `f` returns `True`.
    /// </summary>
    /// <remarks>
    /// **Performance note**: when you need to parse a single token, it is often
    /// a good idea to use `satisfy` with the right predicate function instead of
    /// creating a complex parser using the combinators.
    /// </remarks>
    /// <remarks>
    /// See also: `anySingle`, `anySingleBut`, `oneOf`, `noneOf`.
    /// </remarks>
    /// <param name="f">Predicate function</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> satisfy(Func<T, bool> f) =>
        token<T>(x => f(x) ? Some(x) : None, default);

    /// <summary>
    /// Parse and return a single token. It's a good idea to attach a 'label'
    /// to this parser.
    /// </summary>
    public static readonly K<MP, T> anySingle =
        satisfy(_ => true);

    /// <summary>
    /// Match any token but the given one. It's a good idea to attach a `label`
    /// to this parser.
    /// </summary>
    /// <param name="token">Token to avoid</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> anySingleBut(T token) =>
        anySingleBut<EqDefault<T>>(token);

    /// <summary>
    /// Match any token but the given one. It's a good idea to attach a `label`
    /// to this parser.
    /// </summary>
    /// <typeparam name="EqT">Equality trait</typeparam>
    /// <param name="token">Token to avoid</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> anySingleBut<EqT>(T token) 
        where EqT : Eq<T> =>
        satisfy(x => !EqT.Equals(x, token));

    /// <summary>
    /// `chunk(chk)` only matches the chunk `chk`.
    /// </summary>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, S> chunk(S chk) =>
        tokens(static (x, y) => x.Equals(y), chk);

    /// <summary>
    /// The parser `label(name, p)` behaves as parser `p`, but whenever the
    /// parser `p` fails _without consuming any input_, it replaces names of
    /// 'expected' tokens with the name `name`.
    /// </summary>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <param name="name">Label name</param>
    /// <param name="p">Parser to label</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, A> label<A>(string name, K<MP, A> p) =>
        MP.Label(name, p);

    /// <summary>
    /// `hidden(p)` behaves just like parser `p`, but it doesn't show any
    /// 'expected' tokens in the error-message when `p` fails.
    /// </summary>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <param name="p">Parser to hide</param>
    /// <returns>Parser</returns>
    [Pure]
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
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <param name="p">Parser to try</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, A> @try<A>(K<MP, A> p) =>
        MP.Try(p);

    /// <summary>
    /// If `p` in `lookAhead(p)` succeeds (either by consuming input or not),
    /// the whole parser behaves like `p` succeeded without consuming anything
    /// (parser state is also not updated). If `p` fails, `lookAhead` has no
    ///  effect, i.e. it will fail consuming input if `p` fails consuming input.
    ///  Combine with `try` if this is undesirable
    /// </summary>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <param name="p">Parser to look ahead with</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, A> lookAhead<A>(K<MP, A> p) =>
        MP.LookAhead(p);

    /// <summary>
    /// `notFollowedBy(p)` only succeeds when the parser `p` fails. This parser
    /// /never consumes/ any input and /never modifies/ parser state. It can be
    /// used to implement the “longest match” rule.
    /// </summary>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <param name="p">Parser to test</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, Unit> notFollowedBy<A>(K<MP, A> p) => 
        MP.NotFollowedBy(p);

    /// <summary>
    /// This parser only succeeds at the end of input
    /// </summary>
    /// <returns>Parser</returns>
    public static readonly K<MP, Unit> eof =
        MP.EOF;

    /// <summary>
    /// An escape hatch for defining custom 'MonadParsec' primitives
    /// </summary>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <param name="f">Parsing function to lift</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, A> lift<A>(Func<State<S, T, E>, Reply<E, S, T, A>> f) =>
        MP.Lift(f);

    /// <summary>
    /// `oneOf(cases)` succeeds if the current token is in the collection of token
    /// `cases`. Returns the parsed token. Note, this parser cannot automatically
    /// generate the “expected” component of the error-message, so usually you should
    /// label it manually with `label` or `|`.
    /// </summary>
    /// <typeparam name="F">Foldable trait</typeparam>
    /// <param name="cases">Token cases to test</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> oneOf<F>(K<F, T> cases)
        where F : Foldable<F> =>
        oneOf<F, EqDefault<T>>(cases);

    /// <summary>
    /// `oneOf(cases)` succeeds if the current token is in the collection of token
    /// `cases`. Returns the parsed token. Note, this parser cannot automatically
    /// generate the “expected” component of the error-message, so usually you should
    /// label it manually with `label` or `|`.
    /// </summary>
    /// <typeparam name="F">Foldable trait</typeparam>
    /// <typeparam name="EqT">Equality trait</typeparam>
    /// <param name="cases">Token cases to test</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> oneOf<F, EqT>(K<F, T> cases)
        where F : Foldable<F> 
        where EqT : Eq<T> =>
        satisfy(x => Foldable.contains<EqT, F, T>(x, cases));

    /// <summary>
    /// `oneOf(cases)` succeeds if the current token is in the collection of token
    /// `cases`. Returns the parsed token. Note, this parser cannot automatically
    /// generate the “expected” component of the error-message, so usually you should
    /// label it manually with `label` or `|`.
    /// </summary>
    /// <param name="cases">Token cases to test</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> oneOf(ReadOnlySpan<T> cases) =>
        MP.OneOf<EqDefault<T>>(S.TokensToChunk(cases));

    /// <summary>
    /// `oneOf(cases)` succeeds if the current token is in the collection of token
    /// `cases`. Returns the parsed token. Note, this parser cannot automatically
    /// generate the “expected” component of the error-message, so usually you should
    /// label it manually with `label` or `|`.
    /// </summary>
    /// <typeparam name="EqT">Equality trait</typeparam>
    /// <param name="cases">Token cases to test</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> oneOf<EqT>(ReadOnlySpan<T> cases) 
        where EqT : Eq<T> =>
        MP.OneOf<EqT>(S.TokensToChunk(cases));

    /// <summary>
    /// `oneOf(cases)` succeeds if the current token is in the collection of token
    /// `cases`. Returns the parsed token. Note, this parser cannot automatically
    /// generate the “expected” component of the error-message, so usually you should
    /// label it manually with `label` or `|`.
    /// </summary>
    /// <param name="cases">Token cases to test</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> oneOf(S cases) =>
        MP.OneOf<EqDefault<T>>(cases);
    
    /// <summary>
    /// `oneOf(cases)` succeeds if the current token is in the collection of token
    /// `cases`. Returns the parsed token. Note, this parser cannot automatically
    /// generate the “expected” component of the error-message, so usually you should
    /// label it manually with `label` or `|`.
    /// </summary>
    /// <typeparam name="EqT">Equality trait</typeparam>
    /// <param name="cases">Token cases to test</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> oneOf<EqT>(S cases) 
        where EqT : Eq<T> =>
        MP.OneOf<EqT>(cases);

    /// <summary>
    /// As the dual of `oneOf`, `noneOf` succeeds if the current token is not in the
    /// supplied list of token `cases`. Returns the parsed character. Note that this
    /// parser cannot automatically generate the “expected” component of the
    /// error-message, so usually you should label it manually with.
    /// `label` or `|`
    /// </summary>
    /// <param name="cases">Token cases to test</param>
    /// <typeparam name="F">Foldable trait</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> noneOf<F>(K<F, T> cases)
        where F : Foldable<F> =>
        satisfy(x => !Foldable.contains(x, cases));
    
    /// <summary>
    /// As the dual of `oneOf`, `noneOf` succeeds if the current token is not in the
    /// supplied list of token `cases`. Returns the parsed character. Note that this
    /// parser cannot automatically generate the “expected” component of the
    /// error-message, so usually you should label it manually with.
    /// `label` or `|`
    /// </summary>
    /// <param name="cases">Token cases to test</param>
    /// <typeparam name="EqT">Equality trait</typeparam>
    /// <typeparam name="F">Foldable trait</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> noneOf<EqT, F>(K<F, T> cases)
        where F : Foldable<F> 
        where EqT : Eq<T> =>
        satisfy(x => !Foldable.contains<EqT, F, T>(x, cases));
    
    /// <summary>
    /// As the dual of `oneOf`, `noneOf` succeeds if the current token is not in the
    /// supplied list of token `cases`. Returns the parsed character. Note that this
    /// parser cannot automatically generate the “expected” component of the
    /// error-message, so usually you should label it manually with.
    /// `label` or `|`
    /// </summary>
    /// <param name="cases">Token cases to test</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> noneOf(ReadOnlySpan<T> cases) => 
        MP.NoneOf<EqDefault<T>>(S.TokensToChunk(cases));
    
    /// <summary>
    /// As the dual of `oneOf`, `noneOf` succeeds if the current token is not in the
    /// supplied list of token `cases`. Returns the parsed character. Note that this
    /// parser cannot automatically generate the “expected” component of the
    /// error-message, so usually you should label it manually with.
    /// `label` or `|`
    /// </summary>
    /// <param name="cases">Token cases to test</param>
    /// <typeparam name="EqT">Equality trait</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> noneOf<EqT>(ReadOnlySpan<T> cases) 
        where EqT : Eq<T> =>
        MP.NoneOf<EqT>(S.TokensToChunk(cases));
    
    /// <summary>
    /// As the dual of `oneOf`, `noneOf` succeeds if the current token is not in the
    /// supplied list of token `cases`. Returns the parsed character. Note that this
    /// parser cannot automatically generate the “expected” component of the
    /// error-message, so usually you should label it manually with.
    /// `label` or `|`
    /// </summary>
    /// <param name="cases">Token cases to test</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> noneOf(S cases) => 
        MP.NoneOf<EqDefault<T>>(cases);
    
    /// <summary>
    /// As the dual of `oneOf`, `noneOf` succeeds if the current token is not in the
    /// supplied list of token `cases`. Returns the parsed character. Note that this
    /// parser cannot automatically generate the “expected” component of the
    /// error-message, so usually you should label it manually with.
    /// `label` or `|`
    /// </summary>
    /// <param name="cases">Token cases to test</param>
    /// <typeparam name="EqT">Equality trait</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, T> noneOf<EqT>(S cases) 
        where EqT : Eq<T> =>
        MP.NoneOf<EqT>(cases);
    
    /// <summary>
    /// The `choice(ps)` parser tries to apply the parsers in the list `ps` in order,
    /// until one of them succeeds. Returns the value of the succeeding parser.
    /// </summary>
    /// <param name="ps">Parsers to try</param>
    /// <typeparam name="A">Type of value to parse</typeparam>
    /// <returns>Succeeding parser or MP.Empty on fail - use the `|` operator to capture failure</returns>
    [Pure]
    public static K<MP, A> choice<A>(params ReadOnlySpan<K<MP, A>> ps) =>
        Alternative.choice(ps);

    /// <summary>
    /// One or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will succeed if at least one item has been yielded.
    /// </remarks>
    /// <param name="p">Parser</param>
    /// <returns>One or more values</returns>
    [Pure]
    public static K<MP, Seq<A>> some<A>(K<MP, A> p) =>
        Alternative.some(p);
    
    /// <summary>
    /// Zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative parser repeatedly, collecting the results, until failure.
    /// Will always succeed.
    /// </remarks>
    /// <param name="p">Parser</param>
    /// <returns>Zero or more values</returns>
    [Pure]
    public static K<MP, Seq<A>> many<A>(K<MP, A> p) =>
        Alternative.many(p);

    /// <summary>
    /// Skip zero or more...
    /// </summary>
    /// <remarks>
    /// Run the parser repeatedly until failure.
    /// Will always succeed.
    /// </remarks>
    /// <param name="p">Parser</param>
    /// <returns>Unit</returns>
    [Pure]
    public static K<MP, Unit> skipMany<A>(K<MP, A> p) =>
        Alternative.skipMany(p);
    
    /// <summary>
    /// `endBy(p, sep)` parses zero-or-more occurrences of `p`, separated and ended by
    /// `sep`. Returns a list of values returned by `p`.
    /// </summary>
    /// <param name="p">Value parser</param>
    /// <param name="sep">Separator parser</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<MP, Seq<A>> endBy<A, SEP>(K<MP, A> p, K<MP, SEP> sep) =>
        Alternative.endBy(p, sep);
    
    /// <summary>
    /// `endBy1(p, sep)` parses one-or-more occurrences of `p`, separated and ended by
    /// `sep`. Returns a list of values returned by `p`.
    /// </summary>
    /// <param name="p">Value parser</param>
    /// <param name="sep">Separator parser</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<MP, Seq<A>> endBy1<A, SEP>(K<MP, A> p, K<MP, SEP> sep) =>
        Alternative.endBy1(p, sep);
    
    /// <summary>
    /// Combine two alternatives
    /// </summary>
    /// <param name="ma">Left parser</param>
    /// <param name="mb">Right parser</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Parser structure with an `Either` lifted into it</returns>
    [Pure]
    public static K<MP, Either<A, B>> either<A, B>(K<MP, A> ma, K<MP, B> mb) =>
        Alternative.either(ma, mb);
    
    /// <summary>
    /// `manyUntil(p, end)` applies `p` _zero_ or more times until `end` succeeds.
    /// Returns the list of values returned by`p`. `end` result is consumed and
    /// lost. Use `manyUntil2` if you wish to keep it.
    /// </summary>
    /// <param name="p">Parser to consume</param>
    /// <param name="end">Terminating parser</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, Seq<A>> manyUntil<A, END>(K<MP, A> p, K<MP, END> end) =>
        Alternative.manyUntil(p, end);
        
    /// <summary>
    /// `manyUntil2(p, end)` applies `p` _zero_ or more times until `end` succeeds.
    /// Returns the list of values returned by `p` plus the `end` result.
    ///
    /// Use `manyUntil` if you don't wish to keep the `end` result.
    /// </summary>
    /// <param name="p">Parser to consume</param>
    /// <param name="end">Terminating structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, (Seq<A> Items, END End)> manyUntil2<A, END>(K<MP, A> p, K<MP, END> end) =>
        Alternative.manyUntil2(p, end);
    
    /// <summary>
    /// `someUntil(p, end)` applies `p` _one_ or more times until `end` succeeds.
    /// Returns the list of values returned by `p`. `end` result is consumed and
    /// lost. Use `someUntil2` if you wish to keep it.
    /// </summary>
    /// <param name="p">Structure to consume</param>
    /// <param name="end">Terminating structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, Seq<A>> someUntil<A, END>(K<MP, A> p, K<MP, END> end)  =>
        Alternative.someUntil(p, end);

    /// <summary>
    /// `someUntil2(p, end)` applies `p` _one_ or more times until `end` succeeds.
    /// Returns the list of values returned by `p` plus the `end` result.
    ///
    /// Use `someUntil` if you don't wish to keep the `end` result.
    /// </summary>
    /// <param name="p">Structure to consume</param>
    /// <param name="end">Terminating structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, (Seq<A> Items, END End)> someUntil2<A, END>(K<MP, A> p, K<MP, END> end) =>
        Alternative.someUntil2(p, end);
    
    /// <summary>
    /// `option(x, p)` tries to apply `p`. If `p` fails without 'consuming' anything, it
    /// returns `value`, otherwise the value returned by `p`.
    /// </summary>
    /// <param name="value">Default value to use if `o` fails without 'consuming' anything</param>
    /// <param name="p">Parser</param>
    /// <typeparam name="A"></typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, A> option<A>(A value, K<MP, A> p)  =>
        Alternative.option(value, p);
    
    /// <summary>
    /// `sepBy(p, sep) processes _zero_ or more occurrences of `p`, separated by `sep`. 
    /// </summary>
    /// <param name="p">Structure to yield return values</param>
    /// <param name="sep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, Seq<A>> sepBy<A, SEP>(K<MP, A> p, K<MP, SEP> sep)  =>
        Alternative.sepBy(p, sep);
    
    /// <summary>
    /// `sepBy(p, sep) processes _one_ or more occurrences of `p`, separated by `sep`. 
    /// </summary>
    /// <param name="p">Structure to yield return values</param>
    /// <param name="sep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, Seq<A>> sepBy1<A, SEP>(K<MP, A> p, K<MP, SEP> sep)  =>
        Alternative.sepBy1(p, sep);

    /// <summary>
    /// `sepEndBy(p, sep) processes _zero_ or more occurrences of `p`, separated
    /// and optionally ended by `sep`. Returns a list of values returned by `p`.
    /// </summary>
    /// <param name="p">Structure to yield return values</param>
    /// <param name="sep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, Seq<A>> sepByEnd<A, SEP>(K<MP, A> p, K<MP, SEP> sep)  =>
        Alternative.sepByEnd(p, sep);

    /// <summary>
    /// `sepEndBy1(p, sep) processes _one_ or more occurrences of `p`, separated
    /// and optionally ended by `sep`. Returns a list of values returned by `p`.
    /// </summary>
    /// <param name="p">Structure to yield return values</param>
    /// <param name="sep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, Seq<A>> sepByEnd1<A, SEP>(K<MP, A> p, K<MP, SEP> sep) =>
        Alternative.sepByEnd1(p, sep);
    
    /// <summary>
    /// Process `p` _one_ or more times and drop all yielded values.
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly until failure. At least one item must be yielded for overall success.
    /// </remarks>
    /// <param name="p">Parser</param>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, Unit> skipSome<A>(K<MP, A> p) => 
        Alternative.skipSome(p);

    /// <summary>
    /// `skip(n, p)` processes `n` occurrences of `p`, skipping its result.
    /// If `n` is not positive, the process equates to `Pure(unit)`.
    /// </summary>
    /// <param name="n">Number of occurrences of `fa` to skip</param>
    /// <param name="p">Parser</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, Unit> skip<A>(int n, K<MP, A> p) => 
        Alternative.skip(n, p);

    /// <summary>
    /// `skipManyUntil(p, end)` applies the process `p` _zero_ or more times
    /// skipping results until process `end` succeeds. The resulting value from
    /// `end` is then returned.
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns>Parser</returns>
    [Pure]
    public static K<MP, END> skipManyUntil<A, END>(K<MP, A> p, K<MP, END> end) =>
        Alternative.skipManyUntil(p, end);

    /// <summary>
    /// `skipManyUntil(p, end)` applies the process `p` _one_ or more times
    /// skipping results until process `end` succeeds. The resulting value from
    /// `end` is then returned.
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<MP, END> skipSomeUntil<A, END>(K<MP, A> p, K<MP, END> end) => 
        Alternative.skipSomeUntil(p, end);    
}
