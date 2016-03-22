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

        public ParserError(Pos pos, string message, Lst<string> expected)
        {
            Pos = pos;
            Message = message;
            Expected = expected;
        }

        public ParserError Expect(string expected) =>
            new ParserError(Pos, Message, List.create(expected));
    }
}
