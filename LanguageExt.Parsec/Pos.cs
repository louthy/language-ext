using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// Represents a parser source position
    /// </summary>
    public class Pos : IEquatable<Pos>, IComparable<Pos>
    {
        public readonly int Line;
        public readonly int Column;

        public Pos(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public static readonly Pos Zero = new Pos(0, 0);

        public bool Equals(Pos other) =>
            Line == other.Line &&
            Column == other.Column;

        public override bool Equals(object obj) =>
            ((obj as Pos)?.Equals(this)).GetValueOrDefault();

        public override int GetHashCode() =>
            Tuple.Create(Line, Column).GetHashCode();

        public override string ToString() =>
            $"(line {Line + 1}, column {Column + 1})";

        public int CompareTo(Pos other) =>
            Line < other.Line
                ? -1
                : Line > other.Line
                    ? 1
                    : Column.CompareTo(other.Column);

        public static bool operator ==(Pos lhs, Pos rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Pos lhs, Pos rhs) =>
            !lhs.Equals(rhs);

        public static bool operator < (Pos lhs, Pos rhs) =>
            lhs.CompareTo(rhs) < 0;

        public static bool operator >(Pos lhs, Pos rhs) =>
            lhs.CompareTo(rhs) > 0;

        public static bool operator <=(Pos lhs, Pos rhs) =>
            lhs.CompareTo(rhs) <= 0;

        public static bool operator >=(Pos lhs, Pos rhs) =>
            lhs.CompareTo(rhs) >= 0;

        public static R Compare<R>(
            Pos lhs,
            Pos rhs,
            Func<R> EQ,
            Func<R> GT,
            Func<R> LT
            )
        {
            var res = lhs.CompareTo(rhs);
            if( res < 0)
            {
                return LT();
            }
            if (res > 0)
            {
                return GT();
            }
            return EQ();
        }
    }
}
