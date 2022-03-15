using System;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// Represents the parser source string and the parser's 
    /// positional state.
    /// </summary>
    public class PString<T>
    {
        public readonly T[] Value;
        public readonly int Index;
        public readonly int EndIndex;
        public readonly Option<object> UserState;
        public readonly Func<T, Pos> TokenPos;

        public PString(T[] value, int index, int endIndex, Option<object> userState, Func<T, Pos> tokenPos)
        {
            Value     = value ?? throw new ArgumentNullException(nameof(value));
            Index     = index;
            EndIndex  = endIndex;
            UserState = userState;
            TokenPos  = tokenPos;
        }

        public Pos Pos =>
            Value.Length == 0
                ? Pos.Zero
                : Index < Value.Length
                    ? TokenPos(Value[Index])
                    : TokenPos(Value[Value.Length - 1]);

        public PString<T> SetValue(T[] value) =>
            new PString<T>(value, Index, value.Length, UserState, TokenPos);

        public PString<T> SetIndex(int index) =>
            new PString<T>(Value, index, EndIndex, UserState, TokenPos);

        public PString<T> SetUserState(object state) =>
            new PString<T>(Value, Index, EndIndex, state, TokenPos);

        public PString<T> SetEndIndex(int endIndex) =>
            new PString<T>(Value, Index, endIndex, UserState, TokenPos);

        public override string ToString() =>
            $"{typeof(T).Name}({Index}, {EndIndex})";

        public static PString<T> Zero(Func<T, Pos> tokenPos) =>
            new PString<T>(System.Array.Empty<T>(), 0, 0, None, tokenPos);

        public PString<U> Cast<U>() where U : T =>
            new PString<U>(Value.Cast<U>().ToArray(), Index, EndIndex, UserState, u => TokenPos((T)u));
    }
}
