using Xunit;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System;

namespace LanguageExtTests
{
    
    public class TryOptionMonadTests
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

        [Fact]
        public void TryOptionBindLazy()
        {
            var tried = false;
            var try1 = TryOption(() => tried = true);
            var try2 = TryOption(() => tried = true);

            var tryBoth =
                from a in try1
                from b in try2
                select a || b;
            
            Assert.False(tried);
            tryBoth.Try();
            Assert.True(tried);
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
