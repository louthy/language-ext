using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Sync
{
    public class TryOptionTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<TryOption<A>> ma, TryAsync<TryOption<A>> mb) =>
            EqAsyncClass<TryAsync<TryOption<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void FailIsSuccFail()
        {
            var ma = TryOptionFail<TryAsync<int>>(new Exception("fail"));
            var mb = ma.Sequence();
            var mc = TryAsync(TryOptionFail<int>(new Exception("fail")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccFailIsFail()
        {
            var ma = TryOption(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsyncFail<TryOption<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccSuccIsSuccSucc()
        {
            var ma = TryOption(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(TryOption(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
