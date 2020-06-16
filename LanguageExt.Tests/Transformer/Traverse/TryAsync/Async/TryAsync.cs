using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Async
{
    public class TryAsyncTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<TryAsync<A>> ma, TryAsync<TryAsync<A>> mb) =>
            EqAsyncClass<TryAsync<TryAsync<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void FailIsSuccFail()
        {
            var ma = TryAsync<TryAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TryAsync(TryAsync<int>(new Exception("fail")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = TryAsyncSucc<TryAsync<int>>(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsync<TryAsync<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = TryAsync(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(TryAsync(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
