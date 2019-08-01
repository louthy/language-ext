using System.Linq;
using System.Collections.Generic;
using LanguageExt;
//using LanguageExt.Trans;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using static LanguageExt.List;
using Xunit;
using LanguageExt.ClassInstances;
using System;

namespace LanguageExt.Tests
{

    public class OptionTTests
    {
        [Fact]
        public void TestLstT()
        {
            var x = List(Some(1), Some(2), None, Some(4));

            var r = LstTBind(x, (a => a % 2 == 0 ? Some(a) : None));

            Assert.True(r == List(None, Some(2), None, Some(4)));
        }

        [Fact]
        public void TestOptionT()
        {
            var x = Some(List(1, 2, 3, 4, 5));

            var r = OptionTBind(x, (a => a % 2 == 0 ? List(a) : List<int>()));

            Assert.True(r == Some(List(2, 4)));
        }

        static Trans<MLst<Option<int>>, Lst<Option<int>>, MOption<int>, Option<int>, int> OptionT;

        [Fact]
        public void TestOptionT2()
        {
            var list = List(Some(1), Some(2), Some(3), Some(4), Some(5));

            var newlist = OptionT.Bind<MLst<Option<int>>, Lst<Option<int>>, MOption<int>, Option<int>, int>(
                list, 
                x => x % 2 == 0 ? Some(x) : None
            );

            Assert.True(newlist == List(None, Some(2), None, Some(4), None));
        }

        [Fact]
        public void TestOptionT3()
        {
            var list = List(Some(1), Some(2), Some(3), Some(4), Some(5));

            var result = OptionT.Sum(list);

            Assert.True(result == 15);
        }

        [Fact]
        public void TestOptionT4()
        {
            var list = List(Some(1), Some(2), Some(3), Some(4), Some(5));

            var result = OptionT.Count(list);

            Assert.True(result == 5);
        }


        public static Lst<Option<B>> LstTBind<A, B>(Lst<Option<A>> list, Func<A, Option<B>> bind)
        {
            var LstT = default(Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>);

            return LstT.Bind<MLst<Option<B>>, Lst<Option<B>>, MOption<B>, Option<B>, B>(list, bind);
        }

        public static Option<Lst<B>> OptionTBind<A, B>(Option<Lst<A>> list, Func<A, Lst<B>> bind)
        {
            var OptionT = default(Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>);

            return OptionT.Bind<MOption<Lst<B>>, Option<Lst<B>>, MLst<B>, Lst<B>, B>(list, bind);
        }

        [Fact]
        public void WrappedListTest()
        {
            var opt = Some(List(1, 2, 3, 4, 5));
            var res = opt.FoldT(0, (s, v) => s + v);
            var mopt = opt.MapT(x => x * 2);
            var mres = mopt.FoldT(0, (s, v) => s + v);

            Assert.True(res == 15, "Expected 15, but got " + res);
            Assert.True(mres == 30, "Expected 30, but got " + mres);
            Assert.True(opt.CountT() == 5, "opt != 5, (" + opt.CountT() + ")");
            Assert.True(mopt.CountT() == 5, "mopt != 5, (" + mopt.CountT() + ")");

            opt = None;
            res = opt.FoldT(0, (s, v) => s + v);

            Assert.True(res == 0, "res != 0, got " + res);
            Assert.True(opt.CountT() == 0, "opt.Count() != 0, got " + opt.CountT());
        }

        //[Fact]
        //public void WrappedMapTest()
        //{
        //    var opt = Some(Map(Tuple(1, "A"), Tuple(2, "B"), Tuple(3, "C"), Tuple(4, "D"), Tuple(5, "E")));
        //    var res = opt.FoldT("", (s, v) => s + v);
        //    var mopt = opt.MapT(x => x.ToLower());
        //    var mres = mopt.FoldT("", (s, v) => s + v);

        //    Assert.True(res == "ABCDE");
        //    Assert.True(opt.CountT() == 5);
        //    Assert.True(mopt.CountT() == 5);

        //    match(mopt,
        //        Some: x =>
        //        {
        //            Assert.True(x[1] == "a");
        //            Assert.True(x[2] == "b");
        //            Assert.True(x[3] == "c");
        //            Assert.True(x[4] == "d");
        //            Assert.True(x[5] == "e");
        //        },
        //        None: () => Assert.False(true)
        //    );
        //}

        [Fact]
        public void WrappedListLinqTest()
        {
            var opt = Some(List(1, 2, 3, 4, 5));

            var res = from x in opt
                      from y in x
                      select y * 2;

            var total = res.Sum();

            Assert.True(total == 30);
        }

