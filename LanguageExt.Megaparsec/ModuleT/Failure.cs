using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public static partial class ModuleT<MP, E, S, T, M>
    where MP : MonadParsecT<MP, E, S, T, M>
    where S : TokenStream<S, T>
    where M : Monad<M>
{
    /// <summary>
    /// Stop parsing and report the `ParseError`.
    /// </summary>
    /// <remarks>
    /// This is the only way to control the position of the error without
    /// manipulating the parser state manually.
    /// </remarks>
    /// <typeparam name="A">Value type to parse</typeparam>
    /// <param name="error">Error</param>
    /// <returns>Parser</returns>
    public static K<MP, A> error<A>(ParseError<T, E> error) =>
        MP.Error<A>(error);

    /// <summary>
    /// Stop parsing and report a trivial `ParseError`.
    /// </summary>
    /// <param name="unexpected">Optional unexpected tokens</param>
    /// <param name="expected">Expected tokens</param>
    /// <typeparam name="A">Value type (never yielded because this is designed to error)</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> failure<A>(Option<ErrorItem<T>> unexpected, Set<ErrorItem<T>> expected) =>
        getOffset >>> (o => error<A>(ParseError.Trivial<T, E>(o, unexpected, expected)));

    /// <summary>
    /// Stop parsing and report a fancy 'ParseError'. To report a single custom parse error
    /// </summary>
    /// <param name="errors">Optional unexpected tokens</param>
    /// <typeparam name="A">Value type (never yielded because this is designed to error)</typeparam>
    /// <returns>Parser</returns>
    public static K<MP, A> failure<A>(Set<ErrorFancy<E>> errors) =>
        getOffset >>> (o => error<A>(ParseError.Fancy<T, E>(o, errors)));

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
    /// `observing(p)` allows us to 'observe' failure of the `p` parser,
    /// should it happen, without actually ending parsing but instead getting
    /// the `ParseError` in `Left`. On success, the parsed value is returned in
    /// `Right`, as usual. Note, this primitive just allows you to observe
    /// parse errors as they happen, it does not backtrack or change how the
    /// `p` parser works in any way.
    /// </summary>
    /// <typeparam name="A">Parser value type</typeparam>
    /// <param name="p">Parser</param>
    /// <returns>Parser</returns>
    public static K<MP, Either<ParseError<T, E>, A>> observing<A>(K<MP, A> p) =>
        MP.Observing(p);

    /// <summary>
    /// Specify how to process a `ParseError` that happens inside the specified
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
        from de in MP.Gets(s => s.ParseErrors) >>>
                   MP.Modify(s => s with { ParseErrors = [] })
        from r1 in MP.Observing(region) >>>
                   MP.Modify(s => s with { ParseErrors = s.ParseErrors.Map(mapError) + de })
        from r2 in r1 switch
                   {
                       Either<ParseError<T, E>, A>.Left (var err) => MP.Error<A>(mapError(err)),
                       Either<ParseError<T, E>, A>.Right (var x)  => MP.Pure(x),
                       _                                          => throw new NotSupportedException()
                   }
        select r2;
  
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
    public static K<MP, A> withRecovery<A>(Func<ParseError<T, E>, K<MP, A>> onError, K<MP, A> p) =>
        MP.WithRecovery(onError, p);
}
