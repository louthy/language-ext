using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public enum ParserErrorTag
    {
        Unknown,
        SysUnExpect,
        UnExpect,
        Expect,
        Message
    }

    public class ParserError
    {
        public readonly ParserErrorTag Tag;
        public readonly Pos Pos;
        public readonly string Msg;
        public readonly Lst<string> Expected;
        public readonly ParserError Inner;

        public ParserError(ParserErrorTag tag, Pos pos, string message, Lst<string> expected, ParserError inner)
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

        public ParserError Merge(ParserError inner) =>
            new ParserError(Tag, Pos, Msg, Expected, inner);

        public override string ToString() =>
            $"error at (line {Pos.Line + 1}, column {Pos.Column + 1}):\n" +
              ( Tag == ParserErrorTag.UnExpect    ? $"unexpected {Msg}"
              : Tag == ParserErrorTag.SysUnExpect ? $"unexpected {Msg}"
              : Tag == ParserErrorTag.Message     ? Msg
              : Tag == ParserErrorTag.Expect      ? $"unexpected {Msg}\nexpecting {String.Join(", ", Expected)}"
              : "unknown error");
    }
}
