using System;
using static LanguageExt.Prelude;
using Xunit;

namespace LanguageExt.Tests
{
    public class WithOptTests
    {
        [Fact]
        public void TestDefaultIsNone()
        {
            WithOpt<int> x = default;

            Assert.True(x.IfNone(100) == 100);
        }

        [Fact]
        public void TestImplicitCastIsSome()
        {
            WithOpt<int> x = 200;

            Assert.True(x.IfNone(0) == 200);
        }

    }
}
