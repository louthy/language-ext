using LanguageExt;
using static LanguageExt.Prelude;
using Xunit;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests
{
    public class RangeTests
    {
        [Fact]
        public void IntRangeAsc()
        {
            var x = IntegerRange.FromMinMax(2, 5, 1).Freeze();
            Assert.True(x == List(2, 3, 4, 5));
        }

        [Fact]
        public void IntRangeDesc()
        {
            var x = IntegerRange.FromMinMax(5, 2, -1).Freeze();
            Assert.True(x == List(5, 4, 3, 2));
        }

        [Fact]
        public void IntCountAsc()
        {
            var x = IntegerRange.FromCount(2, 5, 2).Freeze();
            Assert.True(x == List(2, 4, 6, 8, 10));
        }

        [Fact]
        public void IntCountDesc()
        {
            var x = IntegerRange.FromCount(2, 5, -2).Freeze();
            Assert.True(x == List(2, 0, -2, -4, -6));
        }

        [Fact]
        public void CharCountAsc()
        {
            var x = CharRange.FromCount('a', 5).Freeze();
            Assert.True(x == List('a', 'b', 'c', 'd', 'e'));
        }

        [Fact]
        public void CharCountDesc()
        {
            var x = CharRange.FromCount('e', (char)5, CharRange.Minus1).Freeze();
            Assert.True(x == List('e', 'd', 'c', 'b', 'a'));
        }


        [Fact]
        public void CharRangeAsc()
        {
            var x = CharRange.FromMinMax('a', 'e').Freeze();
            Assert.True(x == List('a', 'b', 'c', 'd', 'e'));
        }

        [Fact]
        public void CharRangeDesc()
        {
            var x = CharRange.FromMinMax('e', 'a').Freeze();
            Assert.True(x == List('e', 'd', 'c', 'b', 'a'));
        }

    }
}
