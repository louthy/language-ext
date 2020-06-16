using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Sync
{
    public class OptionUnsafeTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<OptionUnsafe<A>> ma, TryOptionAsync<OptionUnsafe<A>> mb) =>
            EqAsyncClass<TryOptionAsync<OptionUnsafe<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void NoneIsSuccNone()
        {
            var ma = OptionUnsafe<TryOptionAsync<int>>.None;
            var mb = ma.Sequence();
            var mc = TryOptionAsync(OptionUnsafe<int>.None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeFailIsFail()
        {
            var ma = SomeUnsafe<TryOptionAsync<int>>(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<OptionUnsafe<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
                
        [Fact]
        public async void SomeNoneIsNone()
        {
            var ma = SomeUnsafe<TryOptionAsync<int>>(TryOptionAsync<int>(None));
            var mb = ma.Sequence();
            var mc = TryOptionalAsync<OptionUnsafe<int>>(None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSuccIsSuccSome()
        {
            var ma = SomeUnsafe(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(SomeUnsafe(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
