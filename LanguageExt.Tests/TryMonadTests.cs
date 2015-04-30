using NUnit.Framework;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class TryMonadTests
    {
        [Test]
        public void TryMatchSuccessTest1()
        {
            GetSomeValue(true).Match(
                Some: v  => Assert.IsTrue(v == "Hello, World"),
                None: () => Assert.Fail(),
                Fail: e  => Assert.Fail()
            );
        }

        [Test]
        public void TryMatchFailTest1()
        {
            GetFailValue().Match(
                Some: v  => Assert.Fail(),
                None: () => Assert.Fail(),
                Fail: e  => Assert.IsTrue(e.Message == "Whoops")
            );
        }

        [Test]
        public void FuncTryMatchSuccessTest1()
        {
            match( 
                GetValue(true),
                Some: v  => Assert.IsTrue(v == "Hello, World"),
                None: () => Assert.Fail(),
                Fail: e  => Assert.Fail()
            );
        }

        [Test]
        public void FuncTryMatchNoneTest1()
        {
            match(
                GetValue(false),
                Some: v  => Assert.Fail(),
                None: () => Assert.IsTrue(true),
                Fail: e  => Assert.Fail()
            );
        }

        [Test]
        public void FuncFailureTryMatchSuccessTest1()
        {
            Assert.IsTrue(
                failure(GetValue(true), "failed") == "Hello, World"
                );
        }

        [Test]
        public void FuncFailureTryMatchFailTest1()
        {
            Assert.IsTrue(
                failure(GetValue(false), "failed") == "failed"
                );
        }

        public TryOption<string> GetSomeValue(bool select) =>
            () => select
                ? Some("Hello, World")
                : None;

        public TryOption<string> GetValue(bool select) =>
            () => select
                ? "Hello, World"
                : (string)null;

        public TryOption<string> GetFailValue() =>
            () => failwith<string>("Whoops");

    }
}
