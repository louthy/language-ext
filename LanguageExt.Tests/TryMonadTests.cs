using NUnit.Framework;
using System;
using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class TryMonadTests
    {
        [Test]
        public void TryMatchSuccessTest1()
        {
            GetValue(true).Match(
                Succ: v => Assert.IsTrue(v == "Hello, World"),
                Fail: e => Assert.Fail()
            );
        }

        [Test]
        public void TryMatchFailTest1()
        {
            GetValue(false).Match(
                Succ: v => Assert.Fail(),
                Fail: e => Assert.IsTrue(e.Message == "Whoops")
            );
        }

        [Test]
        public void FuncTryMatchSuccessTest1()
        {
            match( 
                GetValue(true),
                Succ: v => Assert.IsTrue(v == "Hello, World"),
                Fail: e => Assert.Fail()
            );
        }

        [Test]
        public void FuncTryMatchFailTest1()
        {
            match(
                GetValue(false),
                Succ: v => Assert.Fail(),
                Fail: e => Assert.IsTrue(e.Message == "Whoops")
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

        public Try<string> GetValue(bool select) =>
            () => select
                ? "Hello, World"
                : failwith<string>("Whoops");
    }
}
