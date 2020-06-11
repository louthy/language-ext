using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Async
{
    public class OptionAsyncTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<OptionAsync<A>> ma, TryOptionAsync<OptionAsync<A>> mb) =>
            EqAsyncClass<TryOptionAsync<OptionAsync<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void NoneIsSuccNone()
        {
            var ma = OptionAsync<TryOptionAsync<int>>.None;
            var mb = ma.Sequence();
            var mc = TryOptionAsync(OptionAsync<int>.None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeFailIsFail()
        {
            var ma = SomeAsync(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<OptionAsync<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeNoneIsNone()
        {
            var ma = SomeAsync(TryOptionAsync<int>(None));
            var mb = ma.Sequence();
            var mc = TryOptionalAsync<OptionAsync<int>>(None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSuccIsSuccSome()
        {
            var ma = SomeAsync(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(SomeAsync(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
