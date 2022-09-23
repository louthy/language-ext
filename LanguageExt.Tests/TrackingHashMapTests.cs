using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.HashMap;
using Xunit;
using System;
using System.Linq;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests
{
    public class TrackingHashMapTests

    {
        [Fact]
        public void itemLensGetShouldGetExistingValue()
        {
            var expected = "3";
            var map = TrackingHashMap((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
            var actual = TrackingHashMap<int, string>.item(3).Get(map);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void itemLensGetShouldThrowExceptionForNonExistingValue()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var map = TrackingHashMap((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
                var actual = TrackingHashMap<int, string>.item(10).Get(map);
            });
        }

        [Fact]
        public void itemOrNoneLensGetShouldGetExistingValue()
        {
            var expected = "3";
            var map = TrackingHashMap((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
            var actual = TrackingHashMap<int, string>.itemOrNone(3).Get(map);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void itemOrNoneLensGetShouldReturnNoneForNonExistingValue()
        {
            var expected = Option<string>.None;
            var map = TrackingHashMap((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
            var actual = TrackingHashMap<int, string>.itemOrNone(10).Get(map);

            Assert.Equal(expected, actual);
        }
    }
}
