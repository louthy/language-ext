using NUnit.Framework;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Reactive.Linq;
using System.Threading;

namespace LanguageExtTests
{
    [TestFixture]
    public class DelayTests
    {
        [Test]
        public void DelayTest1()
        {
            var span = TimeSpan.FromMilliseconds(500);
            var till = DateTime.Now.Add(span);
            var v = 0;

            delay(() => 1, span).Subscribe(x => v = x);

            while( DateTime.Now < till )
            {
                Assert.IsTrue(v == 0);
                Thread.Sleep(1);
            }

            while (DateTime.Now < till.AddMilliseconds(50))
            {
                Thread.Sleep(1);
            }

            Assert.IsTrue(v == 1);
        }
    }
}
