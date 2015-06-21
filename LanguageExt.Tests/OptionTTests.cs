using LanguageExt.Trans;
using static LanguageExt.Prelude;

using NU = NUnit.Framework;

namespace LanguageExtTests
{
    [NU.TestFixture]
    public class OptionTTests
    {
        [NU.Test]
        public void WrappedListTest()
        {
            var opt  = Some(list(1, 2, 3, 4, 5));
            var res  = opt.FoldT(0, (s, v) => s + v);
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
            var opt = Some(map(tuple(1,"A"), tuple(2, "B"), tuple(3, "C"), tuple(4, "D"), tuple(5, "E")));
            var res = opt.FoldT(0, (s, v) => s + v);
            var mopt = opt.MapT(x => x.ToLower());
            var mres = mopt.FoldT(0, (s, v) => s + v);

            NU.Assert.IsTrue(res == 15);
            NU.Assert.IsTrue(opt.CountT() == 5);
            NU.Assert.IsTrue(mopt.CountT() == 5);

            match( mopt,
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

    }
}
