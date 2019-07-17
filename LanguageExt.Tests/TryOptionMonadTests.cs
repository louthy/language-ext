using Xunit;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System;

namespace LanguageExt.Tests
{
    
    public class TryMonadTests
    {
        [Fact]
        public void TryOptionListTest()
        {
            var x = from a in List(1,2,3)
                    let  b = GetSomeValue(true).ToOption()
                    select b;                       // returns a list of Option
        }

        [Fact]
        public void TryMatchSuccessTest1()
        {
            GetSomeValue(true).Match(
                Some: v  => Assert.True(v == "Hello, World"),
                None: () => Assert.False(true),
                Fail: e  => Assert.False(true)
            );
        }

        [Fact]
        public void TryMatchFailTest1()
        {
            GetFailValue().Match(
                Some: v  => Assert.False(true),
                None: () => Assert.False(true),
                Fail: e  => Assert.True(e.Message == "Whoops")
            );
        }

        [Fact]
        public void FuncTryMatchSuccessTest1()
        {
            match( 
                GetValue(true),
                Some: v  => Assert.True(v == "Hello, World"),
                None: () => Assert.False(true),
                Fail: e  => Assert.False(true)
            );
        }

        [Fact]
        public void FuncTryMatchNoneTest1()
        {
            match(
                GetValue(false),
                Some: v  => Assert.False(true),
                None: () => Assert.True(true),
                Fail: e  => Assert.False(true)
            );
        }

        [Fact]
        public void FuncFailureTryMatchSuccessTest1()
        {
            Assert.True(
                ifNoneOrFail(GetValue(true), "failed") == "Hello, World"
                );
        }

        [Fact]
        public void FuncFailureTryMatchFailTest1()
        {
            Assert.True(
                ifNoneOrFail(GetValue(false), "failed") == "failed"
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
