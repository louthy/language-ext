using System;

namespace LanguageExt.Traits;

/// <summary>
/// Low-level streaming trait.  Used primarily by Megaparsec.
/// </summary>
/// <typeparam name="S">Token type</typeparam>
public interface TokenStream<TOKENS, TOKEN>
    where TOKENS : TokenStream<TOKENS, TOKEN>
{
    /// <summary>
    /// Lift a single token to chunk of the stream
    /// </summary>
    public static abstract TOKENS TokenToChunk(in TOKEN token);

    /// <summary>
    /// Lift many tokens to chunk of the stream
    /// </summary>
    public static abstract TOKENS TokensToChunk(in ReadOnlySpan<TOKEN> token);
    
    /// <summary>
    /// Turn a chunk into a sequence of tokens
    /// </summary>
    public static abstract ReadOnlySpan<TOKEN> ChunkToTokens(in TOKENS tokens);

    /// <summary>
    /// Get the length of a chunk
    /// </summary>
    public static abstract int ChunkLength(in TOKENS tokens);

    /// <summary>
    /// Take the first element of the stream if it exists.
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>Head element taken from the stream and a Tail of remaining stream items</returns>
    public static abstract bool Take1(in TOKENS stream, out TOKEN head, out TOKENS tail);
    
    /// <summary>
    /// `Take` should try to extract a chunk of length `amount`, or if the
    /// stream is too short, the rest of the stream. Valid implementation
    /// should follow the rules:
    /// 
    /// * If the requested length `amount` is `0` (or less), `None` should never be returned, instead `Some([], s)`
    /// should be returned, where `[]` stands for the empty chunk, and `s` is the original stream (second argument).
    /// * If the requested length is greater than `0` and the stream is empty, `None` should be returned indicating end-of-input.
    /// * In other cases, take chunk of length `amount` (or shorter if the   stream is not long enough) from the input
    /// stream and return the chunk along with the rest of the stream.
    /// </summary>
    /// <param name="amount">Number of elements to take</param>
    /// <param name="stream">Stream</param>
    /// <returns>Head element taken from the stream and a Tail of remaining stream items</returns>
    public static abstract bool Take(int amount, in TOKENS stream, out ReadOnlySpan<TOKEN> head, out TOKENS tail);
    
    /// <summary>
    /// Extract chunk of the stream taking tokens while the supplied predicate returns 'True'. Return the chunk and the
    /// rest of the stream.
    /// 
    /// For many types of streams, the method allows for significant performance improvements, although it is not
    /// strictly necessary from a conceptual point-of-view.
    /// </summary>
    /// <param name="predicate">Token testing predicate</param>
    /// <param name="stream">Stream to read from</param>
    /// <returns></returns>
    public static abstract TOKENS TakeWhile(Func<TOKEN, bool> predicate, in TOKENS stream, out ReadOnlySpan<TOKEN> head);
}
