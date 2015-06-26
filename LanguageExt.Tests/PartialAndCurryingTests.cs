using static LanguageExt.Prelude;
using static LanguageExt.List;
using LanguageExt;
using LanguageExt.Trans;
using LanguageExt.Trans.Linq;

using NU = NUnit.Framework;

namespace LanguageExtTests
{
    [NU.TestFixture]
    class PartialAndCurryingTests
    {
        [NU.Test]
        public void CurryTest()
        {
            var add = curry((int x, int y) => x + y);
            NU.Assert.IsTrue(add(10)(5) == 15);
        }

        [NU.Test]
        public void PartialTest1()
        {
            var partial = curry((int x, int y) => x + y)(10);

            NU.Assert.IsTrue(partial(5) == 15);
        }

        [NU.Test]
        public void PartialTest2()
        {
            var partial = par((int x, int y) => x + y, 10);

            NU.Assert.IsTrue(partial(5) == 15);
        }

        [NU.Test]
        public void PartialTest3()
        {
            var partial = par((int x, int y, int c, int d) => x + y + c + d, 10, 10);

            NU.Assert.IsTrue(partial(5, 5) == 30);
        }
    }
}
