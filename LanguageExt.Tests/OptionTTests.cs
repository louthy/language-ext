using static LanguageExt.Prelude;
using static LanguageExt.List;
using LanguageExt;
using LanguageExt.Trans;
using LanguageExt.Trans.Linq;

using NU = NUnit.Framework;

namespace LanguageExtTests
{
    [NU.TestFixture]
    public class OptionTTests
    {
        [NU.Test]
        public void WrappedListTest()
        {
            var opt = Some(List(1, 2, 3, 4, 5));
            var res = foldT(opt, 0, (s, v) => s + v);
            var mopt = mapT(opt, x => x * 2);
            var mres = foldT(mopt, 0, (s, v) => s + v);

            NU.Assert.IsTrue(res == 15, "Expected 15, but got " + res);
            NU.Assert.IsTrue(mres == 30, "Expected 30, but got " + mres);
            NU.Assert.IsTrue(opt.CountT() == 5, "opt != 5, (" + opt.CountT() + ")");
            NU.Assert.IsTrue(mopt.CountT() == 5, "mopt != 5, (" + mopt.CountT() + ")");

            opt = None;
            res = opt.FoldT(0, (s, v) => s + v);

            NU.Assert.IsTrue(res == 0, "res != 0, got " + res);
            NU.Assert.IsTrue(opt.CountT() == 0, "opt.Count() != 0, got " + opt.CountT());
        }

        [NU.Test]
        public void WrappedMapTest()
        {
            var opt = Some(Map(Tuple(1, "A"), Tuple(2, "B"), Tuple(3, "C"), Tuple(4, "D"), Tuple(5, "E")));
            var res = opt.FoldT("", (s, v) => s + v);
            var mopt = opt.MapT(x => x.ToLower());
            var mres = mopt.FoldT("", (s, v) => s + v);

            NU.Assert.IsTrue(res == "ABCDE");
            NU.Assert.IsTrue(opt.CountT() == 5);
            NU.Assert.IsTrue(mopt.CountT() == 5);

            match(mopt,
                Some: x =>
                {
                    NU.Assert.IsTrue(x[1] == "a");
                    NU.Assert.IsTrue(x[2] == "b");
                    NU.Assert.IsTrue(x[3] == "c");
                    NU.Assert.IsTrue(x[4] == "d");
                    NU.Assert.IsTrue(x[5] == "e");
                },
                None: () => NU.Assert.Fail()
            );
        }

        [NU.Test]
        public void WrappedListLinqTest()
        {
            var opt = Some(List(1, 2, 3, 4, 5));

            var res = from x in opt
                      select x * 2;

            var total = sumT(res);

            NU.Assert.IsTrue(total == 30);
        }

        [NU.Test]
        public void WrappedMapLinqTest()
        {
            var opt = Some(Map(Tuple(1, "A"), Tuple(2, "B"), Tuple(3, "C"), Tuple(4, "D"), Tuple(5, "E")));

            var res = from x in opt
                      select x.ToLower();

            var fd = foldT(res, "", (s, x) => s + x);

            NU.Assert.IsTrue(fd == "abcde");
        }

        [NU.Test]
        public void WrappedOptionOptionLinqTest()
        {
            var opt = Some(Some(Some(100)));

            var res = from x in opt
                      from y in x
                      select y * 2;

            NU.Assert.IsTrue(res.LiftT() == 200);

            opt = Some(Some<Option<int>>(None));

            res = from x in opt
                  from y in x
                  select y * 2;

            NU.Assert.IsTrue(res.LiftT() == 0);
        }

        [NU.Test]
        public void WrappedOptionLinqTest()
        {
            var opt = Some(Some(100));

            var res = from x in opt
                      select x * 2;

            NU.Assert.IsTrue(res.LiftT() == 200);

            opt = Some<Option<int>>(None);

            res = from x in opt
                  select x * 2;

            NU.Assert.IsTrue(res.LiftT() == 0);
        }

