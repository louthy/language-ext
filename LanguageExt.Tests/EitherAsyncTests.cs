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
        /// <summary>
        /// Tests for bug / PR #508
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task BottomTest()
        {
            Assert.True(await EitherAsync<int, string>.Bottom.IsBottom);
        }

        [Fact]
        public async Task ToAsyncExtensionLeftTest()
        {
            var eitherAsync = Left<Exception, Unit>(new Exception()).AsTask().ToAsync();

            Assert.True(await eitherAsync.IsLeft);
        }
    }
}
