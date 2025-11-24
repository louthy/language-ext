using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.Parsers;

/// <summary>
/// Parser state parser combinators
/// </summary>
/// <typeparam name="E">Error type</typeparam>
/// <typeparam name="S">Stream type</typeparam>
/// <typeparam name="T">Stream token type</typeparam>
/// <typeparam name="M">Self</typeparam>
public class State<E, S, T, M>
    where S : TokenStream<S, T>
    where M : MonadParsec<E, S, T, M>
{
    /// <summary>
    /// Return the current input
    /// </summary>
    public static readonly K<M, S> input =
        (s => s.Input) * M.ParserState;

    /// <summary>
    /// `setInput(input)` continues parsing with `input`.
    /// </summary>
    /// <param name="input">Input to continue with</param>
    /// <returns>Parser</returns>
    public static K<M, Unit> setInput(S input) =>
        M.UpdateParserState(current => current with { Input = input });

    /// <summary>
    /// Get the number of tokens processed so far.
    /// </summary>
    /// <returns>Parser</returns>
    public static readonly K<M, int> offset =
        (x => x.Offset) * M.ParserState;

    /// <summary>
    /// Set the number of tokens processed so far
    /// </summary>
    /// <param name="offset">Token offset</param>
    /// <returns>Parser</returns>
    public static K<M, Unit> setOffset(int offset) =>
        M.UpdateParserState(s => s with { Offset = offset });

    /// <summary>
    /// Write a state to the parser
    /// </summary>
    /// <param name="st"></param>
    /// <returns></returns>
    public static K<M, Unit> setParserState(State<S, T, E> st) =>
        M.UpdateParserState(_ => st);
}
