using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Sync
{
    public class OptionUnsafeTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<OptionUnsafe<A>> ma, TryAsync<OptionUnsafe<A>> mb) =>
            EqAsyncClass<TryAsync<OptionUnsafe<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void NoneIsSuccNone()
        {
            var ma = OptionUnsafe<TryAsync<int>>.None;
            var mb = ma.Sequence();
            var mc = TryAsync(OptionUnsafe<int>.None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeFailIsFail()
        {
            var ma = SomeUnsafe<TryAsync<int>>(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsync<OptionUnsafe<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSuccIsSuccSome()
        {
            var ma = SomeUnsafe(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(SomeUnsafe(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