        [Fact]
        public void WrappedMapLinqTest()
        {
            var opt = Some(Map(Tuple(1, "A"), Tuple(2, "B"), Tuple(3, "C"), Tuple(4, "D"), Tuple(5, "E")));

            var res = from x in opt
                      from y in x
                      select y.Value.ToLower();

            var fd = res.FoldT("", (s, x) => s + x);

            Assert.True(fd == "abcde");
        }

        [Fact]
        public void WrappedOptionOptionLinqTest()
        {
            var opt = Some(Some(Some(100)));

            var res = from x in opt
                      from y in x
                      from z in y
                      select z * 2;

            Assert.True(equals<EqInt, int>(res, Some(200)));

            opt = Some(Some<Option<int>>(None));

            var res2 = from x in opt
                       from y in x
                       from z in y
                       select z * 2;

            Assert.False(equals<EqInt, int>(res, Some(0)));
        }

        [Fact]
        public void WrappedOptionLinqTest()
        {
            var opt = Some(Some(100));

            var res = from x in opt
                      from y in x
                      select y * 2;

            Assert.True(res.Map(x => x == 200).IfNone(false));

            opt = Some<Option<int>>(None);

            res = from x in opt
                  from y in x
                  select y * 2;

            var bopt = res.Map(x => x == 1);

            Assert.True(bopt.IfNone(true));
        }

        [Fact]
        public void WrappedTryOptionLinqTest()
        {
            var opt = Some(TryOption(() => Some(100)));

            var res = from x in opt
                      select x * 2;

            Assert.True(res.EqualsT<TInt, int>(Some(TryOption(200))));

            opt = Some(TryOption<int>(() => None));

            res = from x in opt
                  select x * 2;

            Assert.True(res.EqualsT<TInt, int>(Some(TryOption<int>(None))));
        }

        //[Fact]
        //public void WrappedEitherLinqTest()
        //{
        //    var opt = Some(Right<string, int>(100));

        //    var res = from x in opt
        //              from y in x
        //              select y * 2;

        //    Assert.True(res.LiftT() == 200);

        //    opt = Some(Left<string, int>("left"));

        //    res = from x in opt
        //          from y in x
        //          select y * 2;

        //    Assert.True(res.LiftT() == 0);
        //}

        [Fact]
        public void WrappedListOfOptionsTest1()
        {
            var opt = List(Some(1), Some(2), Some(3), Some(4), Some(5));

            opt = from x in opt
                  where x > 2
                  select x;

            Assert.True(opt.Count() == 5, "Count should be 5, is: " + opt.Count());
            Assert.True(equals<EqInt, int>(opt[0], None), "opt[1] != None. Is: " + opt[0]);
            Assert.True(equals<EqInt, int>(opt[1], None), "opt[2] != None. Is : " + opt[1]);
            Assert.True(equals<EqInt, int>(opt[2], Some(3)), "opt[3] != Some(3)");
            Assert.True(equals<EqInt, int>(opt[3], Some(4)), "opt[4] != Some(4)");
            Assert.True(equals<EqInt, int>(opt[4], Some(5)), "opt[5] != Some(5)");

            opt = opt.Filter(isSome);

            Assert.True(opt.Count() == 3, "Count should be 3, is: " + opt.Count());
            Assert.True(equals<EqInt, int>(opt[0], Some(3)), "opt[0] != Some(3)");
            Assert.True(equals<EqInt, int>(opt[1], Some(4)), "opt[1] != Some(4)");
            Assert.True(equals<EqInt, int>(opt[2], Some(5)), "opt[2] != Some(5)");

        }

        [Fact]
        public void WrappedListOfOptionsTest2()
        {
            var opt = List(Some(1), Some(2), Some(3), Some(4), Some(5));

            opt = opt.FilterT(x => x > 2);

            Assert.True(opt.Count() == 5, "Count should be 5, is: " + opt.Count());
            Assert.True(equals<TInt, int>(opt[0], Option<int>.None), "opt[0] != None. Is: " + opt[0]);
            Assert.True(equals<TInt, int>(opt[1], Option<int>.None), "opt[1] != None. Is: " + opt[1]);
            Assert.True(equals<TInt, int>(opt[2], Some(3)), "opt[2] != Some(3), Is: " + opt[2]);
            Assert.True(equals<TInt, int>(opt[3], Some(4)), "opt[3] != Some(4), Is: " + opt[3]);
            Assert.True(equals<TInt, int>(opt[4], Some(5)), "opt[4] != Some(5), Is: " + opt[4]);

            opt = opt.Filter(isSome);

            Assert.True(opt.Count() == 3, "Count should be 3, is: " + opt.Count());
            Assert.True(equals<TInt, int>(opt[0], Some(3)), "opt[0] != Some(3)");
            Assert.True(equals<TInt, int>(opt[1], Some(4)), "opt[1] != Some(4)");
            Assert.True(equals<TInt, int>(opt[2], Some(5)), "opt[2] != Some(5)");

        }
    }
}
