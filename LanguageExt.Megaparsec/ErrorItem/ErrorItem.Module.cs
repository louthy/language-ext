using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public class ErrorItem : Functor<ErrorItem>
{
    public static ErrorItem<T> Token<T>(T token) => 
        new ErrorItem<T>.Tokens([token]);
    
    public static ErrorItem<T> Tokens<T>(in Seq<T> tokens) => 
        new ErrorItem<T>.Tokens(tokens);
    
    public static ErrorItem<T> Tokens<T>(in ReadOnlySpan<T> tokens) => 
        new ErrorItem<T>.Tokens([..tokens]);
    
    public static ErrorItem<T> Tokens<S, T>(in S tokens)
        where S : TokenStream<S, T> => 
        Tokens(S.ChunkToTokens(tokens));
    
    public static ErrorItem<T> Label<T>(string label) => 
        new ErrorItem<T>.Label(label);
    
    public static ErrorItem<T> EndOfInput<T>() => 
        new ErrorItem<T>.EndfOfInput();

    public static K<ErrorItem, B> Map<A, B>(Func<A, B> f, K<ErrorItem, A> ma) =>
        ma switch
        {
            ErrorItem<A>.Tokens (var tokens) => Tokens(tokens.Map(f)),
            ErrorItem<A>.Label (var label)   => Label<B>(label),
            ErrorItem<A>.EndfOfInput         => EndOfInput<B>(),
            _                                => throw new NotSupportedException()
        };
}

