using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class PString
    {
        public readonly string Value;
        public readonly Pos Pos;
        public readonly Pos DefPos;
        public readonly Sidedness Side;

        public PString(string value, Pos pos, Pos defPos, Sidedness side)
        {
            Value = value;
            Pos = pos;
            DefPos = defPos;
            Side = side;
        }

        public PString SetDefPos(Pos defpos) =>
            new PString(Value, Pos, defpos, Side);

        public PString SetPos(Pos pos) =>
            new PString(Value, pos, DefPos, Side);

        public PString SetSide(Sidedness side) =>
            new PString(Value, Pos, DefPos, side);

        public PString SetValue(string value) =>
            new PString(value, Pos, DefPos, Side);

        public override string ToString() =>
            Value;

        public static readonly PString Zero =
            new PString("", Pos.Zero, Pos.Zero, Sidedness.Onside);
    }
}
