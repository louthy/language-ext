using System;
using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public class parseDateTimeOffsetTests : AbstractParseTTests<DateTimeOffset>
    {
        protected override Option<DateTimeOffset> ParseT(string value) => Prelude.parseDateTimeOffset(value);

        [Fact]
        public void parseDateTimeOffset_ValidStringFromNewMillennium_SomeUtc() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(new DateTimeOffset(2001, 1, 1, 12, 0, 0, 0 * Prelude.hours));

        [Fact]
        public void parseDateTimeOffset_ValidStringFromNewMillennium_SomeBerlin() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(new DateTimeOffset(2001, 1, 1, 12, 0, 0, 1 * Prelude.hours));

        [Fact]
        public void parseDateTimeOffset_ISO8601_SomeBerlin() =>
            Assert.Equal(Prelude.Some(new DateTimeOffset(2018, 1, 25, 15, 32, 5, 1 * Prelude.hours)), Prelude.parseDateTimeOffset("2018-01-25T15:32:05+01:00"));

        [Fact]
        public void parseDateTimeOffset_ISO8601_SomeUtc() =>
            Assert.Equal(Prelude.Some(new DateTimeOffset(2018, 1, 25, 15, 32, 5, 0 * Prelude.hours)), Prelude.parseDateTimeOffset("2018-01-25T15:32:05Z"));

        [Fact]
        public void parseDateTimeOffset_Universal_SomeBerlin() =>
            Assert.Equal(Prelude.Some(new DateTimeOffset(2018, 1, 25, 15, 32, 5, 1 * Prelude.hours)), Prelude.parseDateTimeOffset("2018-01-25 15:32:05+01:00"));

        [Fact]
        public void parseDateTimeOffset_Universal_SomeUtc() =>
            Assert.Equal(Prelude.Some(new DateTimeOffset(2018, 1, 25, 15, 32, 5, 0 * Prelude.hours)), Prelude.parseDateTimeOffset("2018-01-25 15:32:05Z"));

    }
}
