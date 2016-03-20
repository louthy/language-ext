using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec;

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
            Assert.True(r.Result == 1234);
        }

        [Fact]
        public void ZeroComb()
        {
            var p = zero<Unit>();
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void ItemComb()
        {
            var p = item;
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == 'H');
            Assert.True(r.Remaining.ToString() == "ello");
        }

        [Fact]
        public void ItemFailComb()
        {
            var p = item;
            var r = parse(p, "");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void Item2Comb()
        {
            var p = item;
            var r1 = parse(p, "Hello");

            Assert.False(r1.IsFaulted);
            Assert.True(r1.Results.Count() == 1);
            Assert.True(r1.Result == 'H');
            Assert.True(r1.Remaining.ToString() == "ello");

            var r2 = parse(p, r1.Remaining);

            Assert.False(r2.IsFaulted);
            Assert.True(r2.Results.Count() == 1);
            Assert.True(r2.Result == 'e');
            Assert.True(r2.Remaining.ToString() == "llo");

        }

        [Fact]
        public void Item1LinqComb()
        {
            var p = from x in item
                    select x;

            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == 'H');
            Assert.True(r.Remaining.ToString() == "ello");
        }

        [Fact]
        public void Item2LinqComb()
        {
            var p = from x in item
                    from y in item
                    select Tuple(x, y);

            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result.Item1 == 'H');
            Assert.True(r.Result.Item2 == 'e');
            Assert.True(r.Remaining.ToString() == "llo");
        }

        [Fact]
        public void EitherFirstComb()
        {
            var p = either(ch('a'),ch('1'));

            var r = parse(p, "a");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == 'a');
            Assert.True(r.Remaining.ToString() == "");
        }

        [Fact]
        public void EitherSecondComb()
        {
            var p = either(ch('a'), ch('1'));

            var r = parse(p, "1");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == '1');
            Assert.True(r.Remaining.ToString() == "");
        }

        [Fact]
        public void EitherLINQComb()
        {
            var p = from x in either(ch('a'), ch('1'))
                    from y in either(ch('a'), ch('1'))
                    select Tuple(x, y);

            var r = parse(p, "a1");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result.Item1 == 'a');
            Assert.True(r.Result.Item2 == '1');
            Assert.True(r.Remaining.ToString() == "");
        }

        [Fact]
        public void UpperComb()
        {
            var p = upper;
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == 'H');
            Assert.True(r.Remaining.ToString() == "ello");
        }

        [Fact]
        public void UpperFailComb()
        {
            var p = upper;
            var r = parse(p, "hello");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void LowerComb()
        {
            var p = lower;
            var r = parse(p, "hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == 'h');
            Assert.True(r.Remaining.ToString() == "ello");
        }

        [Fact]
        public void LowerFailComb()
        {
            var p = lower;
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void DigitComb()
        {
            var p = digit;
            var r = parse(p, "1234");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == '1');
            Assert.True(r.Remaining.ToString() == "234");
        }

        [Fact]
        public void DigitFailComb()
        {
            var p = digit;
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void LetterComb()
        {
            var p = letter;
            var r = parse(p, "hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == 'h');
            Assert.True(r.Remaining.ToString() == "ello");
        }

        [Fact]
        public void LetterFailComb()
        {
            var p = letter;
            var r = parse(p, "1ello");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void WordComb()
        {
            var p = word;
            var r = parse(p, "hello   ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == "hello");
            Assert.True(r.Remaining.ToString() == "   ");
        }

        [Fact]
        public void WordFailComb()
        {
            var p = word;
            var r = parse(p, "1ello  ");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void StringMatchComb()
        {
            var p = str("hello");
            var r = parse(p, "hello world");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == "hello");
            Assert.True(r.Remaining.ToString() == " world");
        }

        [Fact]
        public void StringMatchFailComb()
        {
            var p = str("hello");
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void NaturalNumberComb()
        {
            var p = natural;
            var r = parse(p, "1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == 1234);
            Assert.True(r.Remaining.ToString() == "  ");
        }

        [Fact]
        public void NaturalNumberFailComb()
        {
            var p = natural;
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void IntegerNumberComb()
        {
            var p = integer;
            var r = parse(p, "1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == 1234);
            Assert.True(r.Remaining.ToString() == "  ");
        }

        [Fact]
        public void IntegerNegativeNumberComb()
        {
            var p = integer;
            var r = parse(p, "-1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == -1234);
            Assert.True(r.Remaining.ToString() == "  ");
        }

        [Fact]
        public void IntegerNumberFailComb()
        {
            var p = integer;
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void BracketAndIntegerComb()
        {
            var p = brackets(integer);
            var r = parse(p, "[1]  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == 1);
            Assert.True(r.Remaining.ToString() == "");
        }

        [Fact]
        public void BracketAndIntegerFailComb()
        {
            var p = brackets(integer);
            var r = parse(p, "[x]  ");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void BracketAndIntegerListComb()
        {
            var p = commaBrackets(integer);
            var r = parse(p, "[1,2,3,4]  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);

            var arr = r.Result.ToArray();
            Assert.True(arr[0] == 1);
            Assert.True(arr[1] == 2);
            Assert.True(arr[2] == 3);
            Assert.True(arr[3] == 4);
            Assert.True(r.Remaining.ToString() == "");
        }

        [Fact]
        public void BracketAndSpacedIntegerListComb()
        {
            var p = commaBrackets(integer);
            var r = parse(p, "[ 1, 2 ,3,   4]  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);

            var arr = r.Result.ToArray();
            Assert.True(arr[0] == 1);
            Assert.True(arr[1] == 2);
            Assert.True(arr[2] == 3);
            Assert.True(arr[3] == 4);
            Assert.True(r.Remaining.ToString() == "");
        }

        [Fact]
        public void BracketAndIntegerListFailComb()
        {
            var p = commaBrackets(integer);
            var r = parse(p, "[1,x,3,4]  ");

            Assert.True(r.IsFaulted);
            Assert.True(r.Results.Count() == 0);
        }

        [Fact]
        public void JunkEmptyComb()
        {
            var p = junk;
            var r = parse(p, "");
            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == "");
        }

        [Fact]
        public void JunkNoMatchComb()
        {
            var p = junk;
            var r = parse(p, ",");
            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == "");
        }

        [Fact]
        public void JunkFourSpacesComb()
        {
            var p = junk;
            var r = parse(p, "    ,");
            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == "    ");
        }

        [Fact]
        public void JunkFourSpacesThenCommentComb()
        {
            var p = junk;
            var r = parse(p, "    // A comment\nabc");
            Assert.False(r.IsFaulted);
            Assert.True(r.Results.Count() == 1);
            Assert.True(r.Result == "     A comment");
            Assert.True(r.Remaining.ToString() == "abc");
        }

        [Fact]
        public void ActorConfigParserTest1()
        {
            var conftext = @"
                pid:   ""/root/test/123""
                flags: [persist-inbox, persist-state, remote-publish]
                strategy:
                    one-for-one:
                        retries:  count = 5, duration=30 seconds
                        back-off: min = 2 seconds, max = 1 hour, step = 5 seconds
                        
                        always: stop
                        redirect: forward-to-self

                        match
                        | System.NotImplementedException -> stop
                        | System.ArgumentNullException   -> escalate
                        | _                              -> restart

                        redirect
                        | restart  -> forward-to-parent
                        | escalate -> forward-to-self
                        | stop     -> forward-to-process ""/root/test/567""
                        | _        -> forward-to-dead-letters

                config:
                    blah:      ""Something for the process to use""
                    another:   ""Another setting""
            ";

            var res = parse(ActorConfigParser.Parser, conftext);

            Assert.False(res.IsFaulted);
            Assert.True(res.Results.Count() == 1);
            var conf = res.Result;
            var remain = res.Remaining;

            Assert.True(conf.Pid.Path == "/root/test/123");
            Assert.True((conf.Flags & ProcessFlags.PersistInbox) != 0);
            Assert.True((conf.Flags & ProcessFlags.PersistState) != 0);
            Assert.True((conf.Flags & ProcessFlags.RemotePublish) != 0);
        }
    }
}
