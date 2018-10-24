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

        [Fact]
        public async Task FilterBottomTest()
        {
            var x = RightAsync<string, int>(1).Filter(isDefault);

            Assert.True(await x.IsBottom);
            Assert.True(await x.BindBottom(0).Match(right => right == 0, left => false, () => false));
            Assert.True(await x.BindBottom(() => 0).Match(right => right == 0, left => false, () => false));
            Assert.True(await x.BindBottom(RightAsync<string, int>(0)).Match(right => right == 0, left => false, () => false));
            Assert.True(await x.BindBottom(() => RightAsync<string, int>(0)).Match(right => right == 0, left => false, () => false));

            Assert.True(await x.BindBottom("is not default").Match(right => false, left => left == "is not default", () => false));
            Assert.True(await x.BindBottom(() => "is not default").Match(right => false, left => left == "is not default", () => false));
            Assert.True(await x.BindBottom(LeftAsync<string, int>("is not default")).Match(right => false, left => left == "is not default", () => false));
            Assert.True(await x.BindBottom(() => LeftAsync<string, int>("is not default")).Match(right => false, left => left == "is not default", () => false));

        }
    }
}
