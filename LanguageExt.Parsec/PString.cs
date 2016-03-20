using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class PString
    {
        internal readonly string Value;
        public readonly int Index;
        public readonly Pos Pos;
        public readonly Pos DefPos;
        public readonly Sidedness Side;

        public PString(string value, int index, Pos pos, Pos defPos, Sidedness side)
        {
            Value = value;
            Index = index;
            Pos = pos;
            DefPos = defPos;
            Side = side;
        }

        public PString SetDefPos(Pos defpos) =>
            new PString(Value, Index, Pos, defpos, Side);

        public PString SetPos(Pos pos) =>
            new PString(Value, Index, pos, DefPos, Side);

        public PString SetSide(Sidedness side) =>
            new PString(Value, Index, Pos, DefPos, side);

        public PString SetValue(string value) =>
            new PString(value, Index, Pos, DefPos, Side);

        public PString SetIndex(int index) =>
            new PString(Value, index, Pos, DefPos, Side);

        public override string ToString() =>
            Value.Substring(Index);

        public static readonly PString Zero =
            new PString("", 0, Pos.Zero, Pos.Zero, Sidedness.Onside);
    }
}
