using Xunit;
using System;
using System.Linq;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Collections
{
    public class IEnumerableOptionAsync
    {
        [Fact]
        public async void EmptyIsSomeEmpty()
        {
            var ma = new OptionAsync<int>[0].AsEnumerable();

            var mb = ma.Sequence();

            var mc = SomeAsync(new int[0].AsEnumerable());
            
            var mr = await (mb.Equals<MEnumerable<int>>(mc));
            
            Assert.True(mr);
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection()
        {
            var ma = new [] { SomeAsync(1), SomeAsync(2), SomeAsync(3) }.AsEnumerable();

            var mb = ma.Sequence();

            var mc = SomeAsync(new[] {1, 2, 3}.AsEnumerable());

            var tb = await mb.Map(bs => Seq(bs)).ToStringAsync();
            var tc = await mc.Map(cs => Seq(cs)).ToStringAsync();

            var mr = await (mb.Equals<MEnumerable<int>>(mc));
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone()
        {
            var ma = new[] {SomeAsync(1), SomeAsync(2), None}.AsEnumerable();

            var mb = ma.Sequence();

            Assert.True(await (mb == None));
        }
    }
}
