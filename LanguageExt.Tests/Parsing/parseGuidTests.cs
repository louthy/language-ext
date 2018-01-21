using System;
using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public class parseGuidTests : AbstractParseTTests<Guid>
    {
        protected override Option<Guid> ParseT(string value) => Prelude.parseGuid(value);

        [Fact]
        public void ParseGuid_ValidStringFixedGuid_SomeOfSameFixedGuid()
        {
            var guid = Guid.Parse("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
            ParseT_ValidStringFromGiven_SomeAsGiven(guid);
        }
    }
}
