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
        internal readonly T[] Value;
        public readonly int Index;
        public readonly int EndIndex;
        public readonly Pos Pos;
        public readonly Pos DefPos;
        public readonly Sidedness Side;
        public readonly Option<object> UserState;

        public PString(T[] value, int index, int endIndex, Pos pos, Pos defPos, Sidedness side, Option<object> userState)
        {
            Value = value;
            Index = index;
            EndIndex = endIndex;
            Pos = pos;
            DefPos = defPos;
            Side = side;
            UserState = userState;
        }

        public PString<T> SetDefPos(Pos defpos) =>
            new PString<T>(Value, Index, EndIndex, Pos, defpos, Side, UserState);

        public PString<T> SetPos(Pos pos) =>
            new PString<T>(Value, Index, EndIndex, pos, DefPos, Side, UserState);

        public PString<T> SetSide(Sidedness side) =>
            new PString<T>(Value, Index, EndIndex, Pos, DefPos, side, UserState);

        public PString<T> SetValue(T[] value) =>
            new PString<T>(value, Index, value.Length, Pos, DefPos, Side, UserState);

        public PString<T> SetIndex(int index) =>
            new PString<T>(Value, index, EndIndex, Pos, DefPos, Side, UserState);

        public PString<T> SetUserState(object state) =>
            new PString<T>(Value, Index, EndIndex, Pos, DefPos, Side, state);

        public PString<T> SetEndIndex(int endIndex) =>
            new PString<T>(Value, Index, endIndex, Pos, DefPos, Side, UserState);

        public override string ToString() =>
            $"{typeof(T).Name}({Index}, {EndIndex})";

        public static readonly PString<T> Zero =
            new PString<T>(new T[0], 0, 0, Pos.Zero, Pos.Zero, Sidedness.Onside, None);

        public PString<U> Cast<U>() =>
            new PString<U>(Value.Cast<U>().ToArray(), Index, EndIndex, Pos, DefPos, Side, UserState);
    }
}
