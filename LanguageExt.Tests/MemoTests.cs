using NUnit.Framework;
using System;
using LanguageExt.List;
using LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class MemoTests
    {
        [Test] public void MemoTest1()
        {
            var saved = DateTime.Now;
            var date = saved;

            var f = memo(() => date.ToString());

            var res1 = f();

            date = DateTime.Now.AddDays(1);

            var res2 = f();

            Assert.IsTrue(res1 == res2);
        }

        [Test]
        public void MemoTest2()
        {
            var fix = 0;

            var m = memo( (int x) => x + fix );

            var nums1 = map(freeze(range(0, 100)), i => m(i));

            fix = 1000;

            var nums2 = map(freeze(range(0, 100)), i => m(i));

            Assert.IsTrue(
                length(filter(zip(nums1, nums2, (a, b) => a == b), v => v)) == 100
                );
        }
    }
}
