using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

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

            Assert.True(await mr.IfNone(0) == 30);


            var mc = Some(10).ToAsync().Match(
                Some: x => Task.FromResult(x * 10),
                None: () => 0
            );

            Assert.True(await mc == 100);


            Option<int> opt = 4;
            Unit unit = await opt.ToAsync().IfSome(i => DoWork());
        }

        public Task DoWork()
        {
            return Task.Run(() => Console.WriteLine("here"));
        }

        [Fact]
        public async void Issue206()
        {
            var sync = new object();
            var tasks = new List<Task>();
            var output = new List<string>();

            Console.WriteLine("OptionAsync");
            OptionAsync<int> optAsync = Some(4).ToAsync();
            var tskOptionAsync = optAsync.IfSome(async (i) =>
            {
                var x = DoWork();

                lock(sync)
                {
                    tasks.Add(x);
                    output.Add($"Inner id {x.Id}");
                }

                await x;
            });
            lock (sync)
            {
                output.Add($"Outer id {tskOptionAsync.Id}");
                tasks.Add(tskOptionAsync);
            }
            await tskOptionAsync;

            Assert.True(tasks[0].Status == TaskStatus.RanToCompletion);
            Assert.True(tasks[1].Status == TaskStatus.RanToCompletion);

        }
    }
}
