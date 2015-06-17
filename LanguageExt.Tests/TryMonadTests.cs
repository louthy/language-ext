using NUnit.Framework;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class TryOptionMonadTests
    {
        [Test]
        public void TryLinq1()
        {
            var res = match( from x in Num(10)
                             from y in Num(10)
                             from z in Num(10)
                             select x * y * z,
                             Succ: v => v,
                             Fail: 0 );

            Assert.IsTrue(res == 1000);
        }

        [Test]
        public void TryLinq2()
        {
            var res = match( from x in Num(10)
                             from y in Num(10)
                             from z in Num(10,false)
                             select x * y * z,
                             Succ: v => v,
                             Fail: 0 );

            Assert.IsTrue(res == 0);
        }

        [Test]
        public void TryLinq3()
        {
            var res = match(from x in Num(10, false)
                            from y in Num(10)
                            from z in Num(10)
                            select x * y * z,
                             Succ: v => v,
                             Fail: 0);

            Assert.IsTrue(res == 0);
        }

        [Test]
        public void TryMatchSuccessTest1()
        {
            GetValue(true).Match(
                Succ: v  => Assert.IsTrue(v == "Hello, World"),
                Fail: e  => Assert.Fail()
            );
        }

        [Test]
        public void TryMatchFailTest1()
        {
            GetValue(false).Match(
                Succ: v  => Assert.Fail(),
                Fail: e  => Assert.IsTrue(e.Message == "Failed!")
            );
        }

        [Test]
        public void FuncTryMatchSuccessTest1()
        {
            match( 
                GetValue(true),
                Succ: v  => Assert.IsTrue(v == "Hello, World"),
                Fail: e  => Assert.Fail()
            );
        }

        [Test]
        public void FuncTryMatchNoneTest1()
        {
            match(
                GetValue(false),
                Succ: v  => Assert.Fail(),
                Fail: e => Assert.IsTrue(e.Message == "Failed!")
            );
        }

        [Test]
        public void FuncFailureTryMatchSuccessTest1()
        {
            Assert.IsTrue(
                ifFail(GetValue(true), "failed") == "Hello, World"
                );
        }

        [Test]
        public void FuncFailureTryMatchFailTest1()
        {
            Assert.IsTrue(
                ifFail(GetValue(false), "failed") == "failed"
                );
        }

        public Try<string> GetValue(bool select) =>
            () => select
                ? "Hello, World"
                : failwith<string>("Failed!");

        public Try<int> Num(int x, bool select = true) =>
            () => select
                ? x
                : failwith<int>("Failed!");
    }
}
