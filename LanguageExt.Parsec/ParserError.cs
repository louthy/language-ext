using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class ParserError
    {
        public readonly Pos Pos;
        public readonly string Message;
        public readonly Lst<string> Expected;
        public readonly ParserError Inner;

        public ParserError(Pos pos, string message, Lst<string> expected, ParserError inner)
        {
            Pos = pos;
            Message = message;
            Expected = expected;
            Inner = inner;
        }

        public ParserError Expect(string expected) =>
            new ParserError(Pos, Message, List.create(expected), Inner);

        public ParserError Merge(ParserError inner) =>
            new ParserError(Pos, Message, Expected, inner);

        public override string ToString() =>
            $"parse error at (line {Pos.Line + 1}, column {Pos.Column + 1}):\n" +
                (String.IsNullOrEmpty(Message)
                    ? $"expecting {String.Join(", ", Expected)}\n"
                    : $"unexpected {Message}\n" +
                      $"expecting {String.Join(", ", Expected)}");
    }
}
