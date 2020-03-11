using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Async
{
    public class TryOptionAsyncTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<TryOptionAsync<A>> ma, TryAsync<TryOptionAsync<A>> mb) =>
            EqAsyncClass<TryAsync<TryOptionAsync<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void FailIsSuccFail()
        {
            var ma = TryOptionAsync<TryAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TryAsync(TryOptionAsync<int>(new Exception("fail")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void NoneIsSuccNone()
        {
            var ma = TryOptionalAsync<TryAsync<int>>(None);
            var mb = ma.Sequence();
            var mc = TryAsync(TryOptionalAsync<int>(None));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = TryOptionAsyncSucc(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsync<TryOptionAsync<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = TryOptionAsync(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(TryOptionAsync(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
