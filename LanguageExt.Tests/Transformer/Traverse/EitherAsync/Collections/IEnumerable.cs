using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.EitherAsyncT.Collections
{
    public class IEnumerableEitherAsync
    {
        static IEnumerable<A> mkEnum<A>(params A[] xs)
        {
            foreach (var x in xs)
            {
                yield return x;
            }
        }

        [Fact]
        public async void EmptyIsSomeEmpty_Parallel()
        {
            var ma = mkEnum<EitherAsync<string, int>>();

            var mb = ma.SequenceParallel();

            var mc = RightAsync<string, IEnumerable<int>>(mkEnum<int>());
            
            Assert.True(await (mb == mc));        
        }
        
        [Fact]
        public async void EmptyIsSomeEmpty_Serial()
        {
            var ma = mkEnum<EitherAsync<string, int>>();

            var mb = ma.SequenceSerial();

            var mc = RightAsync<string, IEnumerable<int>>(mkEnum<int>());
            
            Assert.True(await (mb == mc));        
        }
        
        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Parallel()
        {
            var ma = mkEnum(RightAsync<string, int>(1), RightAsync<string, int>(2), RightAsync<string, int>(3));

            var mb = ma.SequenceParallel();

            var mc = RightAsync<string, IEnumerable<int>>(mkEnum(1, 2, 3));

            Assert.True(await (mb == mc));
        }

        [Fact]
        public async void CollectionOfSomesIsSomeCollection_Serial()
        {
            var ma = mkEnum(RightAsync<string, int>(1), RightAsync<string, int>(2), RightAsync<string, int>(3));

            var mb = ma.SequenceSerial();

            var mc = RightAsync<string, IEnumerable<int>>(mkEnum(1, 2, 3));

            Assert.True(await (mb == mc));
        }
        
        [Fact]
        public async void CollectionOfSomesAndLeftsIsLeft_Parallel()
        {
            var ma = mkEnum(RightAsync<string, int>(1), RightAsync<string, int>(2), LeftAsync<string, int>("alt"));

            var mb = ma.SequenceParallel();

            var mc = LeftAsync<string, IEnumerable<int>>("alt");
            
            Assert.True(await (mb == mc));
        }
        
                
        [Fact]
        public async void CollectionOfSomesAndLeftsIsLeft_Serial()
        {
            var ma = mkEnum(RightAsync<string, int>(1), RightAsync<string, int>(2), LeftAsync<string, int>("alt"));

            var mb = ma.SequenceSerial();

            var mc = LeftAsync<string, IEnumerable<int>>("alt");
            
            Assert.True(await (mb == mc));
        }
    }
}
