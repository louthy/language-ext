using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Sync
{
    public class EitherUnsafeTryOptionAsync
    {
        static Task<bool> Eq<L, R>(TryOptionAsync<EitherUnsafe<L, R>> ma, TryOptionAsync<EitherUnsafe<L, R>> mb) =>
            EqAsyncClass<TryOptionAsync<EitherUnsafe<L, R>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void LeftIsSuccLeft()
        {
            var ma = LeftUnsafe<Error, TryOptionAsync<int>>(Error.New("fail"));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(LeftUnsafe<Error, int>(Error.New("fail")));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightFailIsFail()
        {
            var ma = RightUnsafe<Error, TryOptionAsync<int>>(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<EitherUnsafe<Error, int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightNoneIsNone()
        {
            var ma = RightUnsafe<Error, TryOptionAsync<int>>(TryOptionAsync<int>(None));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<EitherUnsafe<Error, int>>(None);

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSuccIsSuccRight()
        {
            var ma = RightUnsafe<Error, TryOptionAsync<int>>(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(RightUnsafe<Error, int>(1234));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
    }
}
