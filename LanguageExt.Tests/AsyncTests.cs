using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    public class AsyncTests
    {
        [Fact]
        public void AsyncLINQTest1()
        {
            var computation = from x in Action1().Async()
                              from y in Action2().Async()
                              from z in Action3().Async()
                              from e in Action4().Async()
                              select x + y + z + e;

            var res1 = computation().Result;
            var res2 = computation().Result;
        }

        public async Task<int> Action1()
        {
            await Task.Delay(100);
            return 100;
        }

        public async Task<int> Action2()
        {
            await Task.Delay(200);
            return 200;
        }

        public async Task<int> Action3()
        {
            await Task.Delay(300);
            return 300;
        }

        public async Task<int> Action4()
        {
            await Task.Delay(400);

            throw new Exception("Bleruehr");

            return 400;
        }

    }
}
