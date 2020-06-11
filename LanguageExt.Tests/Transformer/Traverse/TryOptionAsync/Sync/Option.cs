using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Sync
{
    public class OptionTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<Option<A>> ma, TryOptionAsync<Option<A>> mb) =>
            EqAsyncClass<TryOptionAsync<Option<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void NoneIsSuccNone()
        {
            var ma = Option<TryOptionAsync<int>>.None;
            var mb = ma.Sequence();
            var mc = TryOptionAsyncSucc(Option<int>.None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeFailIsFail()
        {
            var ma = Some<TryOptionAsync<int>>(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<Option<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
                
        [Fact]
        public async void SomeNoneIsNone()
        {
            var ma = Some<TryOptionAsync<int>>(TryOptionAsync<int>(None));
            var mb = ma.Sequence();
            var mc = TryOptionalAsync<Option<int>>(None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSuccIsSuccSome()
        {
            var ma = Some(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsyncSucc(Some(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
