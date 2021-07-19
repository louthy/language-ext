using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// Represents the parser source text and the parser's 
    /// positional state.
    /// </summary>
    public class PString
    {
        public readonly string Value;
        public readonly int Index;
        public readonly int EndIndex;
        public readonly Pos Pos;
        public readonly Pos DefPos;
        public readonly Sidedness Side;
        public readonly Option<object> UserState;

        public PString(string value, int index, int endIndex, Pos pos, Pos defPos, Sidedness side, Option<object> userState)
        {
            Value     = value ?? throw new ArgumentNullException(nameof(value));
            Value     = value;
            Index     = index;
            EndIndex  = endIndex;
            Pos       = pos;
            DefPos    = defPos;
            Side      = side;
            UserState = userState;
        }

        public PString SetDefPos(Pos defpos) =>
            new PString(Value, Index, EndIndex, Pos, defpos, Side, UserState);

        public PString SetPos(Pos pos) =>
            new PString(Value, Index, EndIndex, pos, DefPos, Side, UserState);

        public PString SetSide(Sidedness side) =>
            new PString(Value, Index, EndIndex, Pos, DefPos, side, UserState);

        public PString SetValue(string value) =>
            new PString(value, Index, value.Length, Pos, DefPos, Side, UserState);

        public PString SetIndex(int index) =>
            new PString(Value, index, EndIndex, Pos, DefPos, Side, UserState);

        public PString SetUserState(object state) =>
            new PString(Value, Index, EndIndex, Pos, DefPos, Side, state);

        public PString SetEndIndex(int endIndex) =>
            new PString(Value, Index, endIndex, Pos, DefPos, Side, UserState);

        public override string ToString() =>
            Value.Substring(Index, EndIndex - Index);

        public static readonly PString Zero =
            new PString("", 0, 0, Pos.Zero, Pos.Zero, Sidedness.Onside, None);
    }
}
