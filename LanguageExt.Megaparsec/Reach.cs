using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

internal static class Reach<S, T>
    where S : TokenStream<S, T>
{
    public interface Folder
    {
        /// <summary>
        /// Folder
        /// </summary>
        B Fold<B>(Func<B, T, B> fold, B state, S stream);
    }

    public static (Option<LineText> Line, PosState<T> UpdatedState) Offset(
        Func<int, S, (S Left, S Right)> splitAt, //< How to split input stream at given offset
        Folder foldl,                            //< How to fold over input stream
        Func<S, string> fromToks,                //< How to convert chunk of input stream into a 'String'
        Func<T, char> fromTok,                   //< How to convert a token into a 'char'
        (T Newline, T Tab) tokens,               //< Newline token and tab token
        Func<T, Pos> columnIncrement,            //< Increment in column position for a token
        int offset,                              //< Offset to reach
        PosState<T> posState                     //< Initial `PosState` to use
        )
    {
        throw new NotImplementedException();
    }
}
