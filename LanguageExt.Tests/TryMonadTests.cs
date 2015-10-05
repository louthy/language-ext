using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    
    public class TryOptionMonadTests
    {
        [Fact]
        public void TryOddNumber1()
        {
            var res = match( from x in OddNumberCrash(10)
                             from y in OddNumberCrash(10)
                             from z in OddNumberCrash(10)
                             select x * y * z,
                             Succ: v => v,
                             Fail: 0 );

            Assert.True(res == 1000);
        }

        [Fact]
        public void TryOddNumber2()
        {
            var res = match( from x in OddNumberCrash(10)
                             from y in OddNumberCrash(9)
                             from z in OddNumberCrash(10)
                             select x * y * z,
                             Succ: v => v,
                             Fail: 0 );

            Assert.True(res == 0);
        }

        [Fact]
        public void TryLinq1()
        {
            var res = match( from x in Num(10)
                             from y in Num(10)
                             from z in Num(10)
                             select x * y * z,
                             Succ: v => v,
                             Fail: 0 );

            Assert.True(res == 1000);
        }

        [Fact]
        public void TryLinq2()
        {
            var res = match( from x in Num(10)
                             from y in Num(10)
                             from z in Num(10,false)
                             select x * y * z,
                             Succ: v => v,
                             Fail: 0 );

            Assert.True(res == 0);
        }

        [Fact]
        public void TryLinq3()
        {
            var res = match(from x in Num(10, false)
                            from y in Num(10)
                            from z in Num(10)
                            select x * y * z,
                             Succ: v => v,
                             Fail: 0);

            Assert.True(res == 0);
        }

        [Fact]
        public void TryMatchSuccessTest1()
        {
            GetValue(true).Match(
                Succ: v  => Assert.True(v == "Hello, World"),
                Fail: e  => Assert.False(true)
            );
        }

        [Fact]
        public void TryMatchFailTest1()
        {
            GetValue(false).Match(
                Succ: v  => Assert.False(true),
                Fail: e  => Assert.True(e.Message == "Failed!")
            );
        }

        [Fact]
        public void FuncTryMatchSuccessTest1()
        {
            match( 
                GetValue(true),
                Succ: v  => Assert.True(v == "Hello, World"),
                Fail: e  => Assert.False(true)
            );
        }

        [Fact]
        public void FuncTryMatchNoneTest1()
        {
            match(
                GetValue(false),
                Succ: v  => Assert.False(true),
                Fail: e => Assert.True(e.Message == "Failed!")
            );
        }

        [Fact]
        public void FuncFailureTryMatchSuccessTest1()
        {
            Assert.True(
                ifFail(GetValue(true), "failed") == "Hello, World"
                );
        }

        [Fact]
        public void FuncFailureTryMatchFailTest1()
        {
            Assert.True(
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

        Try<int> OddNumberCrash(int x) => () =>
        {
            if (x % 2 == 0)
                return x;
            else
                throw new System.Exception("Any exception");
        };
    }
}
