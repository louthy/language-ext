using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;

namespace LanguageExtTests
{
    public class ParsecTests
    {
        [Fact]
        public void ResultComb()
        {
            var p = result(1234);
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1234);
        }

        [Fact]
        public void ZeroComb()
        {
            var p = zero<Unit>();
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void ItemComb()
        {
            var p = anyChar;
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'H');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void ItemFailComb()
        {
            var p = anyChar;
            var r = parse(p, "");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void Item2Comb()
        {
            var p = anyChar;
            var r1 = parse(p, "Hello");

            Assert.False(r1.IsFaulted);
            Assert.True(r1.Reply.Result == 'H');
            Assert.True(r1.Reply.State.ToString() == "ello");

            var r2 = parse(p, r1.Reply.State);

            Assert.False(r2.IsFaulted);
            Assert.True(r2.Reply.Result == 'e');
            Assert.True(r2.Reply.State.ToString() == "llo");

        }

        [Fact]
        public void Item1LinqComb()
        {
            var p = from x in anyChar
                    select x;

            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'H');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void Item2LinqComb()
        {
            var p = from x in anyChar
                    from y in anyChar
                    select Tuple(x, y);

            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result.Item1 == 'H');
            Assert.True(r.Reply.Result.Item2 == 'e');
            Assert.True(r.Reply.State.ToString() == "llo");
        }

        [Fact]
        public void EitherFirstComb()
        {
            var p = either(ch('a'),ch('1'));

            var r = parse(p, "a");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'a');
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void EitherSecondComb()
        {
            var p = either(ch('a'), ch('1'));

            var r = parse(p, "1");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == '1');
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void EitherLINQComb()
        {
            var p = from x in either(ch('a'), ch('1'))
                    from y in either(ch('a'), ch('1'))
                    select Tuple(x, y);

            var r = parse(p, "a1");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result.Item1 == 'a');
            Assert.True(r.Reply.Result.Item2 == '1');
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void UpperComb()
        {
            var p = upper;
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'H');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void UpperFailComb()
        {
            var p = upper;
            var r = parse(p, "hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void LowerComb()
        {
            var p = lower;
            var r = parse(p, "hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'h');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void LowerFailComb()
        {
            var p = lower;
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void DigitComb()
        {
            var p = digit;
            var r = parse(p, "1234");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == '1');
            Assert.True(r.Reply.State.ToString() == "234");
        }

        [Fact]
        public void DigitFailComb()
        {
            var p = digit;
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void LetterComb()
        {
            var p = letter;
            var r = parse(p, "hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'h');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void LetterFailComb()
        {
            var p = letter;
            var r = parse(p, "1ello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void WordComb()
        {
            var p = asString(many1(letter));
            var r = parse(p, "hello   ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == "hello");
            Assert.True(r.Reply.State.ToString() == "   ");
        }

        [Fact]
        public void WordFailComb()
        {
            var p = asString(many1(letter));
            var r = parse(p, "1ello  ");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void StringMatchComb()
        {
            var p = str("hello");
            var r = parse(p, "hello world");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == "hello");
            Assert.True(r.Reply.State.ToString() == " world");
        }

        [Fact]
        public void StringMatchFailComb()
        {
            var p = str("hello");
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void NaturalNumberComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Natural;
            var r = parse(p, "1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1234);
            Assert.True(r.Reply.State.ToString() == "  ");
        }

        [Fact]
        public void NaturalNumberFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Natural;
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void IntegerNumberComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Integer;
            var r = parse(p, "1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1234);
            Assert.True(r.Reply.State.ToString() == "  ");
        }

        [Fact]
        public void IntegerNegativeNumberComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Integer;
            var r = parse(p, "-1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == -1234);
            Assert.True(r.Reply.State.ToString() == "  ");
        }

        [Fact]
        public void IntegerNumberFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Integer;
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void BracketAndIntegerComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = from x in tok.Brackets(tok.Integer)
                    from _ in tok.WhiteSpace
                    select x;

            var r = parse(p, "[1]  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void BracketAndIntegerFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Brackets(tok.Integer);
            var r = parse(p, "[x]  ");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void BracketAndIntegerListComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = from x in tok.BracketsCommaSep(tok.Integer)
                    from _ in tok.WhiteSpace
                    select x;
            var r = parse(p, "[1,2,3,4]  ");

            Assert.False(r.IsFaulted);

            var arr = r.Reply.Result.ToArray();
            Assert.True(arr[0] == 1);
            Assert.True(arr[1] == 2);
            Assert.True(arr[2] == 3);
            Assert.True(arr[3] == 4);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void BracketAndSpacedIntegerListComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = from x in tok.BracketsCommaSep(tok.Integer)
                    from _ in tok.WhiteSpace
                    select x;

            var r = parse(p, "[ 1, 2 ,3,   4]  ");

            Assert.False(r.IsFaulted);

            var arr = r.Reply.Result.ToArray();
            Assert.True(arr[0] == 1);
            Assert.True(arr[1] == 2);
            Assert.True(arr[2] == 3);
            Assert.True(arr[3] == 4);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void BracketAndIntegerListFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.BracketsCommaSep(tok.Integer);
            var r = parse(p, "[1,x,3,4]  ");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void JunkEmptyComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.WhiteSpace;
            var r = parse(p, "");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
        }

        [Fact]
        public void JunkNoMatchComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.WhiteSpace;
            var r = parse(p, ",");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
        }

        [Fact]
        public void JunkFourSpacesComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.WhiteSpace;
            var r = parse(p, "    ,");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
        }

        [Fact]
        public void JunkFourSpacesThenCommentComb()
        {
            var tok = makeTokenParser(Language.JavaStyle);
            var p = tok.WhiteSpace;
            var r = parse(p, "    // A comment\nabc");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
            Assert.True(r.Reply.State.ToString() == "abc");
        }

        [Fact]
        public void StringLiteralComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.StringLiteral;
            var r = parse(p, "\"/abc\"");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == "/abc");
        }

        [Fact]
        public void ActorConfigParserTest1()
        {
            var conftext = @"
                pid:          ""/root/test/123""
                flags:        [persist-inbox, persist-state, remote-publish]
                mailbox-size: 1000

                strategy:
                    one-for-one:

                        retries (count = 5, duration=30 seconds)
                        back-off (min = 2 seconds, max = 1 hour, step = 5 seconds)
                        
                        match
                        | System.NotImplementedException -> stop
                        | System.ArgumentNullException   -> escalate
                        | _                              -> restart

                        redirect when
                        | restart  -> forward-to-parent
                        | escalate -> forward-to-self
                        | stop     -> forward-to-process ""/root/test/567""
                        | _        -> forward-to-dead-letters

                settings:
                    blah:    ""Something for the process to use""
                    another: ""Another setting""
            ";

            var res = parse(ActorConfigParser.Parser, conftext);

            Assert.False(res.IsFaulted);
            var conf = res.Reply.Result;
            var remain = res.Reply.State;

            Assert.True(conf.Pid.Path == "/root/test/123");
            Assert.True((conf.Flags & ProcessFlags.PersistInbox) != 0);
            Assert.True((conf.Flags & ProcessFlags.PersistState) != 0);
            Assert.True((conf.Flags & ProcessFlags.RemotePublish) != 0);
            Assert.True(conf.MailboxSize == 1000);
            Assert.True(conf.Settings["blah"] == "Something for the process to use");
            Assert.True(conf.Settings["another"] == "Another setting");
            Assert.True(res.Reply.State.ToString() == "");
        }

        [Fact]
        public void ActorConfigParserFailTest1()
        {
            var conftext = @"

                pidy:          ""/root/test/123""
                flagsy:        [persist-inbox, persist-state, remote-publish]
                mailbox-sizey: 1000
                settingsy:
                    blah:    ""Something for the process to use""
                    another: ""Another setting""
            ";

            var res = parse(ActorConfigParser.Parser, conftext);

            Assert.True(res.IsFaulted);

            //Assert.True(res.Reply.Error.Message.StartsWith("Expected: 'pid', 'flags'"));
            //Assert.True(res.Reply.Error.Location.Pos.Column == 16);
            //Assert.True(res.Reply.Error.Location.Pos.Line == 2);
        }

        [Fact]
        public void ActorConfigParserFailTest2()
        {
            var conftext = @"

                pid:          ""/root/test/123""
                flags:        [persist-inbox, PersisT-State, remote-publish]
                mailbox-size: 1000
                settings:
                    blah:    ""Something for the process to use""
                    another: ""Another setting""
            ";

            var res = parse(ActorConfigParser.Parser, conftext);

            Assert.True(res.IsFaulted);

            //Assert.True(res.Reply.Error.Message.StartsWith("Expected: 'pid', 'flags'"));
            //Assert.True(res.Reply.Error.Location.Pos.Column == 16);
            //Assert.True(res.Reply.Error.Location.Pos.Line == 2);
        }
    }
}
