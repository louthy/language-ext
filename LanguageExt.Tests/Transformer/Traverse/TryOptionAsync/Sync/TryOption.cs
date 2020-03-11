using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Sync
{
    public class TryOptionTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<TryOption<A>> ma, TryOptionAsync<TryOption<A>> mb) =>
            EqAsyncClass<TryOptionAsync<TryOption<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void FailIsSuccFail()
        {
            var ma = TryOptionFail<TryOptionAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(TryOptionFail<int>(new Exception("fail")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = TryOption(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionAsyncFail<TryOption<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = TryOption(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(TryOption(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
