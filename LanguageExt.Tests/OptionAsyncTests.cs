using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.Tests
{
    public class OptionAsyncTests
    {
        /// <summary>
        /// TODO: This is just some initial tests, will need a full suite
        /// </summary>
        [Fact]
        public async void InitialTests()
        {
            var ma = SomeAsync(_ => 10);
            var mb = SomeAsync(_ => 20);

            var mr = from a in ma
                     from b in mb
                     select a + b;

            Assert.True(await mr.IfNone(0) == 10);


            var mc = Some(10).ToAsync().Match(
                Some: x => Task.FromResult(x * 10),
                None: () => 0
            );

            Assert.True(await mc == 100);


            Option<int> opt = 4;
            Unit unit = await opt.ToAsync().IfSome(i => DoWork());
            unit = ifSome(opt, x => DoWork());
        }

        public Task DoWork()
        {
            return Task.CompletedTask;
        }
    }
}
