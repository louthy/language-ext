using System;
using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public class parseTimeSpanTests : AbstractParseTTests<TimeSpan>
    {
        protected override Option<TimeSpan> ParseT(string value) => Prelude.parseTimeSpan(value);

        [Fact]
        public void parseTimeSpan_valid() =>
            Assert.Equal(Prelude.Some(new TimeSpan(0, 0, 0, 19, 12)), Prelude.parseTimeSpan("00:00:19.0120000"));

        [Theory]
        [InlineData("00:00:19.1200000")]
        [InlineData("00:00:19")]
        [InlineData("00:00")]
        public void parseTimeSpan_multipleValid(string input) =>
            ParseT_ValidStringFromGiven_SomeAsGiven(TimeSpan.Parse(input));

        [Theory]
        [InlineData("")]
        [InlineData("petter")]
        [InlineData("0:60:0")]
        [InlineData("0:0:60")]
        [InlineData(".123")]
        [InlineData("10.12")]
        public void parseTimeSpan_multipleInvalid(string input) => Assert.Equal(Prelude.None, Prelude.parseTimeSpan(input));

    }
}
