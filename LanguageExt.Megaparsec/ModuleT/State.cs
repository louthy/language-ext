using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public static partial class ModuleT<MP, E, S, T, M>
    where MP : MonadParsecT<MP, E, S, T, M>
    where S : TokenStream<S, T>
    where M : Monad<M>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  State parser combinators
    //

    /// <summary>
    /// Return the full parser state as a `State` record
    /// </summary>
    /// <returns>Parser</returns>
    public static readonly K<MP, State<S, T, E>> getParserState =
        MP.Ask;

    /// <summary>
    /// Write the full parser state
    /// </summary>
    /// <returns>Parser</returns>
    public static K<MP, Unit> setParserState(State<S, T, E> s) => 
        MP.Put(s);

    /// <summary>
    /// Return the full parser state and then map it to a new value using the supplied function
    /// </summary>
    /// <returns>Parser</returns>
    public static K<MP, A> mapParserState<A>(Func<State<S, T, E>, A> f) => 
        MP.Asks(f);

    /// <summary>
    /// Update the parser state using the supplied function
    /// </summary>
    /// <param name="f">Update function</param>
    /// <returns>Parser</returns>
    public static K<MP, Unit> modifyParserState(Func<State<S, T, E>, State<S, T, E>> f) =>
        MP.Modify(f);

    /// <summary>
    /// Return the current input
    /// </summary>
    public static readonly K<MP, S> getInput =
        MP.Asks(s => s.Input);

    /// <summary>
    /// `setInput(input)` continues parsing with `input`.
    /// </summary>
    /// <param name="input">Input to continue with</param>
    /// <returns>Parser</returns>
    public static K<MP, Unit> setInput(S input) =>
        MP.Modify(current => current with { Input = input });

    /// <summary>
    /// Get the number of tokens processed so far.
    /// </summary>
    /// <returns>Parser</returns>
    public static readonly K<MP, int> getOffset =
        MP.Asks(x => x.Offset);

    /// <summary>
    /// Set the number of tokens processed so far
    /// </summary>
    /// <param name="offset">Token offset</param>
    /// <returns>Parser</returns>
    public static K<MP, Unit> setOffset(int offset) =>
        MP.Modify(s => s with { Offset = offset });
    
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
    public static readonly K<MP, SourcePos> getSourcePos =
        from st in MP.Get
        let pst =  Reach<S, T>.offsetNoLine(st.Offset, st.PosState)
        from _  in MP.Put(st with { PosState = pst })
        select pst.SourcePos;    
}
