using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Sync
{
    public class TryTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<Try<A>> ma, TryAsync<Try<A>> mb) =>
            EqAsyncClass<TryAsync<Try<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void FailIsSuccFail()
        {
            var ma = TryFail<TryAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TryAsync(TryFail<int>(new Exception("fail")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = Try(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsyncFail<Try<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = Try(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(Try(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
