using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Collections
{
    public class IEnumerableTryAsync
    {
        static IEnumerable<A> mkEnum<A>(params A[] items)
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }

        static IEnumerable<int> EmptyA => new int[0];
        static TryAsync<IEnumerable<int>> EmptyMA => TryAsync(EmptyA);
        static Task<bool> Eq(TryAsync<IEnumerable<int>> ma, TryAsync<IEnumerable<int>> mb) =>
            EqAsyncClass<TryAsync<IEnumerable<int>>>.EqualsAsync(ma, mb);

        [Fact]
        public async void EmptyIsSuccEmpty_Parallel()
        {
            var ma = mkEnum<TryAsync<int>>();

            var mb = ma.SequenceParallel();

            var mc = EmptyMA;

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }
        
        [Fact]
        public async void EmptyIsSuccEmpty_Serial()
        {
            var ma = mkEnum<TryAsync<int>>();

            var mb = ma.SequenceSerial();

            var mc = EmptyMA;

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }
        
        [Fact]
        public async void CollectionOfSuccsIsSomeCollection_Parallel()
        {
            var ma = mkEnum(TryAsync(1), TryAsync(2), TryAsync(3));

            var mb = ma.SequenceParallel();

            var mc = TryAsync(mkEnum(1, 2, 3));

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }

        [Fact]
        public async void CollectionOfSuccsIsSomeCollection_Serial()
        {
            var ma = mkEnum(TryAsync(1), TryAsync(2), TryAsync(3));

            var mb = ma.SequenceSerial();

            var mc = TryAsync(mkEnum(1, 2, 3));

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail_Parallel()
        {
            var ma = mkEnum(TryAsync(1), TryAsync(2), TryAsync<int>(new Exception("fail")));

            var mb = ma.SequenceParallel();

            var mc = TryAsync<IEnumerable<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }
        
                
        [Fact]
        public async void CollectionOfSuccsAndFailsIsNone_Serial()
        {
            var ma = mkEnum(TryAsync(1), TryAsync(2), TryAsync<int>(new Exception("fail")));

            var mb = ma.SequenceSerial();

            var mc = TryAsync<IEnumerable<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }
    }
}
