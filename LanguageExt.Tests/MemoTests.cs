using NUnit.Framework;
using System;
using LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class MemoTests
    {
        [Test] public void MemoTest()
        {
            var saved = DateTime.Now;
            var date = saved;

            var f = memo(() => date.ToString());

            var res1 = f();

            date = DateTime.Now.AddDays(1);

            var res2 = f();

            Assert.IsTrue(res1 == res2);
        }
    }
}
