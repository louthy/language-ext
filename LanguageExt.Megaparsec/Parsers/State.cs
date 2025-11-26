using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public static partial class Module<MP, E, S, T, M>
    where MP : MonadParsecT<MP, E, S, T, M>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  State parser combinators
    //

    /// <summary>
    /// Return the current input
    /// </summary>
    public static readonly K<MP, S> getInput =
        (s => s.Input) * MP.ParserState;

    /// <summary>
    /// `setInput(input)` continues parsing with `input`.
    /// </summary>
    /// <param name="input">Input to continue with</param>
    /// <returns>Parser</returns>
    public static K<MP, Unit> setInput(S input) =>
        MP.UpdateParserState(current => current with { Input = input });

    /// <summary>
    /// Get the number of tokens processed so far.
    /// </summary>
    /// <returns>Parser</returns>
    public static readonly K<MP, int> getOffset =
        (x => x.Offset) * MP.ParserState;

    /// <summary>
    /// Set the number of tokens processed so far
    /// </summary>
    /// <param name="offset">Token offset</param>
    /// <returns>Parser</returns>
    public static K<MP, Unit> setOffset(int offset) =>
        MP.UpdateParserState(s => s with { Offset = offset });

    /// <summary>
    /// Write a state to the parser
    /// </summary>
    /// <param name="st"></param>
    /// <returns></returns>
    public static K<MP, Unit> setParserState(State<S, T, E> st) =>
        MP.UpdateParserState(_ => st);
}
