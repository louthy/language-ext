using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Async
{
    public class EitherAsyncTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<EitherAsync<Error, A>> ma, TryAsync<EitherAsync<Error, A>> mb) =>
            EqAsyncClass<TryAsync<EitherAsync<Error, A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void LeftIsSuccLeft()
        {
            var ma = LeftAsync<Error, TryAsync<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = TryAsync(LeftAsync<Error, int>(Error.New("alt")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightFailIsFail()
        {
            var ma = RightAsync<Error, TryAsync<int>>(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsyncFail<EitherAsync<Error, int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSuccIsSuccRight()
        {
            var ma = RightAsync<Error, TryAsync<int>>(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(RightAsync<Error, int>(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
