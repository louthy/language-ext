using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Sync
{
    public class EitherUnsafeTryAsync
    {
        static Task<bool> Eq<L, R>(TryAsync<EitherUnsafe<L, R>> ma, TryAsync<EitherUnsafe<L, R>> mb) =>
            EqAsyncClass<TryAsync<EitherUnsafe<L, R>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void LeftIsSuccLeft()
        {
            var ma = LeftUnsafe<Error, TryAsync<int>>(Error.New("fail"));
            var mb = ma.Sequence();
            var mc = TryAsync(LeftUnsafe<Error, int>(Error.New("fail")));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightFailIsFail()
        {
            var ma = RightUnsafe<Error, TryAsync<int>>(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsync<EitherUnsafe<Error, int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSuccIsSuccRight()
        {
            var ma = RightUnsafe<Error, TryAsync<int>>(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(RightUnsafe<Error, int>(1234));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
    }
}
