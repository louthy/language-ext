using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Async
{
    public class TryOptionAsyncTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<TryOptionAsync<A>> ma, TryOptionAsync<TryOptionAsync<A>> mb) =>
            EqAsyncClass<TryOptionAsync<TryOptionAsync<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void FailIsSuccFail()
        {
            var ma = TryOptionAsync<TryOptionAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(TryOptionAsync<int>(new Exception("fail")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void NoneIsSuccNone()
        {
            var ma = TryOptionalAsync<TryOptionAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = TryOptionAsync(TryOptionalAsync<int>(None));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = TryOptionAsyncSucc(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<TryOptionAsync<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
                
        [Fact]
        public async void SuccNoneIsNone()
        {
            var ma = TryOptionAsyncSucc(TryOptionAsync<int>(None));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<TryOptionAsync<int>>(None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = TryOptionAsync(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(TryOptionAsync(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
