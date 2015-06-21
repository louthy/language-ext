using static LanguageExt.Prelude;
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
            var opt = Some(list(1, 2, 3, 4, 5));
            var res = opt.FoldT(0, (s, v) => s + v);
            var mopt = opt.MapT(x => x * 2);
            var mres = mopt.FoldT(0, (s, v) => s + v);

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
            var opt = Some(map(tuple(1, "A"), tuple(2, "B"), tuple(3, "C"), tuple(4, "D"), tuple(5, "E")));
            var res = opt.FoldT(0, (s, v) => s + v);
            var mopt = opt.MapT(x => x.ToLower());
            var mres = mopt.FoldT(0, (s, v) => s + v);

            NU.Assert.IsTrue(res == 15);
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
            var opt = Some(list(1, 2, 3, 4, 5));

            var res = from x in opt
                      select x * 2;

            match(res,
                Some: x =>
                {
                    NU.Assert.IsTrue(x[0] == 2);
                    NU.Assert.IsTrue(x[1] == 4);
                    NU.Assert.IsTrue(x[2] == 6);
                    NU.Assert.IsTrue(x[3] == 8);
                    NU.Assert.IsTrue(x[4] == 10);
                },
                None: () => NU.Assert.Fail()
            );
        }

        [NU.Test]
        public void WrappedMapLinqTest()
        {
            var opt = Some(map(tuple(1, "A"), tuple(2, "B"), tuple(3, "C"), tuple(4, "D"), tuple(5, "E")));

            var res = from x in opt
                      select x.ToLower();

            match(res,
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
        public void WrappedOptionLinqTest()
        {
            var opt = Some(Some(100));

            var res = from x in opt
                      select x * 2;

            NU.Assert.IsTrue(res.IfNone(0).IfNone(0) == 200);

            opt = Some<Option<int>>(None);

            res = from x in opt
                  select x * 2;

            NU.Assert.IsTrue(res.IfNone(0).IfNone(1) == 1);
        }

        [NU.Test]
        public void WrappedTryOptionLinqTest()
        {
            Option<TryOption<int>> opt = Some<TryOption<int>>(() => Some(100));

            var res = from x in opt
                      select x * 2;

            NU.Assert.IsTrue(res.IfNone(() => 0).IfNone(0) == 200);

            opt = Some<TryOption<int>>(() => None);

            res = from x in opt
                  select x * 2;

            NU.Assert.IsTrue(res.IfNone(() => 0).IfNone(1) == 1);
        }

        [NU.Test]
        public void WrappedEitherLinqTest()
        {
            var opt = Some(Right<string, int>(100));

            var res = from x in opt
                      select x * 2;

            NU.Assert.IsTrue(res.IfNone(0).IfLeft(0) == 200);

            opt = Some(Left<string, int>("left"));

            res = from x in opt
                  select x * 2;

            NU.Assert.IsTrue(res.IfNone(0).IfLeft(1) == 1);
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