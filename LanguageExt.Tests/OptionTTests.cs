using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class OptionTTests
    {
        [Test]
        public void WrappedListTest()
        {
            var opt  = Some(list(1, 2, 3, 4, 5));
            var res  = fold(opt, 0, (s, v) => s + v);
            var mopt = map(opt, x => x * 2);
            var mres = fold(mopt, 0, (s, v) => s + v);

            Assert.IsTrue(res == 15);
            Assert.IsTrue(mres == 30);
            Assert.IsTrue(opt.Count() == 5);
            Assert.IsTrue(mopt.Count() == 5);

            opt = None;
            res = fold(opt, 0, (s, v) => s + v);

            Assert.IsTrue(res == 0);
            Assert.IsTrue(opt.Count() == 0);
        }

        [Test]
        public void WrappedMapTest()
        {
            var opt = Some(map(tuple(1,"A"), tuple(2, "B"), tuple(3, "C"), tuple(4, "D"), tuple(5, "E")));
            var res = opt.Fold(0, (s, v) => s + v);
            var mopt = opt.Map(x => x.ToLower());
            var mres = mopt.Fold(0, (s, v) => s + v);

            Assert.IsTrue(res == 15);
            Assert.IsTrue(opt.Count() == 5);
            Assert.IsTrue(mopt.Count() == 5);

            match( mopt,
                Some: x =>
                {
                    Assert.IsTrue(x[1] == "a");
                    Assert.IsTrue(x[2] == "b");
                    Assert.IsTrue(x[3] == "c");
                    Assert.IsTrue(x[4] == "d");
                    Assert.IsTrue(x[5] == "e");
                },
                None: () => Assert.Fail()
            );
        }

    }
}
