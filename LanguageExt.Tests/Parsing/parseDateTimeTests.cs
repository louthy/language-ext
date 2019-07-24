using System;
using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public class parseDateTimeTests : AbstractParseTTests<DateTime>
    {
        protected override Option<DateTime> ParseT(string value) => Prelude.parseDateTime(value);

        [Fact]
        public void parseDateTime_ValidStringFromNewMillennium_SomeNewMillennium() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(new DateTime(2001, 1, 1));
    }
}