        [NU.Test]
        public void WrappedTryOptionLinqTest()
        {
            Option<TryOption<int>> opt = Some<TryOption<int>>(() => Some(100));

            var res = from x in opt
                      select x * 2;

            NU.Assert.IsTrue(res.LiftT() == 200);

            opt = Some<TryOption<int>>(() => None);

            res = from x in opt
                  select x * 2;

            NU.Assert.IsTrue(res.LiftT() == 0);
        }

        [NU.Test]
        public void WrappedEitherLinqTest()
        {
            var opt = Some(Right<string, int>(100));

            var res = from x in opt
                      select x * 2;

            NU.Assert.IsTrue(res.LiftT() == 200);

            opt = Some(Left<string, int>("left"));

            res = from x in opt
                  select x * 2;

            NU.Assert.IsTrue(res.LiftT() == 0);
        }

        [NU.Test]
        public void WrappedListOfOptionsTest1()
        {
            var opt = List(Some(1), Some(2), Some(3), Some(4), Some(5));

            opt = from x in opt
                  where x > 2
                  select x;

            NU.Assert.IsTrue(opt.Count() == 5, "Count should be 5, is: " + opt.Count());
            NU.Assert.IsTrue(opt[0] == None, "opt[1] != None. Is: "+opt[0]);
            NU.Assert.IsTrue(opt[1] == None, "opt[2] != None. Is : " + opt[1]);
            NU.Assert.IsTrue(opt[2] == Some(3), "opt[3] != Some(3)");
            NU.Assert.IsTrue(opt[3] == Some(4), "opt[4] != Some(4)");
            NU.Assert.IsTrue(opt[4] == Some(5), "opt[5] != Some(5)");

            opt = opt.Filter(isSome);

            NU.Assert.IsTrue(opt.Count() == 3, "Count should be 3, is: " + opt.Count());
            NU.Assert.IsTrue(opt[0] == Some(3), "opt[0] != Some(3)");
            NU.Assert.IsTrue(opt[1] == Some(4), "opt[1] != Some(4)");
            NU.Assert.IsTrue(opt[2] == Some(5), "opt[2] != Some(5)");

        }

        [NU.Test]
        public void WrappedListOfOptionsTest2()
        {
            var opt = List(Some(1), Some(2), Some(3), Some(4), Some(5));

            opt = opt.FilterT(x => x > 2);

            NU.Assert.IsTrue(opt.Count() == 5, "Count should be 5, is: " + opt.Count());
            NU.Assert.IsTrue(opt[0] == None, "opt[1] != None. Is: " + opt[0]);
            NU.Assert.IsTrue(opt[1] == None, "opt[2] != None. Is: " + opt[1]);
            NU.Assert.IsTrue(opt[2] == Some(3), "opt[3] != Some(3), Is: "+ opt[2]);
            NU.Assert.IsTrue(opt[3] == Some(4), "opt[4] != Some(4), Is: " + opt[3]);
            NU.Assert.IsTrue(opt[4] == Some(5), "opt[5] != Some(5), Is: " + opt[4]);

            opt = opt.Filter(isSome);

            NU.Assert.IsTrue(opt.Count() == 3, "Count should be 3, is: " + opt.Count());
            NU.Assert.IsTrue(opt[0] == Some(3), "opt[0] != Some(3)");
            NU.Assert.IsTrue(opt[1] == Some(4), "opt[1] != Some(4)");
            NU.Assert.IsTrue(opt[2] == Some(5), "opt[2] != Some(5)");

        }

        /*
        [NU.Test]
        public void WrappedEitherLinqTest()
        {
            var x = list(1, 2, 3);
            var y = Some(100);
            var z = Some(100);

            var r = from a in x
                    from b in y
                    select a * b;

            var o = from a in y
                    from b in z
                    select a * b;
        }*/
    }
}