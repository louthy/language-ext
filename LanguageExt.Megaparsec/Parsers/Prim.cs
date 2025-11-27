using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public static partial class Module<MP, E, S, T, M>
    where MP : MonadParsecT<MP, E, S, T, M>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Primitive parser combinators
    //
    
    /// <summary>
    /// Stop parsing and report a trivial `ParseError`.
    /// </summary>
    /// <param name="unexpected">Optional unexpected tokens</param>
    /// <param name="expected">Expected tokens</param>
    /// <typeparam name="A">Value type (never yielded because this is designed to error)</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> failure<A>(Option<ErrorItem<T>> unexpected, Set<ErrorItem<T>> expected) =>
        from o in getOffset
        from r in error<A>(ParseError.Trivial<T, E>(o, unexpected, expected))
        select r;

    /// <summary>
    /// Stop parsing and report a fancy 'ParseError'. To report a single custom parse error
    /// </summary>
    /// <param name="errors">Optional unexpected tokens</param>
    /// <param name="expected">Expected tokens</param>
    /// <typeparam name="A">Value type (never yielded because this is designed to error)</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> failure<A>(Set<ErrorFancy<E>> errors) =>
        from o in getOffset
        from r in error<A>(ParseError.Fancy<T, E>(o, errors))
        select r;

    /// <summary>
    /// Stop parsing and report a fancy 'ParseError'. To report a single custom parse error
    /// </summary>
    /// <param name="error">Custom error</param>
    /// <typeparam name="A">Value type (never yielded because this is designed to error)</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> failure<A>(E error) =>
        Pure(error) >> ErrorFancy.Custom >> Set.singleton >> (failure<A>) >> lower;

    /// <summary>
    /// The parser `unexpected(item)` fails with an error message telling
    /// about an unexpected `item` without consuming any input.
    /// </summary>
    /// <param name="item">The unexpected item</param>
    /// <typeparam name="A">Value type (never yielded because this is designed to error)</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> unexpected<A>(ErrorItem<T> item) =>
        failure<A>(Some(item), default);

    /// <summary>
    /// Specify how to process a `ParseError` that happens inside of the specified
    /// region. This applies to both normal and delayed `ParseError` values.
    /// 
    /// As a side effect of the implementation, the inner computation will start
    /// with an empty collection of delayed errors; they will be updated and
    /// “restored” on the way out of 'region'.
    /// </summary>
    /// <param name="mapError">Error mapping for any raised error in the region</param>
    /// <param name="region">Region to process</param>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> region<A>(Func<ParseError<T, E>, ParseError<T, E>> mapError, K<MP, A> region) =>
        from de in MP.Asks(s => s.ParseErrors)
        from _1 in MP.Modify(s => s with { ParseErrors = [] })
        from r1 in MP.Observing(region)
        from _2 in MP.Modify(s => s with { ParseErrors = s.ParseErrors.Map(mapError) + de })
        from r2 in r1 switch
                   {
                       Either<ParseError<T, E>, A>.Left (var err) => MP.Error<A>(mapError(err)),
                       Either<ParseError<T, E>, A>.Right (var x)  => MP.Pure(x),
                       _                                          => throw new NotSupportedException()
                   }
        select r2;

    /// <summary>
    /// Match a single token
    /// </summary>
    /// <example>
    ///     semicolon = single(';')
    /// </example>
    /// <param name="token">Token to match</param>
    /// <returns>Parser</returns>
    public static K<MP, T> single(T token) =>
        token<T>(x => x?.Equals(token) ?? false ? Some(x) : None,
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
    public static K<MP, T> anySingleBut(T token) =>
        satisfy(x => !(x?.Equals(token) ?? false));

    /// <summary>
    /// `oneOf(cases)` succeeds if the current token is in the collection of token
    /// `cases`. Returns the parsed token. Note, this parser cannot automatically
    /// generate the “expected” component of the error-message, so usually you should
    /// label it manually with `label` or `|`.
    /// </summary>
    /// <param name="cases">Token cases to test</param>
    /// <typeparam name="F">Foldable trait</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, T> oneOf<F>(K<F, T> cases)
        where F : Foldable<F> =>
        satisfy(x => Foldable.contains(x, cases));

    /// <summary>
    /// `oneOf(cases)` succeeds if the current token is in the collection of token
    /// `cases`. Returns the parsed token. Note, this parser cannot automatically
    /// generate the “expected” component of the error-message, so usually you should
    /// label it manually with `label` or `|`.
    /// </summary>
    /// <param name="cases">Token cases to test</param>
    /// <returns>Parser</returns>
    public static K<MP, T> oneOf(Seq<T> cases) =>
        satisfy(x => Foldable.contains(x, cases));

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
    /// <returns>Parser</returns>
    public static K<MP, T> noneOf(Seq<T> cases) =>
        satisfy(x => !Foldable.contains(x, cases));

    /// <summary>
    /// `chunk(chk)` only matches the chunk `chk`.
    /// </summary>
    /// <returns>Parser</returns>
    public static K<MP, S> chunk(S chk) =>
        tokens(static (x, y) => x.Equals(y), chk);
}
