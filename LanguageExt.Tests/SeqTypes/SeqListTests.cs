using System.Collections.Generic;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.SeqTypes
{
    public class SeqListTests
    {
        [Fact]
        public void Take_ZeroFromNonempty_Empty()
        {
            var seq    = toSeq(new List<int> { 0 });
            var actual = seq.Take(0);
            Assert.Equal(actual, SeqEmpty.Default);
        }

        [Fact]
        public void Take_NegativeFromNonempty_Empty()
        {
            var seq    = toSeq(new List<int> { 0 });
            var actual = seq.Take(-1);
            Assert.Equal(actual, SeqEmpty.Default);
        }

        [Fact]
        public void Skip_NegativeFromNonempty_Unchanged()
        {
            var expected = toSeq(new List<int> { 0 });
            var actual   = expected.Skip(-1);
            Assert.Equal(actual, expected);
        }

    }
}
