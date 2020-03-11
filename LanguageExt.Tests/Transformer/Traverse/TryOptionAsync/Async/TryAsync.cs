using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Async
{
    public class TryAsyncTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<TryAsync<A>> ma, TryOptionAsync<TryAsync<A>> mb) =>
            EqAsyncClass<TryOptionAsync<TryAsync<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void FailIsSuccFail()
        {
            var ma = TryAsync<TryOptionAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(TryAsync<int>(new Exception("fail")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = TryAsyncSucc<TryOptionAsync<int>>(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<TryAsync<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccNoneIsNone()
        {
            var ma = TryAsyncSucc<TryOptionAsync<int>>(TryOptionAsync<int>(None));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<TryAsync<int>>(None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }      
        
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = TryAsync(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(TryAsync(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
