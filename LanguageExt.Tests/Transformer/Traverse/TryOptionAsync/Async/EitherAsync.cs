using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Async
{
    public class EitherAsyncTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<EitherAsync<Error, A>> ma, TryOptionAsync<EitherAsync<Error, A>> mb) =>
            EqAsyncClass<TryOptionAsync<EitherAsync<Error, A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void LeftIsSuccLeft()
        {
            var ma = LeftAsync<Error, TryOptionAsync<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(LeftAsync<Error, int>(Error.New("alt")));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightFailIsFail()
        {
            var ma = RightAsync<Error, TryOptionAsync<int>>(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionAsyncFail<EitherAsync<Error, int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightNoneIsNone()
        {
            var ma = RightAsync<Error, TryOptionAsync<int>>(TryOptionAsync<int>(None));
            var mb = ma.Sequence();
            var mc = TryOptionalAsync<EitherAsync<Error, int>>(None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSuccIsSuccRight()
        {
            var ma = RightAsync<Error, TryOptionAsync<int>>(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(RightAsync<Error, int>(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
