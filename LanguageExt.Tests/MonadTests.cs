﻿using Xunit;
using LanguageExt;
using LanguageExt.Trans;
using LanguageExt.Trans.Linq;
using static LanguageExt.List;
using static LanguageExt.Prelude;
using System;
using System.Linq;

namespace LanguageExtTests
{
    
    public class MonadTests
    {
        [Fact]
        public void WriterTest()
        {
            var logNumber = fun((int x) => Writer(x, List("Got number: " + x)));

            var multWithLog = from a in logNumber(3)
                              from b in logNumber(5)
                              from _ in tell("Gonna multiply these two")
                              select a * b;

            var res = multWithLog();

            Assert.True(length(res.Output) == 3);
            Assert.True(res.Value == 15);
            Assert.True(head(res.Output) == "Got number: 3");
            Assert.True(head(tail(res.Output)) == "Got number: 5");
            Assert.True(head(tail(tail(res.Output))) == "Gonna multiply these two");
        }

        private class Bindings
        {
            public readonly Map<string, int> Map;
            public Bindings(params Tuple<string, int>[] items)
            {
                Map = Map(items);
            }

            public static Bindings New(params Tuple<string, int>[] items)
            {
                return new Bindings(items);
            }
        }

        [Fact]
        public void ReaderAskTest()
        {
            var lookupVar = fun((string name, Bindings bindings) => LanguageExt.Map.find(bindings.Map, name).IfNone(0));

            var calcIsCountCorrect = from count in asks((Bindings env) => lookupVar("count", env))
                                     from bindings in ask<Bindings>()
                                     select count == LanguageExt.Map.length(bindings.Map);

            var sampleBindings = Bindings.New(
                                    Tuple("count", 3),
                                    Tuple("1", 1),
                                    Tuple("b", 2)
                                    );

            bool res = calcIsCountCorrect(sampleBindings).Value;

            Assert.True(res);
        }

        [Fact]
        public void ReaderLocalTest()
        {
            var calculateContentLen = from content in ask<string>()
                                      select content.Length;

            var calculateModifiedContentLen = local(content => "Prefix " + content, calculateContentLen);

            var s = "12345";
            var modifiedLen = calculateModifiedContentLen(s);
            var len = calculateContentLen(s);

            Assert.True(modifiedLen.Value == 12);
            Assert.True(len.Value == 5);
        }

        [Fact]
        public void ReaderBottomTest()
        {
            var v1 = Reader<int, int>(10);
            var v2 = Reader<int, int>(10);

            var rdr = from x in v1
                      from y in v2
                      from c in ask<int>()
                      where x * c > 50 && y * c > 50
                      select (x + y) * c;

            Assert.True(rdr(10).Value == 200);
            Assert.True(rdr(2).Value == 0);
            Assert.True(rdr(2).IsBottom);
        }

        [Fact]
        public void StateTest()
        {
            var comp = from inp in get<string>()
                       let hw = inp + ", world"
                       from _ in put(hw)
                       select hw.Length;

            var r = comp("hello");

            Assert.True(r.Value == 12);
            Assert.True(r.State == "hello, world");
        }

        [Fact]
        public void StateBottomTest()
        {
            var v1 = State<int, int>(10);
            var v2 = State<int, int>(10);

            var rdr = from x in v1
                      from y in v2
                      from c in get<int>()
                      where x * c > 50 && y * c > 50
                      select (x + y) * c;

            Assert.True(rdr(10).Value == 200);
            Assert.True(rdr(2).Value == 0);
            Assert.True(rdr(2).IsBottom);
        }

        [Fact]
        public void StateBottomTest2()
        {
            var v1 = State<int, int>(10);
            var v2 = State<int, int>(10);

            var rdr = from x in v1
                      from c in get<int>()
                      where x * c > 50 
                      from y in v2
                      select (x + y) * c;

            Assert.True(rdr(10).Value == 200);
            Assert.True(rdr(2).Value == 0);
            Assert.True(rdr(2).IsBottom);
        }

        [Fact]
        public void ReaderListSumFoldTest()
        {
            var v1 = Reader<int, Lst<int>>(List(1, 2, 3, 4, 5));
            var v2 = Reader<int, Lst<int>>(List(1, 2, 3, 4, 5));

            var rdr = from x in v1.SumT()
                      from y in v2.FoldT(0, (s, v) => s + v * 2)
                      from c in ask<int>()
                      where x * c > 50 && y * c > 50
                      select (x + y) * c;

            Assert.True(rdr(10) == 450);
            Assert.True(rdr(2) == 0);
            Assert.True(rdr(2).IsBottom);
        }

        [Fact]
        public void LiftTest()
        {
            var x = List(None, Some(1), Some(2), Some(3), Some(4));

            Assert.True(x.Lift() == None);
            Assert.True(x.LiftT() == 0);

            var y = List(Some(1), Some(2), Some(3), Some(4));

            Assert.True(y.Lift() == Some(1));
            Assert.True(y.LiftT() == 1);

            var z = Some(Some(Some(Some(1))));

            Assert.True(z.LiftT().Lift() == Some(1));
            Assert.True(z.LiftT().LiftT() == 1);
        }

        [Fact]
        public void ReaderTryOptionLinqTest()
        {
            var res = from x in ask<string>()
                      from y in Hello(x)
                      select y;

            Assert.True(res.LiftT("World") == "Hello, World");

            var res2 = from x in ask<string>()
                       from y in NoWorky(x)
                       select y;

            Assert.True(res2.LiftT("World").IsNone);
        }

        [Fact]
        public void ReaderTryErrorLinqTest()
        {
            var res = tryread(() => 
                from x in ask<string>()
                from y in Hello(Error())
                select y
                );

            Assert.True(res.LiftT("World").IsNone);

            var res2 =
                from x in ask<string>()
                from y in tryfun(() => Hello(Error()))
                select y;

            Assert.True(res2.LiftT("World").IsNone);

            var res3 =
                from x in ask<string>()
                from y in tryfun(() => Hello2(Error()))
                select y;

            res3.LiftUnsafe("World").IfSome(x => failwith<Unit>("wrong"));

        }

        [Fact]
        public void ReaderWriterBindTest()
        {
            var x = from a in ask<string>()
                    from b in tell("Hello " + a)
                    select b;

            Assert.True(x("everyone").Value().Output.Head() == "Hello everyone");
        }

        [Fact]
        public void TryReaderBindTest()
        {
            var tryadd = from a in Try(() => 123)
                         from b in ask<int>()
                         select a + b;

            var res = tryadd.Map(r => r.Lift(123))
                            .Lift();

            Assert.True(res == 246);
        }

        [Fact]
        public void SomeLiftTest()
        {
            var z = Some(Some(10));
            var r = z.LiftT();
            Assert.True(r == 10);
        }

        [Fact]
        public void FilterTTest()
        {
            var o = Some(List(1, 2, 3, 4, 5));
            var o2 = o.FilterT(n => n > 2);

            Assert.True(o2.Count() == 1);
            Assert.True(o2.CountT() == 3);
        }


        [Fact]
        public void ReaderListSumTest()
        {
            var r2 = from v in ask<int>()
                     from l in List(1, 2, 3, 4, 5)
                     select l * v;

            var r3 = r2.SumT()(10);

            Assert.True(r3 == 150);
        }

        private static string Error()
        {
            throw new Exception("Nooooo");
        }

        private static TryOption<string> Hello2(string who) => () => Some("Hello, " + who);
        private static Try<Option<string>> Hello(string who) => () => Some("Hello, " + who);
        private static Try<Option<string>> NoWorky(string who) => () => Some(failwith<string>("fail!"));
    }
}