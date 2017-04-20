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

            Assert.True(SeqOne(x).Count() == 1);
            Assert.True(SeqOne(x).Head() == x);
        }

        [Fact]
        public void ObjectNull()
        {
            string x = null;

            Assert.True(Seq(x).Count() == 0);
        }

        [Fact]
        public void EnumerableExists()
        {
            var x = new[] { "a", "b", "c" };

            var y = Seq(x);

            var res = Seq(x).Tail().AsEnumerable().ToList();


            Assert.True(Seq(x).Count() == 3);
            Assert.True(Seq(x).Head() == "a");
            Assert.True(Seq(x).Tail().Head() == "b");
            Assert.True(Seq(x).Tail().Tail().Head() == "c");
        }

        [Fact]
        public void EnumerableNull()
        {
            string[] x = null;

            Assert.True(Seq(x).Count() == 0);
        }
    }
}
