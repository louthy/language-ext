using System;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class SeqTests
    {
        [Fact]
        public void ObjectExists()
        {
            var x = "test";

            Assert.True(seqOne(x).Count() == 1);
            Assert.True(seqOne(x).Head() == x);
        }

        [Fact]
        public void ObjectNull()
        {
            string x = null;

            Assert.True(seq(x).Count() == 0);
        }

        [Fact]
        public void EnumerableExists()
        {
            var x = new[] { "a", "b", "c" };

            Assert.True(seq(x).Count() == 3);
            Assert.True(seq(x).Head() == "a");
            Assert.True(seq(x).Tail().Head() == "b");
            Assert.True(seq(x).Tail().Tail().Head() == "c");
        }

        [Fact]
        public void EnumerableNull()
        {
            string[] x = null;

            Assert.True(seq(x).Count() == 0);
        }
    }
}
