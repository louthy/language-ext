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
        public async void EmptyIsSomeEmpty_Parallel()
        {
            var ma = new OptionAsync<int>[0].AsEnumerable();

            var mb = ma.SequenceParallel();

            var mc = SomeAsync(new int[0].AsEnumerable());
            
            var mr = await (mb.Equals<MEnumerable<int>>(mc));
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void EmptyIsSomeEmpty_Serial()
        {
            var ma = new OptionAsync<int>[0].AsEnumerable();

            var mb = ma.SequenceSerial();

            var mc = SomeAsync(new int[0].AsEnumerable());
            
            var mr = await (mb.Equals<MEnumerable<int>>(mc));
            
            Assert.True(mr);
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Parallel()
        {
            var ma = new [] { SomeAsync(1), SomeAsync(2), SomeAsync(3) }.AsEnumerable();

            var mb = ma.SequenceParallel();

            var mc = SomeAsync(new[] {1, 2, 3}.AsEnumerable());

            var mr = await (mb.Equals<MEnumerable<int>>(mc));
            
            Assert.True(mr);
        }
        
        
        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Serial()
        {
            var ma = new [] { SomeAsync(1), SomeAsync(2), SomeAsync(3) }.AsEnumerable();

            var mb = ma.SequenceSerial();

            var mc = SomeAsync(new[] {1, 2, 3}.AsEnumerable());

            var mr = await (mb.Equals<MEnumerable<int>>(mc));
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone_Parallel()
        {
            var ma = new[] {SomeAsync(1), SomeAsync(2), None}.AsEnumerable();

            var mb = ma.SequenceParallel();

            Assert.True(await (mb == None));
        }
        
        [Fact]
        public async void CollectionOfSomesAndNonesIsNone_Serial()
        {
            var ma = new[] {SomeAsync(1), SomeAsync(2), None}.AsEnumerable();

            var mb = ma.SequenceSerial();

            Assert.True(await (mb == None));
        }
    }
}
