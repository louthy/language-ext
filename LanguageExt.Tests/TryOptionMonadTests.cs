using NUnit.Framework;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System;

namespace LanguageExtTests
{
    [TestFixture]
    public class TryMonadTests
    {
        public void TryOptionListTest()
        {
            var x = from a in List(1,2,3)
                    let  b = GetSomeValue(true).ToOption()
                    select b;                       // returns a list of Option
        }

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
                ifNone(GetValue(true), "failed") == "Hello, World"
                );
        }

        [Test]
        public void FuncFailureTryMatchFailTest1()
        {
            Assert.IsTrue(
                ifNone(GetValue(false), "failed") == "failed"
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
