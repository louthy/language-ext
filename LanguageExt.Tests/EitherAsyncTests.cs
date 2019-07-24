using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    using Xunit;

    public class EitherAsyncTests
    {
        [Fact]
        public async Task ToAsyncExtensionLeftTest()
        {
            var eitherAsync = Left<Exception, Unit>(new Exception()).AsTask().ToAsync();

            Assert.True(await eitherAsync.IsLeft);
        }
    }
}
