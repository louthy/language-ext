using Xunit;
using System;
using LanguageExt;
using static LanguageExt.List;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests
{
    
    public class MonadTests
    {
        [Fact]
        public void WriterTest()
        {
            var telll = fun((string x) => tell<MLst<string>, Lst<string>>(List(x)));

            var value = fun((int x) => from _ in telll($"Got number: {x}")
                                       select x);

            var multWithLog = from a in value(3)
                              from b in value(5)
                              from _ in telll("Gonna multiply these two")
                              select a * b;

            var res = multWithLog.Run();

            Assert.True(length(res.Output) == 3);
            Assert.True(res.Value.IfNoneOrFail(0) == 15);
            Assert.True(head(res.Output) == "Got number: 3");
            Assert.True(head(tail(res.Output)) == "Got number: 5");
            Assert.True(head(tail(tail(res.Output))) == "Gonna multiply these two");
        }

        static Writer<MSeq<string>, Seq<string>, int> writer(int value, Seq<string> output) => () =>
            (value, output, false);

        static Writer<MSeq<string>, Seq<string>, Seq<int>> multWithLog(Seq<int> input) =>
            from _ in writer(0, Seq1("Start"))
            let c = input.Map(i => writer(i * 10, Seq1($"Number: {i}")))
            from r in c.Sequence()
            select r;

        [Fact]
        public void WriterSequenceTest()
        {
            var res = multWithLog(Seq(1, 2, 3)).Run();

            Assert.True(res.Value.IfNoneOrFail(Seq<int>()) == Seq(10, 20, 30));
            Assert.True(res.Output == Seq("Start", "Number: 1", "Number: 2", "Number: 3"));
        }

        private class Bindings
        {
            public readonly Map<string, int> Map;
            public Bindings(params Tuple<string, int>[] items)
            {
                Map = toMap(items);
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

            bool res = calcIsCountCorrect.Run(sampleBindings).IfNoneOrFail(false);

            Assert.True(res);
        }

        [Fact]
        public void ReaderLocalTest()
        {
            var calculateContentLen = from content in ask<string>()
                                      select content.Length;

            var calculateModifiedContentLen = local(calculateContentLen, content => "Prefix " + content);

            var s = "12345";
            var modifiedLen = calculateModifiedContentLen.Run(s).IfNoneOrFail(0);
            var len = calculateContentLen.Run(s).IfNoneOrFail(0);

            Assert.True(modifiedLen == 12);
            Assert.True(len == 5);
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

            Assert.True(rdr.Run(10).IfNoneOrFail(0) == 200);
            Assert.True(rdr.Run(2).IfNoneOrFail(0) == 0);
        }

        [Fact]
        public void StateTest()
        {
            var comp = from inp in get<string>()
                       let hw = inp + ", world"
                       from _ in put(hw)
                       select hw.Length;

            var r = comp.Run("hello");

            Assert.True(r.Value.IfNoneOrFail(0) == 12);
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

            Assert.True(rdr.Run(10).Value.IfNoneOrFail(0) == 200);
            Assert.True(rdr.Run(2).Value.IfNoneOrFail(0) == 0);
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

            Assert.True(rdr.Run(10).Value.IfNoneOrFail(0) == 200);
            Assert.True(rdr.Run(2).Value.IfNoneOrFail(0) == 0);
        }

        [Fact]
        public void RWSTest()
        {
            var previous = RWS<MLst<string>, bool, Lst<string>,  Map<int, string>, int>(1);
            var comp =
                from val in previous
                from state in get<MLst<string>, bool, Lst<string>, Map<int, string>, int>()
                from env in ask<MLst<string>, bool, Lst<string>, Map<int, string>>()
                from _ in put<MLst<string>, bool, Lst<string>, Map<int, string>, int>(state.Add(2, "B"))
                from __ in tell<MLst<string>, bool, Lst<string>, Map<int, string>, int>(List($"{val}", $"{env}"))
                select val + 2;

            var (value, output, finalState, faulted) = comp(true, Map((1, "a")));
            Assert.True(value == 3);
            Assert.True(output == List("1", "True"));
            Assert.True(finalState == Map((1, "a"), (2, "B")));
        }

        [Fact]
        public void RWSFailTest()
        {
            var previous = RWS<MLst<string>, bool, Lst<string>,  Map<int, string>, int>(1);
            var comp =
                from val in previous
                from _ in modify<MLst<string>, bool, Lst<string>, Map<int, string>, int>(_ => failwith<Map<int,string>>(""))
                select val + 2;

            Assert.ThrowsAny<Exception>(act(comp.Run(false, Map((1, "a"))).Value.IfFailThrow));
        }

        [Fact]
        public void RWSBottomTest()
        {
            var previous = RWS<MLst<string>, bool, Lst<string>,  Map<int, string>, int>(1);
            var comp =
                from val in previous
                where false
                select val + 3;

            Assert.ThrowsAny<Exception>(act(comp.Run(false, Map((1, "a"))).Value.IfFailThrow));
        }

        // TODO: Restore when type-classes are complete
        //[Fact]
        //public void ReaderListSumFoldTest()
        //{
        //    var v1 = Reader<int, Lst<int>>(List(1, 2, 3, 4, 5));
        //    var v2 = Reader<int, Lst<int>>(List(1, 2, 3, 4, 5));

        //    var rdr = from x in v1.SumT()
        //              from y in v2.FoldT(0, (s, v) => s + v * 2)
        //              from c in ask<int>()
        //              where x * c > 50 && y * c > 50
        //              select (x + y) * c;

        //    Assert.True(rdr(10) == 450);
        //    Assert.True(rdr(2) == 0);
        //    Assert.True(rdr(2).IsBottom);
        //}

        //[Fact]
        //public void ReaderTryOptionLinqTest()
        //{
        //    var res = from x in ask<string>()
        //              from y in Hello(x).Map(MReader<string, string>.Inst.Return)
        //              select y;

        //    Assert.True(res.LiftT("World") == "Hello, World");

        //    var res2 = from x in ask<string>()
        //               from y in NoWorky(x)
        //               select y;

        //    Assert.True(res2.LiftT("World").IsNone);
        //}

        //[Fact]
        //public void ReaderTryErrorLinqTest()
        //{
        //    var res = tryread(() => 
        //        from x in ask<string>()
        //        from y in Hello(Error())
        //        select y
        //        );

        //    Assert.True(res.LiftT("World").IsNone);

        //    var res2 =
        //        from x in ask<string>()
        //        from y in tryfun(() => Hello(Error()))
        //        select y;

        //    Assert.True(res2.LiftT("World").IsNone);

        //    var res3 =
        //        from x in ask<string>()
        //        from y in tryfun(() => Hello2(Error()))
        //        select y;

        //    res3.LiftUnsafe("World").IfSome(x => failwith<Unit>("wrong"));

        //}

        //[Fact]
        //public void ReaderWriterBindTest()
        //{
        //    var x = from a in ask<string>()
        //            from b in tell("Hello " + a)
        //            select b;

        //    Assert.True(x("everyone").Value().Output.Head() == "Hello everyone");
        //}

        //[Fact]
        //public void TryReaderBindTest()
        //{
        //    var tryadd = from a in Try(() => 123)
        //                 from b in ask<int>()
        //                 select a + b;

        //    var res = tryadd.Map(r => r.Lift(123))
        //                    .Lift();

        //    Assert.True(res == 246);
        //}

        //[Fact]
        //public void SomeLiftTest()
        //{
        //    var z = Some(Some(10));
        //    var r = z.LiftT();
        //    Assert.True(r == 10);
        //}

        //[Fact]
        //public void FilterTTest()
        //{
        //    var o = Some(List(1, 2, 3, 4, 5));
        //    var o2 = o.FilterT(n => n > 2);

        //    Assert.True(o2.Count() == 1);
        //    Assert.True(o2.CountT() == 3);
        //}


        //[Fact]
        //public void ReaderListSumTest()
        //{
        //    var r2 = from v in ask<int>()
        //             from l in List(1, 2, 3, 4, 5)
        //             select l * v;

        //    var r3 = r2.SumT()(10);

        //    Assert.True(r3 == 150);
        //}

        //private static string Error()
        //{
        //    throw new Exception("Nooooo");
        //}

        private static TryOption<string> Hello2(string who) => () => Some("Hello, " + who);
        private static Try<Option<string>> Hello(string who) => () => Some("Hello, " + who);
        private static Try<Option<string>> NoWorky(string who) => () => Some(failwith<string>("fail!"));
    }
}
