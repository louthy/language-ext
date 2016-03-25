using static LanguageExt.Prelude;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// Represents the parser source text and the parser's 
    /// positional state.
    /// </summary>
    public class PString
    {
        internal readonly string Value;
        public readonly int Index;
        public readonly Pos Pos;
        public readonly Pos DefPos;
        public readonly Sidedness Side;
        public readonly Option<object> UserState;

        public PString(string value, int index, Pos pos, Pos defPos, Sidedness side, Option<object> userState)
        {
            Value = value;
            Index = index;
            Pos = pos;
            DefPos = defPos;
            Side = side;
            UserState = userState;
        }

        public PString SetDefPos(Pos defpos) =>
            new PString(Value, Index, Pos, defpos, Side, UserState);

        public PString SetPos(Pos pos) =>
            new PString(Value, Index, pos, DefPos, Side, UserState);

        public PString SetSide(Sidedness side) =>
            new PString(Value, Index, Pos, DefPos, side, UserState);

        public PString SetValue(string value) =>
            new PString(value, Index, Pos, DefPos, Side, UserState);

        public PString SetIndex(int index) =>
            new PString(Value, index, Pos, DefPos, Side, UserState);

        public PString SetUserState(object state) =>
            new PString(Value, Index, Pos, DefPos, Side, state);

        public override string ToString() =>
            Value.Substring(Index);

        public static readonly PString Zero =
            new PString("", 0, Pos.Zero, Pos.Zero, Sidedness.Onside, None);
    }
}
