using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Sync
{
    public class TryTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<Try<A>> ma, TryOptionAsync<Try<A>> mb) =>
            EqAsyncClass<TryOptionAsync<Try<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void FailIsSuccFail()
        {
            var ma = TryFail<TryOptionAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(TryFail<int>(new Exception("fail")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = Try(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionAsyncFail<Try<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
                
        [Fact]
        public async void SuccNoneIsNone()
        {
            var ma = Try(TryOptionAsync<int>(None));
            var mb = ma.Sequence();
            var mc = TryOptionalAsync<Try<int>>(None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = Try(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(Try(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
