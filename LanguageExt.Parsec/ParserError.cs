using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    public enum ParserErrorTag
    {
        Unknown     = 0,        // numbered because the
        SysUnExpect = 1,        // order is important
        UnExpect    = 2,
        Expect      = 3,
        Message     = 4
    }

    public class ParserError : IEquatable<ParserError>, IComparable<ParserError>
    {
        public readonly ParserErrorTag Tag;
        public readonly Pos Pos;
        public readonly string Msg;
        public readonly Lst<string> Expected;
        public readonly ParserError Inner;

        public ParserError(ParserErrorTag tag, Pos pos, string message, Lst<string> expected, ParserError inner = null)
        {
            Tag = tag;
            Pos = pos;
            Msg = message;
            Expected = expected;
            Inner = inner;
        }

        public static ParserError Unknown(Pos pos) =>
            new ParserError(ParserErrorTag.Unknown, pos, "", List.empty<string>(), null);

        public static ParserError SysUnExpect(Pos pos, string message) =>
            new ParserError(ParserErrorTag.SysUnExpect, pos, message, List.empty<string>(), null);

        public static ParserError UnExpect(Pos pos, string message) =>
            new ParserError(ParserErrorTag.UnExpect, pos, message, List.empty<string>(), null);

        public static ParserError Expect(Pos pos, string message, string expected) =>
            new ParserError(ParserErrorTag.Expect, pos, message, List.create(expected), null);

        public static ParserError Message(Pos pos, string message) =>
            new ParserError(ParserErrorTag.Message, pos, message, List.empty<string>(), null);

        public override string ToString() =>
            $"{Tag} error at (line {Pos.Line + 1}, column {Pos.Column + 1}):\n" +
              ( Tag == ParserErrorTag.UnExpect    ? $"unexpected {Msg}"
              : Tag == ParserErrorTag.SysUnExpect ? $"unexpected {Msg}"
              : Tag == ParserErrorTag.Message     ? Msg
              : Tag == ParserErrorTag.Expect      ? $"unexpected {Msg}\nexpecting {String.Join(", ", Expected)}"
              : "unknown error");

        public bool Equals(ParserError other) =>
            other != null && Tag == other.Tag && Msg == other.Msg;

        public override bool Equals(object obj) =>
            ((obj as ParserError)?.Equals(this)).GetValueOrDefault();

        public override int GetHashCode() =>
            Tuple(Tag,Pos,Msg).GetHashCode();

        public int CompareTo(ParserError other) =>
            Tag.CompareTo(other.Tag);

        public static bool operator ==(ParserError lhs, ParserError rhs) =>
            isnull(lhs) && isnull(rhs)
                ? true
                : isnull(lhs) || isnull(rhs)
                    ? false
                    : !lhs.Equals(rhs);

        public static bool operator !=(ParserError lhs, ParserError rhs) =>
            !(lhs == rhs);

        public static bool operator <(ParserError lhs, ParserError rhs) =>
            lhs.CompareTo(rhs) < 0;

        public static bool operator >(ParserError lhs, ParserError rhs) =>
            lhs.CompareTo(rhs) > 0;

        public static bool operator <=(ParserError lhs, ParserError rhs) =>
            lhs.CompareTo(rhs) <= 0;

        public static bool operator >=(ParserError lhs, ParserError rhs) =>
            lhs.CompareTo(rhs) >= 0;

        public static R Compare<R>(
            ParserError lhs,
            ParserError rhs,
            Func<R> EQ,
            Func<R> GT,
            Func<R> LT
            )
        {
            var res = lhs.CompareTo(rhs);
            if (res < 0)
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
