using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.CondExt;
using Newtonsoft.Json;

namespace LanguageExt.Tests
{
    public class CondTest
    {
        [Fact]
        public void SynchronousTests()
        {
            var cond1 = Cond<int>(x => x == 4).Then(true).Else(false);
            var cond2 = Subj<int>().Where(x => x == 4).Then(true).Else(false);
            var cond3 = Subj<int>().Any(x => x == 4, x => x > 4).Then(true).Else(false);
            var cond4 = Subj<int>().All(x => x > 0, x => x < 10).Then(true).Else(false);
            var cond5 = Subj<int>().Where(x => x > 0).Where(x => x < 10).Then(true).Else(false);

            Assert.True(cond1(4));
            Assert.False(cond1(2));

            Assert.True(cond2(4));
            Assert.False(cond2(2));

            Assert.True(cond3(5));
            Assert.False(cond3(3));

            Assert.False(cond4(0));
            Assert.False(cond4(10));
            Assert.True(cond4(5));

            Assert.False(cond5(0));
            Assert.False(cond5(10));
            Assert.True(cond5(5));

            Assert.True(4.Apply(cond1));
            Assert.False(2.Apply(cond1));

            Assert.True(4.Apply(cond2));
            Assert.False(2.Apply(cond2));

            Assert.True(5.Apply(cond3));
            Assert.False(3.Apply(cond3));

            Assert.False(0.Apply(cond4));
            Assert.False(10.Apply(cond4));
            Assert.True(5.Apply(cond4));

            Assert.False(0.Apply(cond5));
            Assert.False(10.Apply(cond5));
            Assert.True(5.Apply(cond5));
        }

        static Task<A> T<A>(A value) => 
            value.AsTask();

        Task<int> GetIntegerValue(int x) => x.AsTask();

        [Fact]
        public async Task AsyncTest2()
        {
            var cond = Cond<int>(async x => (await GetIntegerValue(x)) > 0).Then(x => 1).Else(x => 0);

            var a = await cond(100);  // 1
            var b = await cond(-100);  // 0

            Assert.True(a == 1);
            Assert.True(b == 0);
        }

        [Fact]
        public async Task AsynchronousTests()
        {
            var cond1 = Cond<int>(x => T(x == 4)).Then(true).Else(false);
            var cond2 = Subj<int>().Where(x => T(x == 4)).Then(true).Else(false);
            var cond3 = Subj<int>().Any(x => T(x == 4), x => T(x > 4)).Then(true).Else(false);
            var cond4 = Subj<int>().All(x => T(x > 0), x => T(x < 10)).Then(true).Else(false);
            var cond5 = Subj<int>().Where(x => T(x == 4)).Then(true).Else(false);
            var cond6 = Subj<int>().Any(x => T(x == 4), x => T(x > 4)).Then(_ => T(true)).Else(_ => false);
            var cond7 = Subj<int>().Any(x => T(x == 4), x => T(x > 4)).Then(_ => T(true)).Else(_ => T(false));
            var cond8 = Subj<int>().Any(x => T(x == 4), x => T(x > 4)).Then(T(true)).Else(false);
            var cond9 = Subj<int>().Any(x => T(x == 4), x => T(x > 4)).Then(T(true)).Else(T(false));
            var condA = Cond<int>(x => x == 4).Then(_ => T(true)).Else(false);
            var condB = Cond<int>(x => x == 4).Then(true).Else(_ => T(false));
            var condC = Cond<int>(x => x == 4).Then(_ => T(true)).Else(_ => false);

            Assert.True(await cond1(4));
            Assert.False(await cond1(3));

            Assert.True(await cond2(4));
            Assert.False(await cond2(3));

            Assert.True(await cond3(4));
            Assert.True(await cond3(5));
            Assert.False(await cond3(0));

            Assert.True(await cond4(5));
            Assert.False(await cond4(0));
            Assert.False(await cond4(10));

            Assert.True(await cond5(4));
            Assert.False(await cond5(3));

            Assert.False(await cond6(3));
            Assert.True(await cond6(4));

            Assert.False(await cond7(3));
            Assert.True(await cond7(4));
            Assert.True(await cond7(5));

            Assert.False(await cond8(3));
            Assert.True(await cond8(4));

            Assert.False(await cond9(3));
            Assert.True(await cond9(4));
            Assert.True(await cond9(5));

            Assert.True(await condA(4));
            Assert.True(await condB(4));
            Assert.True(await condC(4));

            Assert.False(await condA(5));
            Assert.False(await condB(5));
            Assert.False(await condC(5));
        }
    }
}
