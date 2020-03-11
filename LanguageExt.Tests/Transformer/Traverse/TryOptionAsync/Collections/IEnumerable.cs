using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Collections
{
    public class IEnumerableTryOptionAsync
    {
        static IEnumerable<A> mkEnum<A>(params A[] items)
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }

        static IEnumerable<int> EmptyA => new int[0];
        static TryOptionAsync<IEnumerable<int>> EmptyMA => TryOptionAsync(EmptyA);
        static Task<bool> Eq(TryOptionAsync<IEnumerable<int>> ma, TryOptionAsync<IEnumerable<int>> mb) =>
            EqAsyncClass<TryOptionAsync<IEnumerable<int>>>.EqualsAsync(ma, mb);

        [Fact]
        public async void EmptyIsSuccEmpty_Parallel()
        {
            var ma = mkEnum<TryOptionAsync<int>>();

            var mb = ma.SequenceParallel();

            var mc = EmptyMA;

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }
        
        [Fact]
        public async void EmptyIsSuccEmpty_Serial()
        {
            var ma = mkEnum<TryOptionAsync<int>>();

            var mb = ma.SequenceSerial();

            var mc = EmptyMA;

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }
        
        [Fact]
        public async void CollectionOfSuccsIsSomeCollection_Parallel()
        {
            var ma = mkEnum(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync(3));

            var mb = ma.SequenceParallel();

            var mc = TryOptionAsync(mkEnum(1, 2, 3));

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }

        [Fact]
        public async void CollectionOfSuccsIsSomeCollection_Serial()
        {
            var ma = mkEnum(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync(3));

            var mb = ma.SequenceSerial();

            var mc = TryOptionAsync(mkEnum(1, 2, 3));

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }
        
        [Fact]
        public async void CollectionOfSuccsAndFailsIsFail_Parallel()
        {
            var ma = mkEnum(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync<int>(new Exception("fail")));

            var mb = ma.SequenceParallel();

            var mc = TryOptionAsync<IEnumerable<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }
        
                
        [Fact]
        public async void CollectionOfSuccsAndFailsIsNone_Serial()
        {
            var ma = mkEnum(TryOptionAsync(1), TryOptionAsync(2), TryOptionAsync<int>(new Exception("fail")));

            var mb = ma.SequenceSerial();

            var mc = TryOptionAsync<IEnumerable<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            Assert.True(mr);        
        }
    }
}
