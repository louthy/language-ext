using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LanguageExt;
using M = LanguageExt.Map;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;
using LanguageExt.Config;
using LanguageExt.UnitsOfMeasure;

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
            var p = either(ch('a'), ch('1'));

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
            Assert.True(r.Reply.State.ToString() == "");
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
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void IntegerNegativeNumberComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Integer;
            var r = parse(p, "-1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == -1234);
            Assert.True(r.Reply.State.ToString() == "");
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

        //[Fact]
        //public void SettingTokenIntTest()
        //{
        //    var text = @"my-setting: 123";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting", FieldSpec.Int("value"))
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 1);
        //    Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.Int);
        //    Assert.True((int)setting.Values["value"].Value == 123);
        //}

        //[Fact]
        //public void SettingTokenDoubleTest()
        //{
        //    var text = @"my-setting: 123.45";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting", FieldSpec.Double("value"))
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 1);
        //    Assert.True(setting.Values["value"].Name == "value");
        //    Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.Double);
        //    Assert.True((double)setting.Values["value"].Value == 123.45);
        //}

        //[Fact]
        //public void SettingTokenStringTest()
        //{
        //    var text = @"my-setting: ""abc"" ";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting", FieldSpec.String("value"))
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 1);
        //    Assert.True(setting.Values["value"].Name == "value");
        //    Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.String);
        //    Assert.True((string)setting.Values["value"].Value == "abc");
        //}

        //[Fact]
        //public void SettingTokenTimeTest()
        //{
        //    var text = @"my-setting: 4 hours ";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting", FieldSpec.Time("value"))
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 1);
        //    Assert.True(setting.Values["value"].Name == "value");
        //    Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.Time);
        //    Assert.True((Time)setting.Values["value"].Value == 4 * hours);
        //}

        //[Fact]
        //public void SettingTokenProcessIdTest()
        //{
        //    var text = @"my-setting: ""/root/user/blah"" ";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting", FieldSpec.ProcessId("value"))
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 1);
        //    Assert.True(setting.Values["value"].Name == "value");
        //    Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.ProcessId);
        //    Assert.True((ProcessId)setting.Values["value"].Value == new ProcessId("/root/user/blah"));
        //}

        //[Fact]
        //public void SettingTokenProcessNameTest()
        //{
        //    var text = @"my-setting: ""root-proc-name"" ";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting", FieldSpec.ProcessName("value"))
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 1);
        //    Assert.True(setting.Values["value"].Name == "value");
        //    Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.ProcessName);
        //    Assert.True((ProcessName)setting.Values["value"].Value == new ProcessName("root-proc-name"));
        //}

        //[Fact]
        //public void SettingTokenProcessFlagsTest()
        //{
        //    var text = @"my-setting: [persist-inbox, persist-state, remote-publish] ";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting", FieldSpec.ProcessFlags("value"))
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 1);
        //    Assert.True(setting.Values["value"].Name == "value");
        //    Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.ProcessFlags);

        //    var flags = (ProcessFlags)setting.Values["value"].Value;
        //    Assert.True((flags & ProcessFlags.PersistInbox) != 0);
        //    Assert.True((flags & ProcessFlags.PersistState) != 0);
        //    Assert.True((flags & ProcessFlags.RemotePublish) != 0);
        //}

        //[Fact]
        //public void SettingTokenArrayIntTest()
        //{
        //    var text = @"my-setting: [1,2,3 , 4] ";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting", FieldSpec.Array("value", ArgumentType.Int))
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 1);
        //    Assert.True(setting.Values["value"].Name == "value");
        //    Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.Array);
        //    Assert.True(setting.Values["value"].Type.GenericType.Tag == ArgumentTypeTag.Int);

        //    var array = (Lst<int>)setting.Values["value"].Value;

        //    Assert.True(array.Count == 4);
        //    Assert.True(array[0] == 1);
        //    Assert.True(array[1] == 2);
        //    Assert.True(array[2] == 3);
        //    Assert.True(array[3] == 4);
        //}

        //[Fact]
        //public void SettingTokenArrayStringTest()
        //{
        //    var text = @"my-setting: [""hello"",""world""] ";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting", FieldSpec.Array("value", ArgumentType.String))
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 1);
        //    Assert.True(setting.Values["value"].Name == "value");
        //    Assert.True(setting.Values["value"].Type.Tag == ArgumentTypeTag.Array);
        //    Assert.True(setting.Values["value"].Type.GenericType.Tag == ArgumentTypeTag.String);

        //    var array = (Lst<string>)setting.Values["value"].Value;

        //    Assert.True(array.Count == 2);
        //    Assert.True(array[0] == "hello");
        //    Assert.True(array[1] == "world");
        //}

        //[Fact]
        //public void SettingTokenNamedArgsIntStringTest()
        //{
        //    var text = @"my-setting: max = 123, name = ""hello"" ";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting", FieldSpec.Int("max"), FieldSpec.String("name"))
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 2);

        //    var arg0 = setting.Values["max"];
        //    Assert.True(arg0.Name == "max");
        //    Assert.True(arg0.Type.Tag == ArgumentTypeTag.Int);
        //    Assert.True((int)arg0.Value == 123);

        //    var arg1 = setting.Values["name"];
        //    Assert.True(arg1.Name == "name");
        //    Assert.True(arg1.Type.Tag == ArgumentTypeTag.String);
        //    Assert.True((string)arg1.Value == "hello");
        //}

        //[Fact]
        //public void SettingTokenNamedArgsIntStringArrayTest()
        //{
        //    var text = @"my-setting: max = 123, name = ""hello"", coef = [0.1,0.3,0.5] ";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting",
        //                FieldSpec.Int("max"),
        //                FieldSpec.String("name"),
        //                FieldSpec.Array("coef", ArgumentType.Double)
        //                )
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 3);

        //    var arg0 = setting.Values["max"];
        //    Assert.True(arg0.Name == "max");
        //    Assert.True(arg0.Type.Tag == ArgumentTypeTag.Int);
        //    Assert.True((int)arg0.Value == 123);

        //    var arg1 = setting.Values["name"];
        //    Assert.True(arg1.Name == "name");
        //    Assert.True(arg1.Type.Tag == ArgumentTypeTag.String);
        //    Assert.True((string)arg1.Value == "hello");

        //    var arg2 = setting.Values["coef"];
        //    Assert.True(arg2.Name == "coef");
        //    Assert.True(arg2.Type.Tag == ArgumentTypeTag.Array);
        //    Assert.True(arg2.Type.GenericType.Tag == ArgumentTypeTag.Double);

        //    var array = (Lst<double>)arg2.Value;
        //    Assert.True(array.Count == 3);
        //    Assert.True(array[0] == 0.1);
        //    Assert.True(array[1] == 0.3);
        //    Assert.True(array[2] == 0.5);
        //}

        //[Fact]
        //public void SettingTokenNamedArgsIntArrayFlagsTest()
        //{
        //    var text = @"my-setting: max = 123, name = ""hello"", coef = [0.1,0.3,0.5], flags = [listen-remote-and-local] ";

        //    var sys = new ProcessSystemConfigParser(
        //            FuncSpec.Attr("my-setting",
        //                FieldSpec.Int("max"),
        //                FieldSpec.String("name"),
        //                FieldSpec.ProcessFlags("flags"),
        //                FieldSpec.Array("coef", ArgumentType.Double)
        //                )
        //            );

        //    var res = parse(sys.Settings, text);

        //    Assert.False(res.IsFaulted);

        //    var setting = res.Reply.Result["my-setting"];

        //    Assert.True(setting.Name == "my-setting");
        //    Assert.True(setting.Values.Length == 4);

        //    var arg0 = setting.Values["max"];
        //    Assert.True(arg0.Name == "max");
        //    Assert.True(arg0.Type.Tag == ArgumentTypeTag.Int);
        //    Assert.True((int)arg0.Value == 123);

        //    var arg1 = setting.Values["name"];
        //    Assert.True(arg1.Name == "name");
        //    Assert.True(arg1.Type.Tag == ArgumentTypeTag.String);
        //    Assert.True((string)arg1.Value == "hello");

        //    var arg2 = setting.Values["coef"];
        //    Assert.True(arg2.Name == "coef");
        //    Assert.True(arg2.Type.Tag == ArgumentTypeTag.Array);
        //    Assert.True(arg2.Type.GenericType.Tag == ArgumentTypeTag.Double);

        //    var arg3 = setting.Values["flags"];
        //    Assert.True(arg3.Name == "flags");
        //    Assert.True(arg3.Type.Tag == ArgumentTypeTag.ProcessFlags);
        //    Assert.True((ProcessFlags)arg3.Value == ProcessFlags.ListenRemoteAndLocal);
        //}

        [Fact]
        public void ProcessesSettingsParserTest()
        {
            var text = @"
            
                time timeout:           30 seconds
                time session-timeout:   60 seconds
                int mailbox-size:      10000

                strategy my-strategy:
                    one-for-one:
                        retries: count = 5, duration=30 seconds
                        backoff: min = 2 seconds, max = 1 hour, step = 5 seconds
                        
                        match
                         | System.NotImplementedException -> stop
                         | System.ArgumentNullException   -> escalate
                         | _                              -> restart

                        redirect when
                         | restart  -> forward-to-parent
                         | escalate -> forward-to-self
                         | stop     -> forward-to-process /root/test/567
                         | _        -> forward-to-dead-letters

                strategy my-other-strategy:
                    one-for-one:
                        pause: 1 second
                        always: resume
                        redirect: forward-to-process /root/test/567

                process abc:
                    pid:          /root/test/123
                    flags:        [persist-inbox, persist-state, remote-publish]
                    mailbox-size: 1000
                    strategy:     my-strategy

                process def:
                    pid:          /root/test/567
                    flags:        [persist-inbox, persist-state]
                    mailbox-size: 100
                    strategy:     my-other-strategy
                ";

            var config = new ProcessSystemConfig("test");

            var res = parse(config.Parser, text);

            Assert.False(res.IsFaulted);

            var result = res.Reply.Result;

            Assert.True(result.Count == 7);

            var timeout = result["timeout"];
            var session = result["session-timeout"];
            var mailbox= result["mailbox-size"];

            // Load process settings
            var processes = M.createRange(from val in result.Values
                                          where val.Spec.Args.Length > 0 && val.Spec.Args[0].Type.Tag == ArgumentTypeTag.Process
                                          let p = (ProcessToken)val.Values.Values.First().Value
                                          where p.ProcessId.IsSome
                                          let id = p.ProcessId.IfNone(ProcessId.None)
                                          select Tuple(id, p));

            var strats = M.createRange(from val in result.Values
                                       where val.Spec.Args.Length > 0 && val.Spec.Args[0].Type.Tag == ArgumentTypeTag.Strategy
                                       let s = (StrategyToken)val.Values.Values.First().Value
                                       select Tuple(val.Name, s));


            Assert.True(timeout.Name == "timeout");
            Assert.True(timeout.Values.Count == 1);
            Assert.True(timeout.Values["value"].Type.Tag == ArgumentTypeTag.Time);
            Assert.True((Time)timeout.Values["value"].Value == 30*seconds);

            Assert.True(session.Name == "session-timeout");
            Assert.True(session.Values.Count == 1);
            Assert.True(session.Values["value"].Type.Tag == ArgumentTypeTag.Time);
            Assert.True((Time)session.Values["value"].Value == 60 * seconds);

            Assert.True(mailbox.Name == "mailbox-size");
            Assert.True(mailbox.Values.Count == 1);
            Assert.True(mailbox.Values["value"].Type.Tag == ArgumentTypeTag.Int);
            Assert.True((int)mailbox.Values["value"].Value == 10000);

            Assert.True(strats.Count == 2);
            Assert.True(processes.Count == 2);

        }
    }
}
