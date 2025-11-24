using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.Parsers;

/// <summary>
/// Parser state parser combinators
/// </summary>
/// <typeparam name="E">Error type</typeparam>
/// <typeparam name="S">Stream type</typeparam>
/// <typeparam name="T">Stream token type</typeparam>
/// <typeparam name="M">Self</typeparam>
public class TraversableState<E, S, T, M>
    where S : TraversableTokenStream<S, T>
    where M : MonadParsec<E, S, T, M>
{
    /// <summary>
    /// Return the current source position. This function /is not cheap/, do
    /// not call it e.g. on matching of every token, that's a bad idea. Still you
    /// can use it to get 'SourcePos' to attach to things that you parse.
    /// 
    /// The function works under the assumption that we move in the input stream
    /// only forwards and never backwards, which is always true unless the user
    /// abuses the library.
    /// </summary>
    /// <remarks>
    /// This isn't a high-performance function, use infrequently and only when you
    /// need to get the position of the current token.
    /// </remarks>
    /// <returns>Parser</returns>
    public static readonly K<M, SourcePos> getSourcePos =
        from st in M.ParserState
        let pst = S.ReachOffsetNoLine(st.Offset, st.PosState)
        from _ in M.UpdateParserState(_ => st with { PosState = pst })
        select pst.SourcePos;
}
